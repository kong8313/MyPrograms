$ErrorActionPreference = 'stop'

$RepoName = "CATI"

# $BuildDependencyId = "ConfirmitCati_FeatureBranch_4CatiBuildInstallations"
$BuildDependencyId = "ConfirmitCati_Master_4CatiBuildInstallations"
$NuPkgName = "Confirmit.CATI.Backend"
$ConfirmitCakeWorkDir = (($PSScriptRoot -replace "\\","/") -replace (git rev-parse --show-toplevel),"").TrimStart("/") 

function Add-Build($credential)
{
    $RestApiBaseUrl = "http://teamcity/httpAuth/app/rest"
    $CakeBuildTemplateId = "ConfirmitCati_ConfirmitCakeDockerBuildWindowsTemplate"
    $CakeBuildName = "Confirmit.CATI.Backend-dockerimage"
    $headers = @{}
    $headers.Add("Accept","application/json")
    $headers.Add("Origin", "http://teamcity")

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
    Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/parameters/Confirmit.RepoName" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $ConfirmitRepoNameJson
    $ConfirmitCakeWorkDirJson = '{"name":"Confirmit.Cake.WorkDir","value":"' + $ConfirmitCakeWorkDir+ '"}'
    Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/parameters/Confirmit.Cake.WorkDir" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $ConfirmitCakeWorkDirJson

    $buildNumberPatternJson = '{"name":"buildNumberPattern","value":"%dep.'+$BuildDependencyId+'.build.number%"}'
    Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/settings/buildNumberPattern" -Credential $credential -Method Put -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $buildNumberPatternJson

    $snapshotDependencies = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/snapshot-dependencies" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    if($snapshotDependencies.count -eq 0) {
        Write-Host "Creating snapshot dependency..." -ForegroundColor Green
        $snapshotDependencyJson = '{"type":"snapshot_dependency","properties":{"count":6,"property":[{"name":"run-build-if-dependency-failed","value":"MAKE_FAILED_TO_START"},{"name":"run-build-if-dependency-failed-to-start","value":"MAKE_FAILED_TO_START"},{"name":"run-build-on-the-same-agent","value":"false"},{"name":"sync-revisions","value":"true"},{"name":"take-started-build-with-same-revisions","value":"true"},{"name":"take-successful-builds-only","value":"true"}]},"source-buildType":{"id":"'+$BuildDependencyId+'"}}'
        Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/snapshot-dependencies" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $snapshotDependencyJson
    }

    $artifactDependencies = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/artifact-dependencies" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    if($artifactDependencies.count -eq 0) {
        $artifactsDir = "artifacts/"
        if($ConfirmitCakeWorkDir) {
            $artifactsDir = "%Confirmit.Cake.WorkDir%/artifacts/"
        }
        Write-Host "Creating artifact dependency..." -ForegroundColor Green
        $artifactDependencyJson = '{"type":"artifact_dependency","properties":{"count":4,"property":[{"name":"cleanDestinationDirectory","value":"true"},{"name":"pathRules","value":"'+$NuPkgName+'.%build.number%.nupkg => '+$artifactsDir+'"},{"name":"revisionName","value":"sameChainOrLastFinished"},{"name":"revisionValue","value":"latest.sameChainOrLastFinished"}]},"source-buildType":{"id":"'+$BuildDependencyId+'"}}'
        Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/artifact-dependencies" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $artifactDependencyJson
    }

    $triggers = Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/triggers" -Credential $credential -Method Get -UseBasicParsing -Headers $headers
    if($triggers.count -eq 0) {
        Write-Host "Creating build trigger..." -ForegroundColor Green
        $triggerJson = '{"type":"buildDependencyTrigger","properties":{"count":3,"property":[{"name":"afterSuccessfulBuildOnly","value":"true"},{"name":"branchFilter","value":"+:*"},{"name":"dependsOn","value":"'+$BuildDependencyId+'"}]}}'
        Invoke-RestMethod -Uri "$RestApiBaseUrl/buildTypes/$cakeBuildId/triggers" -Credential $credential -Method Post -UseBasicParsing -Headers $headers -ContentType "application/json" -Body $triggerJson
    }
}

$credential = Get-Credential $env:username

Add-Build $credential
