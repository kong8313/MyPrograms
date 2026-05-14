try
{
   $ErrorActionPreference = "Stop"

    Write-Host "Prepare Simulator.Client build results for deploy"
    $buildFolder = ".\build\"
    $jsFileName = Get-ChildItem -Path $buildFolder -Filter src.*.js -File -Name| Select-Object -first 1

    if(-not $jsFileName)
    {
        Write-Host "scr.*.js file name is not found"
        return
    }

    $jsFilePath = $buildFolder + $jsFileName
    $renamedFilePath = $buildFolder + "src.js"
    Write-Host "Rename $jsFilePath file"
    Move-Item -Path $jsFilePath -Destination $renamedFilePath  -Force

    $indexFilePath = $buildFolder + "index.html"
    Write-Host "Change $indexFilePath file"
    ((Get-Content -path $indexFilePath) -replace $jsFileName, 'src.js') | Set-Content -Path $indexFilePath

    $clientLocation = (Get-Item -Path ".\..\..").FullName
    $clientLocation = "$clientLocation\assemblies\Telephony\SimulatorDialerDriver\Client"
    Write-Host "Copy results to $clientLocation folder"
    if(!(Test-Path($clientLocation)))
    {
        New-Item -ItemType directory -Path "$clientLocation"
    }

    Copy-Item -Path "$buildFolder*" -Destination "$clientLocation" -Recurse
}
catch [Exception]
{ 
    $_.Exception.Message
    $_.Exception.StackTrace
    Exit -1
}