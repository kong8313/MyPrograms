function CloneRepo ($repoName)
{
	$rootLocation = (Get-Item -Path ".\..\..\..\").FullName
		
	$folderName = $repoName.Replace('-', '.')
	$localPath = "$rootLocation\$folderName"
	
	if(!(Test-Path -Path $localPath))
	{
		$repoPath = "https://gitlab.com/pgforsta/forsta/forsta-plus/horizons-applications/$repoName.git"
		Write-Host "Checkout $repoPath"		
		
		& git clone $repoPath $localPath
		if ($LASTEXITCODE -ne 0)
		{
			throw [System.Exception] "Cannot clone '$repoPath' to '$localPath'."
		}
	}
}

try
{    
    CloneRepo "confirmit-catiinterviewer-api"
    CloneRepo "confirmit-catiinterviewer-client"
	CloneRepo "confirmit-catisupervisor-api"
	CloneRepo "confirmit-catisupervisor-client"
     
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}
