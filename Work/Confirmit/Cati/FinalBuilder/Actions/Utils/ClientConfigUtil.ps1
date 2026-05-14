#
# AddHostnameURIRestrictions.ps1
#

#######################################################
### External Dependencies. Should be refactored out ###
#######################################################

function GetVariableValueIfExists([string] $variableName)
{
    $secretsPath = Join-Path "c:\etc\confirmit\secrets" $variableName
    if(Test-Path $secretsPath) {
        return Get-Content -Raw $secretsPath
    }
	$variableValue = [Environment]::GetEnvironmentVariable($variableName)
	if($variableValue) { return $variableValue }
		
	$variableValue = Get-Variable -Name $variableName -ValueOnly -errorAction silentlyContinue
	return $variableValue				
}

function GetSqlServerName([string] $applicationId)
{	
	$overrideVariable = "ConfirmitSiteApplication"+$applicationId+"SQLServerName"
    $overrideValue = GetVariableValueIfExists($overrideVariable)	

	if($overrideValue)
	{
		return $overrideValue
	}
	else 
	{
		return $ConfirmitSiteApplicationDefaultSQLServerName    
	}	
}

#######################
### Start of script ###
#######################

function DbGetFirstFieldOrNull($query)
{
	$connectionString = "Server={0};Database=Identity;UID={1};PWD={2}" -f (GetSqlServerName $applicationId), $ConfirmitDatabaseUserDeployCName, $ConfirmitDatabaseUserDeployCPassword
	$conn = New-Object System.Data.SqlClient.SQLConnection
	$conn.ConnectionString=$connectionString
	$conn.Open()
	$cmd = New-Object system.Data.SqlClient.SqlCommand($query,$conn)
	$reader = $cmd.ExecuteReader()
	$val = $null
	
	if ($reader.Read())
	{
		$val = $reader[0]
	}
	$conn.Close()

	return $val
}

function DbRunQueryNoResult($query)
{
	$connectionString = "Server={0};Database=Identity;UID={1};PWD={2}" -f (GetSqlServerName $applicationId), $ConfirmitDatabaseUserDeployCName, $ConfirmitDatabaseUserDeployCPassword
	$conn = New-Object System.Data.SqlClient.SQLConnection
	$conn.ConnectionString=$connectionString
	$conn.Open()
	$cmd = New-Object system.Data.SqlClient.SqlCommand($query,$conn)
	$cmd.ExecuteReader()
	$conn.Close()
}

function GetConfig($url)
{
	$query = "SELECT [ConfigValue] FROM [dbo].[CfgConfig] WHERE [ConfigName] = '{0}'" -f $url

	#we query the confirm database here, so we don't call 'DbGetFirstFieldOrNull' method
	$connectionString = "Server={0};Database=confirm;UID={1};PWD={2}" -f (GetSqlServerName $applicationId), $ConfirmitDatabaseUserDeployCName, $ConfirmitDatabaseUserDeployCPassword
	$conn = New-Object System.Data.SqlClient.SQLConnection
	$conn.ConnectionString=$connectionString
	$conn.Open()
	$cmd = New-Object system.Data.SqlClient.SqlCommand($query,$conn)
	$reader = $cmd.ExecuteReader()
	$cfgval = $null
	#Write-Host ("ran query {0}" -f $query)
	if ($reader.Read())
	{
		$cfgval = $reader[0]
		if ($cfgval -is [System.DBNull])
		{
			$cfgval = $null
		}
	}
	$conn.Close()

	return $cfgval
}

function GetCORSOrigin($client)
{
	$query = ("
		SELECT cco.Origin
		FROM [Identity].[dbo].[Clients] as c 
		LEFT JOIN [Identity].[dbo].[ClientCorsOrigins] as cco on cco.Client_Id = c.Id 
		WHERE ClientId = '{0}'
	" -f $client)
	#Write-Host ("Getting CORS Origin: {0}" -f $query)

	$val = DbGetFirstFieldOrNull $query

	Write-Host ("Got CORSOrigin from database: {0}" -f $val)
	return $val
}

function GetSpecificCORSOrigin($client, $domain)
{
	$query = ("
		SELECT cco.Origin
		FROM [Identity].[dbo].[Clients] as c 
		LEFT JOIN [Identity].[dbo].[ClientCorsOrigins] as cco on cco.Client_Id = c.Id 
		WHERE ClientId = '{0}'
		AND Origin LIKE '%{1}'
	" -f $client, $domain)
	#Write-Host ("Getting CORS Origin: {0}" -f $query)

	$val = DbGetFirstFieldOrNull $query

	Write-Host ("Got CORSOrigin from database: {0}" -f $val)
	return $val
}

function GetRedirectURI($client)
{
	$query = ("
	SELECT Uri 
	FROM [Identity].[dbo].[ClientRedirectUris] 
	WHERE Client_Id = (
		SELECT [Id] 
		FROM [Identity].[dbo].[Clients] 
		WHERE [ClientId] = '{0}'
	)" -f $client)
	#Write-Host ("Getting Redirect URI: {0}" -f $query)

	$val = DbGetFirstFieldOrNull $query

	Write-Host ("Got Redirect URI from database: {0}" -f $val)
	return $val
}

function GetSpecificRedirectURI($client, $domain)
{
	$query = ("
	SELECT Uri 
	FROM [Identity].[dbo].[ClientRedirectUris] 
	WHERE Client_Id = (
		SELECT [Id] 
		FROM [Identity].[dbo].[Clients] 
		WHERE [ClientId] = '{0}')
    AND [URI] LIKE '%{1}'
	" -f $client, $domain)
	#Write-Host ("Getting Redirect URI: {0}" -f $query)

	$val = DbGetFirstFieldOrNull $query

	Write-Host ("Got Redirect URI from database: {0}" -f $val)
	return $val
}

function GetPostLogoutRedirectURI($client)
{
	$query = ("
	SELECT Uri 
	FROM [Identity].[dbo].[ClientPostLogoutRedirectUris] 
	WHERE Client_Id = (
		SELECT [Id] 
		FROM [Identity].[dbo].[Clients] 
		WHERE [ClientId] = '{0}'
	)" -f $client)
	#Write-Host ("Getting PostLogout Redirect URI: {0}" -f $query)

	$val = DbGetFirstFieldOrNull $query

	Write-Host ("Got PostLogout Redirect URI from database: {0}" -f $val)
	return $val
}

function GetSpecificPostLogoutRedirectURI($client, $domain)
{
	$query = ("
	SELECT Uri 
	FROM [Identity].[dbo].[ClientPostLogoutRedirectUris] 
	WHERE Client_Id = (
		SELECT [Id] 
		FROM [Identity].[dbo].[Clients] 
		WHERE [ClientId] = '{0}')
    AND [URI] LIKE '%{1}'
	" -f $client, $domain)
	#Write-Host ("Getting PostLogout Redirect URI: {0}" -f $query)

	$val = DbGetFirstFieldOrNull $query

	Write-Host ("Got PostLogout Redirect URI from database: {0}" -f $val)
	return $val
}

function CreateCORSOrigin($client, $origin) 
{
	$query = "
	INSERT INTO [Identity].[dbo].[ClientCorsOrigins] (Origin, Client_Id) 
	VALUES (
		'{0}', 
		(
			SELECT [Id]
			FROM [Identity].[dbo].[Clients] 
			WHERE [ClientId] = '{1}'
		)
	)" -f $origin, $client
	#Write-Host ("Adding CORS Origin: {0}" -f $query)
	DbRunQueryNoResult $query
}

function UpdateCORSOrigin($client, $origin) 
{
	$query = ("
	UPDATE cco
	SET cco.Origin = '{0}'
	FROM [Identity].[dbo].[ClientCorsOrigins] as cco
	JOIN [Identity].[dbo].[Clients] as c on c.Id = cco.Client_Id
	WHERE c.ClientId = '{1}'" -f $origin, $client)
	#Write-Host ("Updating CORS Origin: {0}" -f $query)
	DbRunQueryNoResult $query
}

function UpdateSpecificCORSOrigin($client, $origindomain, $protocol)
{
	$query = ("
	UPDATE cco
	SET cco.Origin = '{0}{1}'
	FROM [Identity].[dbo].[ClientCorsOrigins] as cco
	JOIN [Identity].[dbo].[Clients] as c on c.Id = cco.Client_Id
	WHERE c.ClientId = '{2}'
	AND cco.Origin LIKE '%{1}'
	" -f $protocol, $origindomain, $client)
	#Write-Host ("Updating CORS Origin: {0}" -f $query)
	DbRunQueryNoResult $query
}

function CreateRedirectURI($client, $domain, $protocol)
{
	$query = ("INSERT INTO [Identity].[dbo].[ClientRedirectUris] (URI, Client_Id) 
	VALUES (
		'{0}{1}', 
		(
			SELECT [Id] 
			FROM [Identity].[dbo].[Clients] 
			WHERE [ClientId] = '{2}'
		)
	)" -f $protocol, $domain, $client)
	#Write-Host ("Adding Redirect URI Origin: {0}" -f $query)
	DbRunQueryNoResult $query
}

function UpdateRedirectURI($client, $domain, $protocol)
{
	$query = ("
		UPDATE [Identity].[dbo].[ClientRedirectUris]
		SET URI = '{0}{1}'
		WHERE Client_Id = (
			SELECT [Id] 
			FROM [Identity].[dbo].[Clients]
			WHERE [ClientId] = '{2}'
		)" -f $protocol, $domain, $client)

	#Write-Host ("Updating Redirect URI Origin: {0}" -f $query)

	DbRunQueryNoResult $query
}

function UpdateSpecificRedirectURI($client, $domain, $protocol)
{
	$query = ("
		UPDATE [Identity].[dbo].[ClientRedirectUris]
		SET URI = '{0}{1}'
		WHERE Client_Id = (
			SELECT [Id] 
			FROM [Identity].[dbo].[Clients]
			WHERE [ClientId] = '{2}')
        AND [URI] LIKE '%{1}'
		" -f $protocol, $domain, $client)

	#Write-Host ("Updating Redirect URI Origin: {0}" -f $query)

	DbRunQueryNoResult $query
}

function CreatePostLogoutRedirectURI($client, $domain, $protocol)
{
	$query = ("INSERT INTO [Identity].[dbo].[ClientPostLogoutRedirectUris] (URI, Client_Id) 
	VALUES (
		'{0}{1}', 
		(
			SELECT [Id] 
			FROM [Identity].[dbo].[Clients] 
			WHERE [ClientId] = '{2}'
		)
	)" -f $protocol, $domain, $client)
	#Write-Host ("Adding PostLogout Redirect URI Origin: {0}" -f $query)
	DbRunQueryNoResult $query
}

function UpdatePostLogoutRedirectURI($client, $domain, $protocol)
{
	$query = ("
		UPDATE [Identity].[dbo].[ClientPostLogoutRedirectUris]
		SET URI = '{0}{1}'
		WHERE Client_Id = (
			SELECT [Id] 
			FROM [Identity].[dbo].[Clients]
			WHERE [ClientId] = '{2}'
		)" -f $protocol, $domain, $client)

	#Write-Host ("Updating PostLogout Redirect URI Origin: {0}" -f $query)

	DbRunQueryNoResult $query
}

function UpdateSpecificPostLogoutRedirectURI($client, $domain, $protocol)
{
	$query = ("
		UPDATE [Identity].[dbo].[ClientPostLogoutRedirectUris]
		SET URI = '{0}{1}'
		WHERE Client_Id = (
			SELECT [Id] 
			FROM [Identity].[dbo].[Clients]
			WHERE [ClientId] = '{2}')
        AND [URI] LIKE '%{1}'
		" -f $protocol, $domain, $client)

	#Write-Host ("Updating PostLogout Redirect URI Origin: {0}" -f $query)

	DbRunQueryNoResult $query
}

function SSLAlwaysHttps
{

	if($ConfirmitSiteSSLEnabled -eq $true) {
		return "https://"
	} else {
		return "http://"
	}
}

function ConfigureCORSOrigin($client, $origin)
{
	$protocol = SSLAlwaysHttps
    $origin = $protocol + $origin
	$CORSOrigin = GetCORSOrigin $client

	$Partial = [System.UriPartial]'Authority'
	$CORSValidatedOrigin = ([System.Uri]$origin).GetLeftPart($Partial)

	if ([string]::IsNullOrEmpty($CORSOrigin))
	{
		#no CORS Origin set. create record
		Write-Host "No existing origin found. Creating new record with origin $origin"
		CreateCORSOrigin $client $CORSValidatedOrigin
	} elseif ($CORSOrigin -ne $CORSValidatedOrigin) {
		#update record
		Write-Host "Updating existing origin to $origin"
		UpdateCORSOrigin $client $CORSValidatedOrigin
	} else {
        Write-Host("CORS Origin is already up to date")
    }
}

function ConfigureMultiCORSOrigin($client, $origin)
{
	$protocol = SSLAlwaysHttps
    $fullorigin = $protocol + $origin
	$domain = ([System.Uri]$fullorigin).Host
	$CORSOrigin = GetSpecificCORSOrigin $client $domain
		
	$Partial = [System.UriPartial]'Authority'
	$CORSValidatedOrigin = ([System.Uri]$fullorigin).GetLeftPart($Partial)
	
	if ([string]::IsNullOrEmpty($CORSOrigin))
	{
		#no CORS Origin set. create record
		Write-Host "No existing origin found. Creating new record with origin $origin"
		CreateCORSOrigin $client $CORSValidatedOrigin
	} elseif ($CORSOrigin -ne $CORSValidatedOrigin) {
		#update record
		Write-Host ("Updating existing origin for domain {1} to {0}{1}" -f $protocol, $origin)
		UpdateSpecificCORSOrigin $client $domain $protocol
	} else {
        Write-Host("CORS Origin is already up to date")
    }
}

function ConfigureClientRedirectURI($client, $domain)
{
	$protocol = SSLAlwaysHttps
	$redirectURI = GetRedirectURI $client
	if ([string]::IsNullOrEmpty($redirectURI))
	{
		#no Redirect URI. create record
		Write-Host ("No existing redirect URI found. Creating new record for domain {0}{1}" -f $protocol, $domain)
		CreateRedirectURI $client $domain $protocol
	} elseif ($redirectURI -ne ($protocol + $domain)) {
		#update record
		Write-Host ("Updating existing redirect URI to {0}{1}" -f $protocol, $domain)
		UpdateRedirectURI $client $domain $protocol
	} else {
        Write-Host("Redirect URI is already up to date")
    }
}

function ConfigureClientMultiRedirectURI($client, $domain)
{
	$protocol = SSLAlwaysHttps
	$redirectURI = GetSpecificRedirectURI $client $domain
	if ([string]::IsNullOrEmpty($redirectURI))
	{
		#no Redirect URI for this domain. create record
		Write-Host "No existing redirect URI found for domain $domain. Creating new record."
		CreateRedirectURI $client $domain $protocol
	} elseif ($redirectURI -ne ($protocol + $domain)) {
		#update record
		Write-Host ("Updating existing redirect URI for domain {1} to {0}{1}" -f $protocol, $domain)
		UpdateSpecificRedirectURI $client $domain $protocol
	} else {
        Write-Host("Redirect URI for domain $domain is already up to date")
    }
}

function ConfigureClientPostLogoutRedirectURI($client, $domain)
{
	$protocol = SSLAlwaysHttps
	$redirectURI = GetPostLogoutRedirectURI $client
	if ([string]::IsNullOrEmpty($redirectURI))
	{
		#no PostLogout Redirect URI. create record
		Write-Host ("No existing PostLogout redirect URI found. Creating new record for domain {0}{1}" -f $protocol, $domain)
		CreatePostLogoutRedirectURI $client $domain $protocol
	} elseif ($redirectURI -ne ($protocol + $domain)) {
		#update record
		Write-Host ("Updating existing PostLogout redirect URI to {0}{1}" -f $protocol, $domain)
		UpdatePostLogoutRedirectURI $client $domain $protocol
	} else {
        Write-Host("PostLogout Redirect URI is already up to date")
    }
}

function ConfigureClientMultiPostLogoutRedirectURI($client, $domain)
{
	$protocol = SSLAlwaysHttps
	$redirectURI = GetSpecificPostLogoutRedirectURI $client $domain
	if ([string]::IsNullOrEmpty($redirectURI))
	{
		#no PostLogout Redirect URI for this domain. create record
		Write-Host "No existing PostLogout redirect URI found for domain $domain. Creating new record."
		CreatePostLogoutRedirectURI $client $domain $protocol
	} elseif ($redirectURI -ne ($protocol + $domain)) {
		#update record
		Write-Host ("Updating existing PostLogout redirect URI for domain {1} to {0}{1}" -f $protocol, $domain)
		UpdateSpecificPostLogoutRedirectURI $client $domain $protocol
	} else {
        Write-Host("PostLogout Redirect URI for domain $domain is already up to date")
    }
}

function ConfigureLogoutURI($client, $domain)
{
	$protocol = SSLAlwaysHttps
	$query = ("
		UPDATE [Identity].[dbo].[Clients]
		SET LogoutURI = '{0}{1}'
		WHERE ClientId = '{2}'
	" -f $protocol, $domain, $client)

	#Write-Host ("Updating Logout URI: {0}" -f $query)
    Write-Host ("Setting LogoutURI to: {0}{1}" -f $protocol, $domain)
	DbRunQueryNoResult $query

}

function ConfigureClientCustomDomains($client, $customDomainsVariable, $path)
{
	if($customDomainsVariable)
	{
		$list = $customDomainsVariable.Split(',')
		foreach($item in $list)
		{
			$customdomain = $item.Trim()
			if($customdomain)
			{
				ConfigureMultiCORSOrigin $client $customdomain
				ConfigureClientMultiRedirectURI $client ("{0}/{1}" -f $customdomain, $path)
				ConfigureClientMultiPostLogoutRedirectURI $client ("{0}/{1}" -f $customdomain, $path)
			}
		}
	}
}


