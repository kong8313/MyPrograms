function Get-CORSOrigin($client, $origin)
{
    $query = ("
        SELECT cco.Origin
        FROM [Identity].[dbo].[Clients] as c 
        LEFT JOIN [Identity].[dbo].[ClientCorsOrigins] as cco on cco.Client_Id = c.Id 
        WHERE ClientId = '{0}' AND Origin= '$origin'
    " -f $client)
    #Write-Host ("Getting CORS Origin: {0}" -f $query)

    $val = DbGetFirstFieldOrNull $query

    Write-Host ("Got CORSOrigin from database: {0}" -f $val)
    return $val
}

# We need to implement our version to add new Origin for existed ClientId instead of updating Origing for the existed ClientId
function Configure-CORSOrigin($client, $origin)
{    
    $protocol = SSLAlwaysHttps
    $origin = $protocol + $origin    

    $Partial = [System.UriPartial]'Authority'
    $CORSValidatedOrigin = ([System.Uri]$origin).GetLeftPart($Partial)
    
    $CORSOrigin = Get-CORSOrigin $client $CORSValidatedOrigin

    if ([string]::IsNullOrEmpty($CORSOrigin))
    {
        #no CORS Origin set. create record
        Write-Host "No existing origin found. Creating new record with origin $origin"
        CreateCORSOrigin $client $CORSValidatedOrigin
    } else {
        Write-Host("CORS Origin is already created")
    }
}

function Configure-Client {
    param(
        [string]$client,
        [string]$domain,
        [string]$hostName
    )
    ConfigureClientMultiRedirectURI $client $domain
    ConfigureClientMultiPostLogoutRedirectURI $client $domain
    if($hostName) {
        Configure-CORSOrigin $client $hostName
    }
}

try
{
    . ".\LoadParameters.ps1"
    . ".\Utils\ClientConfigUtil.ps1"

    $domainString = "MultimodeBaseURL"
    $domain = GetConfig $domainString

    if($domain)
    {
        $hostName = ([System.Uri]$domain).Host
        
        $client = "Confirmit.CATISupervisor.Client"
        Write-Host ("Configuring Client: {0}" -f $client)
        $catiDomain = ([System.Uri]$domain).Host + ([System.Uri]$domain).PathAndQuery #Strip off protocol, which will be decided by SSLEnabled setting.
        
        Configure-Client -client $client -domain ("{0}/*" -f $catiDomain) -hostName $hostName
        Configure-Client -client $client -domain ("{0}/catisupervisor/*" -f $hostName)
        Configure-Client -client $client -domain ("{0}:9000/catisupervisor/*" -f $hostName) -hostName ("{0}:9000" -f $hostName)
        
        $client = "Confirmit.Reviewer.Client"
        Write-Host ("Configuring Development Client: {0}" -f $client)
        Configure-Client -client $client -domain ("{0}:1234/reviewer/*" -f $hostName) -hostName ("{0}:1234" -f $hostName)

        $query = "update [Clients] set [Enabled] = 1 where [ClientId] = 'ro-client'"
        DbRunQueryNoResult $query
    }
    else
    {
        Write-Host ("Config for domain '{0}' not found." -f $domainString)
    }
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}