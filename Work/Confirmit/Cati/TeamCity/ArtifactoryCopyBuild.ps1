param(
	[parameter(Mandatory=$true)] [String] $BuildName,
	[parameter(Mandatory=$true)] [String] $BuildNumber,
	[parameter(Mandatory=$true)] [String] $TargetRepo,
	[parameter(Mandatory=$true)] [String] $ArtifactoryApiUrl,
	[parameter(Mandatory=$true)] [String] $UserName,
	[parameter(Mandatory=$true)] [String] $Password
)

$ErrorActionPreference = "Stop"

$authInfo = ("{0}:{1}" -f $UserName,$Password)
$authInfo = [System.Text.Encoding]::UTF8.GetBytes($authInfo)
$authInfo = [System.Convert]::ToBase64String($authInfo)

function Deploy-ArtifactByChecksum($artifactName, $checksumSha1)
{
	$url = $ArtifactoryApiUrl.trim('/').trim('/api') + "/" + $TargetRepo + "/" + $artifactName
    $headers = @{Authorization=("Basic {0}" -f $authInfo); "X-Checksum-Deploy"="true"; "X-Checksum-Sha1"="$checksumSha1"}
	$response = Invoke-RestMethod $url -Method PUT -headers $headers -Verbose
}

function Get-BuildInfo()
{
	$url = $ArtifactoryApiUrl.trim('/') + "/build/" + $BuildName + "/" + $BuildNumber
    $headers = @{Authorization=("Basic {0}" -f $authInfo)}
	$response = Invoke-RestMethod $url -Method GET -headers $headers -Verbose
	return $response.buildInfo
}

try
{
	$buildInfo = Get-BuildInfo
	foreach($module in $buildInfo.modules)	{
		foreach($artifact in $module.artifacts)	{
			Deploy-ArtifactByChecksum $artifact.name $artifact.sha1
		}
	}
}
catch [Exception] 
{ 
    $_.Exception.Message
	$_.Exception.Response
    Exit 1 
}