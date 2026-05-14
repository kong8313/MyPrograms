var repoName = "cati";
var appNameShort = "aws-connect-dialer-proxy";
var healthzReadyPath = "healthz/ready";

#load "nuget:?package=Confirmit.Cake.DockerBuild&version=[18.35.0,19.0.0)"

templateVersion = "12.7.0";

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
Task("BeforeDeleteHelmChart");

//Not used for MicroService Api
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

// Add dependent tasks that will run after all other tasks, but before the Promote task
// This is the place to add additional tasks that needs to run to validate the build before it is promoted to SaaS
// The promote to SaaS is still only done if --promote=true and the version number is not a pre-release version number
Task("RunAdditionalTasksBeforePromote");

// Default task to run. You can optionally add tasks before and after the IISApp task
Task("Default")
    .IsDependentOn("IISApp");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
