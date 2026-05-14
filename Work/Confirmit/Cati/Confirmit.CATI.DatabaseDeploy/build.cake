var repoName = "Confirmit.CATI.DatabaseDeploy";
var appNameShort = "cati-databasedeploy";
var healthzReadyPath = "healthz/ready";

#load "nuget:?package=Confirmit.Cake.DockerBuild&version=[18.*,19.0.0)"

templateVersion = "3.0.0-custom";

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

// Add dependent tasks that will run before the helm chart is installed
// This can be a good place to initialize the helmChartValuesYaml variable if it uses e.g. version information
Task("BeforeInstallHelmChart");

// Add dependent tasks that will run integration/end2end tests and before the Helm chart is deleted
// Use this if you need to do additional cleanup/uninstall tasks
Task("BeforeDeleteHelmChart");

//Not used for MicroService Agent
Task("RunTestsUsingIngress");

// Use this task to add cleanup steps for resources deployed as part of the build
// This task will be called as part of the Cleanup step in TeamCity and will always run. Even if build fails
// e.g. Delete additional Helm chart installed by the build
Task("CustomCleanup");
// Task("CustomCleanup")
//     .IsDependentOn("SetVersionInfo")
//     .WithCriteria(() => FileExists("./.kube/config"))
//     .Does(() => 
// {
//     try
//     {
//         DeleteHelmChart($"{GetHelmReleaseName()}-horizonsisa-api");
//     }
//     catch {}
// });

// Add dependent tasks that will run after the application is deployed with Octopus
// When the RunTestsUsingOctopusPackage task and dependencies has completed the application version will be rolled back in Octopus
Task("RunTestsUsingOctopusPackage");

// Add dependent tasks that will run after all other tasks, but before the PromoteClientApp task
// This is the place to add additional tasks that needs to run to validate the build before it is promoted to SaaS
// The promote to SaaS is still only done if --promote=true and the version number is not a pre-release version number
Task("RunAdditionalTasksBeforePromote");

Task("ZipSqlScripts")
    .Does(() => 
{
    var zipFile = "code/src/Confirmit.CATI.DatabaseUpdateLibraryCore/scripts.zip";
    if(FileExists(zipFile)) DeleteFile(zipFile);
    Zip("../Confirmit.CATI.Database.2012/Scripts", zipFile);
});

// Default task to run. You can optionally add tasks before and after the ClientApp task
Task("Default")
    .IsDependentOn("SetTemplateAgent")
    .IsDependentOn("ZipSqlScripts")
    .IsDependentOn("BuildApplicationContainerImage")
    .IsDependentOn("PublishApplicationContainerImage")
    .IsDependentOn("RunAdditionalTasksBeforePromote")
    .IsDependentOn("PromoteApplicationContainerImage");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
