param(
    [parameter(Mandatory=$true)] [string]$profileDirectory,
    [parameter(Mandatory=$true)] [string]$username,
    [parameter(Mandatory=$true)] [string]$password,
    [parameter(Mandatory=$true)] [string]$email
)

function Create-NPMConfigFile([string] $directory, [string]$username, [string]$password, [string]$email)
{    
    $auth = 'Basic ' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$($username):$($password)"));
    $response = invoke-webrequest -uri "http://artifactory.firmglobal.com/artifactory/api/npm/auth" -Headers @{ Authorization = $auth } -UseBasicParsing
    
    $npmrcRegistryUrl = "http://artifactory.firmglobal.com/artifactory/api/npm/npm-confirmit-virtual"
    $npmrcRegistry = "registry=$npmrcRegistryUrl"
    $npmrcPath = "$directory\.npmrc"
    new-item -path $npmrcPath -type file -force | Out-Null
    add-content $npmrcPath $response.Content
    add-content $npmrcPath $npmrcRegistry
	add-content $npmrcPath "progress=false"

    write-host "NPM configuration written to $npmrcPath"
}

Create-NPMConfigFile $profileDirectory $username $password $email