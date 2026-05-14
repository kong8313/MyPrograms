# Confirmit.CATI.Backend

## Cake build used to add Docker and Helm support to legacy .NET Framework Agents

The old psake build is there as before. Can be changed to Cake later on.
The Cake build is designed to run after the psake build (*build.ps1*) and will the the output (*nupkg*) and add that to a aspnet 4.8 Windows docker image. It will also create a Helm chart for deploying this application to Kubernetes.

Allow the `initrepo.cmd` to be executed after the template has been applied to the existing repo.

Run `create_cake_build.ps1` and login as a user that has permission to create a TeamCity build. This will create a new TeamCity build configuration that will run after the original TeamCity build and use the NuPkg of the rest service as an artifact dependency.

## Required manual TeamCity changes

To complete the Cake build created by `create_cake_build.ps1` in TeamCity do the following

* Assign TeamCity project to Docker Agent pool

## Required changes to the application

* Update Confirmit.Configuration to latest version

## Local builds

Use customVersion in Cake build to specify the version of the NuPkg produced by the psake build

```PowerShell
build.ps1
dotnet-cake --customVersion=2.0.0 --versionSource=Custom
```

## Run application as docker container(not in k8s)

```PowerShell
.\dockerrun.ps1 -version 2.0.0
```

## To update Confirmit.CATI.Backend when there is a new version of the template

1. Make sure you have committed all you changes in the repo. There should NOT be any pending changes

2. Run the update_from_template script from the Confirmit.CATI.Backend\k8s directory. This will install the latest version of the MicroService template and update Confirmit.CATI.Backend with the latest changes
  `.\update_from_template.ps1`

3. Review changes with your favorite git tool and merge in changes changes that you want to keep. Note that the update will overwrite files that are part of the template so make sure to review the changes so that you don't loose any of your local changes. Commit when you are satisfied with the changes

4. If there was a change in the PowerShell script **create_cake_build.ps1** when you did the update then you will need to run this script again to make sure the TeamCity build is updated with any required changes
