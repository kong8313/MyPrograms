# Computer-Assisted Telephone Interviewing (CATI) 
## Installation and getting started

Congratulation! You may have heard a lot about CATI and now decided to install it on your blade.  
These instruction supposed to help you with this process, but... be calm and patient :)

### A little history

CATI is a Confirmit application (not a part of the Horizons or Monolith) with a separate build and deployment.
It is possible to deploy CATI with Octopus, but here we will talk about development build.
Keep in mind that CATI cannot work without Horizons so you have to checkout and build Horizons first.

### Development build process

CATI now is 5 different repos but you have to clone only one of them. Other four needed repos will be cloned automatically.  
It is recommended to use "C:\Dev\" folder because it is excluded fron Symantec antivirus scan.

* Clone https://gitlab.com/pgforsta/forsta/forsta-plus/horizons/cati.git to "c:\dev\cati" folder
* Open cati\FinalBuilder\CATI.fbp8 in FinalBuilder application (version 8 now)
* Double click on cleanInstall target, select the first line (Check external repos), press RMB and select "Run From Current Action"

Note: Alternative way is to run "cati\MSBuild\_clean_install.bat" script.
If you are really lucky all steps will have "Completed" status and you can go ahed to "Getting started" section.  
If some steps are failed you can read error message, try to fix a problem and rerun the steps by selecting them and using "Run Selected Actions" menu item.  


### Getting started

Here are the instrctions how to start using CATI.
Open http://<servername>/home/ and make sure that "CATI Supervisor" icon is visible there.

Create a survey:
* Create a new survey and launch it with checked CATI channel enabled
* Create a new sample file with the following columns separated by tab: respid, TelephoneNumber, Email, RespondentName, TimezoneID
* Upload sample file from Respondents->Upload

Assign interviewer on a survey:
* Open CATI Supervisor using CATI->Surveys menu item
* Select your survey, press RMB and select "Open" item
* Go to Interviewers->All interviewers section
* Create a new interviewer. This user will be used in the CATI Console to pass interviews
* Open settings of the new interviewer, move to Assignments tab and Assign the user to your survey

Using CATI Interviewer Console:
* Open http://{bladeName}/catiinterviewer/
* Use crenentials specified when creating your interviewer to log in

You can also use old desktop interviewer console located in "cati\assemblies\CatiInterviewerConsole.exe"

### Details about FinalBuilder build

It performs the following actions:
* compiles all code
* installs Backend files to "c:\Program Files\Confirmit CATI Rel" folder
* creates backend windows services,
* prepares IIS apps for CATI Supervisor and CATI Conosle, 
* Deploys Dialer Simulator to "c:\Program Files\Confirmit CATI LTU Simulator (G) Dialer Web Service Rel" folder.
* Configures desktop Cati Interviewer Console.

Also it clones and builds the code from the follolwwing repositories:
* https://gitlab.com/pgforsta/forsta/forsta-plus/horizons-applications/confirmit-catiinterviewer-api.git to confirmit.catiinterviewer.api folder (the repo with backend of Browser Based Cati Console (BBCC) )
* https://gitlab.com/pgforsta/forsta/forsta-plus/horizons-applications/confirmit-catiinterviewer-client.git to confirmit.catiinterviewer.client folder (the repo with client of Browser Based Cati Console (BBCC) )
* https://gitlab.com/pgforsta/forsta/forsta-plus/horizons-applications/confirmit-catisupervisor-api.git to confirmit.catisupervisor.api folder (the repo with backend of new Supervisor)
* https://gitlab.com/pgforsta/forsta/forsta-plus/horizons-applications/confirmit-catisupervisor-client.git to confirmit.catisupervisor.client folder (the repo with client of new Supervisor)


### Configuration changes

CATI installation process automatically updates the following settings:
* in Authoring->Admin->System Configuration->Multimode section:
    * "MultimodeBaseURL" to http://{bladeName}/Supervisor.Rel
    * "MultimodeWebServiceURL" to http://{bladeName}/Rel/ManagementMultimodeInstance
    * "WebCATIConsoleDomain" to {bladeName}
* in Authoring->Admin->System Configuration->Miscellaneous section:
    * "ConfirmitURL" to http://{bladeName}/confirm
    * "DeployURL" to http://{bladeName}
    * "WebServiceBaseUrl" to http://{bladeName}
    * "RestApiURL" to http://{bladeName}/api

Note: do not remove CATI settings from Autoring to avoid strange errors in Autoring.

### Advanced configuration

Optionally you can view Cati\FinalBuilder\InstallationParameters.ps1 file and check that all parameters have correct values. By default they are correct but if you have to use different SQL server instance or any other 
parameters you can create InstallationParametersEx.ps1 file and override required parameters only (it is excluded from GIT).


## Development instructions

Here are instructions how to make change in the CATI codebase.

If you made changes in the ASP.NET Supervisor - it is enough to just build a project in Visual Studio to be able to test your changes.
If you made changes in the code running in the backend windows services - build a project in Visual Studio and run "update" target in FinalBuilder.


### Dialer support

Dialer is a special hardware thatin used in call centers to automatically call respondents. We have a dialer simulator that allows to test the system with dialer.
CleanInstall deploys the simulator but  it has to be enabled manually.
You can test that simulator is ready by opening "http://localhost/LTUSimulator(G)DialerService.Rel/" url in a browser.  
After opening this link you will be able to open Swagger (http://localhost:3838/catidialersimulator/swagger/ui/index#/) and Simulator Client (http://localhost:3838/catidialersimulator/app).  

To use dialer simulator in the system:
* Go to Supervisor->Resources->Dialer
* Add a new dialer
* Specify
    * "Type" as "Simulator (Open Dialer API)"
    * "DialType" as "Automatic"
    * "ID" as "1"
    * "Name" as "Open API Simulator"
    * "Service Address" as "http://{bladeName}/LTUSimulator(G)DialerService/DialerService.svc" (pay attention there shouldn't be ".Rel" in the IIS app name)
* Save changes
* Press RMB in the dialer, select "Connect and Activate" and confirm

If operation fails - look at the Multimode log (Authoring->Admin->Activity Logs->Multimode Log).
The dialer logs are located in c:\DialerLogs
Note: The "Prepare Simultor Step" in the CleanInstall breaks the activation so you have to connect and activate the dailer again.


### Database update process

CATI has a complex database with many tables, stored procedures, functions and so on. You can open solution Database.2012.sln with database to make changes but result of build of this solution will be used only in integration tests.  
To deploy your changes to clean install, test servers or production you have to prepare update script. It can be generated automatically by execution of "Cati\MSBuild\_generate_database_update_script.cmd" script.  
This script compares your current version of database with your changes with a server version of the database and generates update script in "Cati\Confirmit.CATI.Database.2012\Scripts\" folder.
Open this script after generation, check it and fix.  
Also you have to write explanation in "Cati\Confirmit.CATI.Database.2012\Scripts\ScriptsDefinitionFile.txt" file. 
Don't forget to run "DatabaseUpdate_CreateDatabasesFromDatabaseProjectScriptAndByUpdateScripts_DatabasesAreTheSame" test to be sure that databases from update scripts and from DB project are the same.


### T4 toolbox
If you made change to database - you may need to re-generate data access code (in Confirmit.CATI.Core project)
It was generated by T4 Toolbox extention. Unfortunatly the author of this extention stoped supporting it.
The latest version is the version for VS 2017. Other people have prepared a version for VS 2019 but can't commit it so we have only offline version of the extention.
You can find it here: "Cati\_3rdpart\T4Toolbox\T4Toolbox For VS2019.vsix". Install it manually to VS 2019 and you can regenerate T4 templates.
You should prepare ConfirmitCATIV15_Build database before you can start regeneration. Run "Cati\Confirmit.CATI.Database.2012\deploy_to_database.bat" to do that automatically.
If script finished fine and ConfirmitCATIV15_Build database has been made you can regenerate two sets of templates.
* Open "Cati\Confirmit.CATI.Core\SystemSettings\SettingsTemplate.tt" in VS and save it. This template regenerate code to work with system settings. You should save this file if you changed anithing in BvSystemSettings.
* Open "Cati\Confirmit.CATI.Core\DAL\Templates\DalTemplate.tt" in VS and save it. This template regenerate code to work with database objects. You should save this file if you changed anithing in Database.2012.sln.


### Integration tests

We usually run integration tests on the build server (it is possible to run build for your branch before mergign to master).
But if you want to run tests locally you can do that from VS or using the following script: "msbuild test.proj /t:Int" from "cati\MSBuild" folder.
This command run tests in parallel mode and save you at least one hour.


### Interviewer and Monitoring Consoles nuget packages

If you need to test Interviewer or Monitoring Consoles you can publish them and prepare nuget packages by the following command: msbuild TeamCityBuildConsole.proj from cati\CatiConsole\MSBuild\ folder.
