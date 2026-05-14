$ErrorActionPreference = 'stop'

$RepoName = "cati"
$BuildDependencyId = "ConfirmitCatiNew_Compile"
$NuPkgName = "Confirmit.CATI.GenericDialerSimulator"
$ConfirmitCakeWorkDir = (($PSScriptRoot -replace "\\","/") -replace (git rev-parse --show-toplevel),"").TrimStart("/") 
$MainCakeBuildName = "cati-dialer-simulator-k8s-v2"
$BuildAndPublishContainerImageWinCakeBuildNamePrefix = $MainCakeBuildName
$RestApiBaseUrl = "https://teamcity.firmglobal.com/httpAuth/app/rest"
$headers = @{}
$headers.Add("Accept","application/json")
$headers.Add("Origin", "https://teamcity.firmglobal.com")
$textHeaders = @{}
$textHeaders.Add("Accept","text/plain")
$textHeaders.Add("Origin", "https://teamcity.firmglobal.com")

function Disable-OldBuild($credential)
{
    $CakeBuildName = $RepoName + "-k8s-api"
    $dependenctConfig = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/id:$($BuildDependencyId)" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    $ProjectId = $dependenctConfig.projectId;
    $buildTypes = Invoke-RestMethod -Uri "$RestApiBaseUrl/projects/id:$ProjectId/buildTypes" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    $cakeBuildId = ($buildTypes.buildType | Where-Object {$_.name -eq $CakeBuildName}).id
    if($null -ne $cakeBuildId) {
        Write-Host "Pausing old build configuration" -ForegroundColor Cyan
        $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/paused" -Credential $credential -Method Put -UseBasicParsing -Headers $textHeaders -ContentType "text/plain" -Body "true"
    }
}

function Add-Build($credential, $CakeBuildTemplateId, $CakeBuildName, $agentReqOsName)
{
    $dependenctConfig = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/id:$($BuildDependencyId)" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    $ProjectId = $dependenctConfig.projectId;
    $buildTypes = Invoke-RestMethod -Uri "$RestApiBaseUrl/projects/id:$ProjectId/buildTypes" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    if($buildTypes.buildType.name -eq $CakeBuildName) {
        Write-Host "TeamCity Cake build configuration already exists" -ForegroundColor Cyan
    }
    else {
        $buildType = Invoke-RestMethod -Uri "$RestApiBaseUrl/projects/id:$ProjectId/buildTypes" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "text/plain" -Body $CakeBuildName
        Write-Host "TeamCity build configuration '$CakeBuildName' created" -ForegroundColor Green
        $template = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/id:$($BuildType.id)/template" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "text/plain" -Body $CakeBuildTemplateId
        Write-Host "TeamCity build configuration '$CakeBuildName' attached to template '$($template.name)'" -ForegroundColor Green
    }

    $buildTypes = Invoke-RestMethod -Uri "$RestApiBaseUrl/projects/id:$ProjectId/buildTypes" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    $cakeBuildId = ($buildTypes.buildType | Where-Object {$_.name -eq $CakeBuildName}).id
    $ConfirmitRepoNameJson = '{"name":"Confirmit.RepoName","value":"' + $RepoName+ '"}'
    $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/parameters/Confirmit.RepoName" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $ConfirmitRepoNameJson
    $ConfirmitCakeWorkDirJson = '{"name":"Confirmit.Cake.WorkDir","value":"' + $ConfirmitCakeWorkDir+ '"}'
    $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/parameters/Confirmit.Cake.WorkDir" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $ConfirmitCakeWorkDirJson
    $ConfirmitAgentOsNameJson = '{"name":"Confirmit.Agent.OS.Name","value":"' + $agentReqOsName+ '"}'
    $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/parameters/Confirmit.Agent.OS.Name" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $ConfirmitAgentOsNameJson

    $buildNumberPatternJson = '{"name":"buildNumberPattern","value":"%dep.'+$BuildDependencyId+'.build.number%"}'
    $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/settings/buildNumberPattern" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $buildNumberPatternJson

    return $cakeBuildId
}

function Add-BuildAndPublishContainerImageWinBuild($credential, $tagSuffix, $agentReqOsName)
{
    $CakeBuildTemplateId = "ConfirmitCatiNew_ConfirmitCakeDockerBuildBuildAndPublishContainerImageWinTemplate"
    $CakeBuildName = $BuildAndPublishContainerImageWinCakeBuildNamePrefix + $tagSuffix

    $cakeBuildId = Add-Build $credential $CakeBuildTemplateId $CakeBuildName $agentReqOsName

    $ConfirmitTagSuffixJson = '{"name":"Confirmit.TagSuffix","value":"' + $tagSuffix+ '"}'
    $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/parameters/Confirmit.TagSuffix" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $ConfirmitTagSuffixJson

    $snapshotDependencies = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/snapshot-dependencies" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    if($snapshotDependencies.count -eq 0) {
        Write-Host "Creating snapshot dependency..." -ForegroundColor Green
        $snapshotDependencyJson = '{"type":"snapshot_dependency","properties":{"count":6,"property":[{"name":"run-build-if-dependency-failed","value":"MAKE_FAILED_TO_START"},{"name":"run-build-if-dependency-failed-to-start","value":"MAKE_FAILED_TO_START"},{"name":"run-build-on-the-same-agent","value":"false"},{"name":"sync-revisions","value":"true"},{"name":"take-started-build-with-same-revisions","value":"true"},{"name":"take-successful-builds-only","value":"true"}]},"source-buildType":{"id":"'+$BuildDependencyId+'"}}'
        $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/snapshot-dependencies" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $snapshotDependencyJson
    }

    $artifactDependencies = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/artifact-dependencies" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    if($artifactDependencies.count -eq 0) {
        $artifactsDir = "artifacts/"
        if($ConfirmitCakeWorkDir) {
            $artifactsDir = "%Confirmit.Cake.WorkDir%/artifacts/"
        }
        Write-Host "Creating artifact dependency..." -ForegroundColor Green
        $artifactDependencyJson = '{"type":"artifact_dependency","properties":{"count":4,"property":[{"name":"cleanDestinationDirectory","value":"true"},{"name":"pathRules","value":"'+$NuPkgName+'.%build.number%.nupkg => '+$artifactsDir+'"},{"name":"revisionName","value":"sameChainOrLastFinished"},{"name":"revisionValue","value":"latest.sameChainOrLastFinished"}]},"source-buildType":{"id":"'+$BuildDependencyId+'"}}'
        $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/artifact-dependencies" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $artifactDependencyJson
        if("enabled: false" -eq "enabled: true") {
            $artifactDependencyJson = '{"type":"artifact_dependency","properties":{"count":4,"property":[{"name":"cleanDestinationDirectory","value":"true"},{"name":"pathRules","value":"'+$RepoName+'.%build.number%.nupkg!/Database => '+$artifactsDir+'Database/"},{"name":"revisionName","value":"sameChainOrLastFinished"},{"name":"revisionValue","value":"latest.sameChainOrLastFinished"}]},"source-buildType":{"id":"'+$BuildDependencyId+'"}}'
            $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/artifact-dependencies" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $artifactDependencyJson    
        }
    }

    return $cakeBuildId
}

function Add-MainBuild($credential, $agentReqOsName, $buildDepsIds)
{
    $CakeBuildTemplateId = "ConfirmitCatiNew_ConfirmitCakeDockerBuildWindowsUseManifestListTemplate"
    $CakeBuildName = $MainCakeBuildName

    $cakeBuildId = Add-Build $credential $CakeBuildTemplateId $CakeBuildName $agentReqOsName

    $triggers = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/triggers" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    if($triggers.count -eq 0) {
        Write-Host "Creating build trigger..." -ForegroundColor Green
        $triggerJson = '{"type":"buildDependencyTrigger","properties":{"count":3,"property":[{"name":"afterSuccessfulBuildOnly","value":"true"},{"name":"branchFilter","value":"+:*"},{"name":"dependsOn","value":"'+$BuildDependencyId+'"}]}}'
        $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/triggers" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $triggerJson
    }

    foreach($buildDepId in $buildDepsIds) {
        $depExists = $true
        try {$snapshotDep = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/snapshot-dependencies/$buildDepId" -Credential $credential -Method Get -UseBasicParsing -Headers $headers } catch {$depExists = $false}
        if($depExists -eq $false) {
            Write-Host "Creating snapshot dependency..." -ForegroundColor Green
            $snapshotDependencyJson = '{"type":"snapshot_dependency","properties":{"count":6,"property":[{"name":"run-build-if-dependency-failed","value":"MAKE_FAILED_TO_START"},{"name":"run-build-if-dependency-failed-to-start","value":"MAKE_FAILED_TO_START"},{"name":"run-build-on-the-same-agent","value":"false"},{"name":"sync-revisions","value":"true"},{"name":"take-started-build-with-same-revisions","value":"true"},{"name":"take-successful-builds-only","value":"true"}]},"source-buildType":{"id":"'+$buildDepId+'"}}'
            $result = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/snapshot-dependencies" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $snapshotDependencyJson
        } 
    }

    return $cakeBuildId
}

$credential = Get-Credential $env:username

$buildIds = @()

$buildIds += Add-BuildAndPublishContainerImageWinBuild $credential "-ltsc2019" "Windows Server 2019"
$buildIds += Add-BuildAndPublishContainerImageWinBuild $credential "-ltsc2022" "Windows Server 2022"
$mainBuildId = Add-MainBuild $credential "Windows Server 2022" $buildIds
Write-Host "https://teamcity.firmglobal.com/buildConfiguration/$mainBuildId"

Disable-OldBuild $credential