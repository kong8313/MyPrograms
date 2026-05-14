[Reflection.Assembly]::LoadFile("$installLocation\Installation\CustomActionLibrary.dll")
[Reflection.Assembly]::LoadFile("$installLocation\Installation\BootstrapperLibrary.dll")

function RemoveAndLineFeed($text)
{
    return $text.TrimEnd("`r", "`n")
}

function CapitalizeTheFirstCharacter($text)
{
    if($text -eq $null)
    {
        return
    }

    return $text.Substring(0, 1).ToUpper() + $text.Substring(1).ToLower()
}

function WriteToConfirmitLog($confirmitDatabaseEngine, $text)
{
    Write-Host $text
    $confirmitDatabaseEngine.SaveEventToDatabase($text)
}

function InsertClientSecretInConfigFile()
{
    param(
        [string] $configFile = $(throw 'Error: No config file was found.'),
        [string] $clientId = $(throw 'Error: No clientId was given.'),
        [string] $generatorKey = $(throw 'Error: Variable Confirmit.Site.Identity.ClientKeyGeneratorSecret is not set.')
    )
	
	function CreateSecret($masterKey, $clientKey)
	{
		 $combined = "$($masterKey)$($clientKey)"
		 $secret = Hash $combined
		 return $secret
	}

	function Hash($textToHash)
	{
		$hasher = new-object System.Security.Cryptography.SHA256Managed
		$toHash = [System.Text.Encoding]::UTF8.GetBytes($textToHash)
		$hashByteArray = $hasher.ComputeHash($toHash)
		$hashedString = [Convert]::ToBase64String($hashByteArray)
		return $hashedString
	}
    
    $clientIdSettingKey = 'TrustedSubsystem.ClientId'
    $clientSecretSettingKey = 'TrustedSubsystem.ClientSecret'

	$secureSectionHandlerXml = '<section name="secureAppSettings" type="System.Configuration.NameValueSectionHandler,system, Version=4.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089" />'    
	$tempXmlDoc = new-object System.Xml.XmlDocument
	$tempXmlDoc.LoadXml($secureSectionHandlerXml)
	$doc = (Get-Content $configFile) -as [Xml]
	$secureSectionHandlerNode = $doc.ImportNode($tempXmlDoc.DocumentElement, $true)
    
	$secureSectionHandler = $doc.SelectSingleNode("/configuration/configSections/section[@name='secureAppSettings']")
   
	if($secureSectionHandler -eq $null)
	{
		Write-Host 'Appending secure section handler'
		$doc.SelectSingleNode("/configuration/configSections").AppendChild($secureSectionHandlerNode) | Out-Null
	}
       
	$secureAppSettingsNode = $doc.SelectSingleNode('/configuration/secureAppSettings')
   
	if($secureAppSettingsNode -eq $null)
	{
		Write-Host 'Appending secureAppSettings element'
		$secureAppSettingsNode = $doc.CreateElement('secureAppSettings');
		$doc.configuration.AppendChild($secureAppSettingsNode) | Out-Null
	}

	$clientIdSettings = $doc.configuration.secureAppSettings.add | where {$_.key -eq $clientIdSettingKey}
	foreach($node in $clientIdSettings)
	{
		$node.ParentNode.RemoveChild($node) | Out-Null
	}
	#add new element
	Write-Host 'Setting clientId'
	$addClientId = $doc.CreateElement('add')
	$addClientId.SetAttribute("key", $clientIdSettingKey) 
	$addClientId.SetAttribute("value", $clientId) 
	$secureAppSettingsNode.AppendChild($addClientId) | Out-Null

	$clientSecretSettings = $doc.configuration.secureAppSettings.add | where {$_.key -eq $clientSecretSettingKey}
	foreach($node in $clientSecretSettings)
	{
		$node.ParentNode.RemoveChild($node) | Out-Null
	}
	#add new element
	$clientSecret = CreateSecret $generatorKey $clientId
	Write-Host 'Setting clientSecret'
	$addClientSecret = $doc.CreateElement('add')
	$addClientSecret.SetAttribute("key", $clientSecretSettingKey) 
	$addClientSecret.SetAttribute("value", $clientSecret) 
	$secureAppSettingsNode.AppendChild($addClientSecret) | Out-Null
    
	$path = Resolve-Path $configFile
    Write-Host "Saving to: $($path)"
	$doc.Save($path)
}

function Run-SQLScriptFile($fileName, $connectionString)
{
    Write-Host "FilePath=$installLocation\Installation\$fileName.sql"

    $query = [IO.File]::ReadAllText("$installLocation\Installation\$fileName.sql")
    $conn=New-Object System.Data.SqlClient.SQLConnection
    $conn.ConnectionString=$connectionString
    $conn.Open()
    $cmd=New-Object system.Data.SqlClient.SqlCommand($query, $conn)
    $cmd.CommandTimeout = 300
    $cmd.ExecuteNonQuery()
}

function Add-CompatibilityLevelSQLAgentJob($masterConnectionString)
{
    Run-SQLScriptFile "CompLevelSystemStoredProcedure" $masterConnectionString
    Run-SQLScriptFile "CompLevelSystemAgentJob" $masterConnectionString
}

function Set-SiteConfigValue($confirmConnectionString, $configId, $value)
{
	if(!$value)
	{
		Write-Host "Site config value not set for $configId. Empty value is ignored"
		return
	}
	Write-Host "Setting site config value: $configId=$value..."
	$query = "UPDATE CfgConfig SET ConfigValue=@Value WHERE ConfigId=@ConfigId"
	$conn=New-Object System.Data.SqlClient.SQLConnection
	$conn.ConnectionString=$confirmConnectionString
	$conn.Open()
	$cmd=New-Object system.Data.SqlClient.SqlCommand($query,$conn)
	$cmd.Parameters.Add("@ConfigId", $configId) | Out-Null
	$cmd.Parameters.Add("@Value", $value) | Out-Null
	$cmd.ExecuteNonQuery() | Out-Null
	$conn.Close()
}