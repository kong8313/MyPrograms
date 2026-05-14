rem The script copies log files for the current date to ftp
rem In order to copy log files for other date than current it should be run with parameter like as follows:
rem      copylogs_facts.bat 20120303

setlocal

set zipexe="C:\Program Files\7-Zip\7z.exe" a -tzip
set directories=C:\DialerLogs\;C:\BellviewTCI\LOGS\;C:\WINDOWS\system32\LogFiles\W3SVC1\;C:\Program Files\Dialogic\log\;C:\perflogs\

if "%1"=="" ( 
for /F "usebackq tokens=1,2,3 delims=/ " %%i IN (`date /t`) DO set todaypattern=%%k%%j%%i
rem @echo %todaypattern%
) else (
set todaypattern=%1
)

rem set todaypattern=20110809
@echo _%todaypattern%_

rem @echo %directories%

md .\%todaypattern%

for /F "tokens=1,2,3,4,5 delims=;" %%i IN ("%directories%") DO (
  for %%f in ("%%i*%todaypattern%*.*") do %zipexe% ".\%todaypattern%\%%~nf%%~xf.zip" "%%f" -ssw
  for %%f in ("%%j*%todaypattern%*.*") do %zipexe% ".\%todaypattern%\%%~nf%%~xf.zip" "%%f" -ssw
  for %%f in ("%%k*%todaypattern%*.*") do %zipexe% ".\%todaypattern%\%%~nf%%~xf.zip" "%%f" -ssw
  for %%f in ("%%l*%todaypattern%*.*") do %zipexe% ".\%todaypattern%\%%~nf%%~xf.zip" "%%f" -ssw
  for %%f in ("%%m*%todaypattern%*.*") do %zipexe% ".\%todaypattern%\%%~nf%%~xf.zip" "%%f" -ssw
)

for %%f in ("C:\WINDOWS\system32\LogFiles\HTTPERR\httperr*.log") do %zipexe% ".\%todaypattern%\%%~nf%%~xf.zip" "%%f" -ssw
for %%f in ("C:\BellviewTCI\BTRC\*.log") do %zipexe% ".\%todaypattern%\%%~nf%%~xf.zip" "%%f" -ssw

%zipexe% .\%todaypattern%\AppEvent.Evt.zip C:\WINDOWS\system32\config\AppEvent.Evt -ssw
%zipexe% .\%todaypattern%\SysEvent.Evt.zip C:\WINDOWS\system32\config\SysEvent.Evt -ssw
%zipexe% .\%todaypattern%\tciws165.evt.zip C:\WINDOWS\system32\config\tciws165.evt -ssw
%zipexe% .\%todaypattern%\web.config.zip "C:\Program Files\Confirmit CATI TCI Dialer Service\web.config" -ssw
%zipexe% .\%todaypattern%\App_Data.zip "C:\Program Files\Confirmit CATI TCI Dialer Service\App_Data" -ssw

echo open ftp.pulsetrain.com >ftpscript
echo user tci ti334 >>ftpscript
echo cd Quadrangle >>ftpscript
echo type binary >>ftpscript
echo mkdir %todaypattern% >>ftpscript
echo cd %todaypattern% >>ftpscript
for %%f in (".\%todaypattern%\*.zip") do echo mput %%f >>ftpscript
echo quit >>ftpscript

ftp -n -i -s:ftpscript

endlocal
