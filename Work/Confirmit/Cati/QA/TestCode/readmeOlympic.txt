This is readme file for Olympic tests.

Preparation part of tests is managed in Solution OlympicPrepareTest. This part includes:

* Authoring preparation steps (logon into Authoring,import survey definition, launch survey as CATI survey, add respondents)
* CATI preparation steps (open survey in CATI Supervisor, create and assign interviewer on survey)

Authoring preparation steps are created on the base of Authoring WS, CATI preparation steps are created on the base of CATI WS. Settings for Solution OlympicPrepareTest are located in OlympicData\OlympicData.xml. Settings for Authoring WS are in App.config (see project folder). Settings for CATI WS are in Confirmit.CATI.REST.SDK.Tests\App.config and Confirmit.CATI.REST.SDK\app.config. Survey definition for import is in OlympicData\p2000303551.xml - it's location should be marked in OlympicData\OlympicData.xml in <surveyLocation> section.

Next part - passing survey in CATI Interviewer Console by interviewer who was assigned in previous part. This is UI Coded Test which is managed in OlympicInterviewerConsole solution. Settings for this test are in OlympicData\OlympicData.xml. There You can see <interview> section for each interview. Meaning of all tags are described in OlympicInterviewerConsole\readme.txt -> Preparation of data part.

Attention! To run test from OlympicInterviewerConsole solution You need make preparation of your CATI Interviewer Console.

Preparing CATI Interviewer Console

If You have 64bit version of CATI Console then open file OlympicData\PrepareAndRunOlympic.bat and edit path in first line. 
Put there location of your "CatiInterviewerConsole.exe" file.
Save and close PrepareAndRunOlympic.bat file.


In OlympicData folder You can see PrepareAndRunOlympic.bat to get 32bit mode Console and start described tests one after other (preparation part, then Console test) from Visual Studio Development Prompt. 

