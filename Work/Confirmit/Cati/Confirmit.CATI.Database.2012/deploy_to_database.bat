call "..\MSBuild\_runVsCmd.cmd"
call MSBuild.exe Confirmit.CATI.Database\Confirmit.CATI.Database.sqlproj /t:Rebuild /p:configuration=Debug;SolutionDir=%CD%\..\
call ..\assemblies\Tools\DeployScript.exe /deploy
