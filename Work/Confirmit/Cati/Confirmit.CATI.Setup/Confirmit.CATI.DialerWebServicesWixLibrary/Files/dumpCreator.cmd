@if "%_echo%"=="" echo off
setlocal
set ROOT_DUMP_FOLDER_PATH=<ROOT_DUMP_FOLDER_PATH>
set PROCDUMP_PATH="<PROCDUMP_PATH>"
set ADDITIONAL_PARAMETERS=<ADDITIONAL_PARAMETERS>
set CURRENT_VERSION="<CURRENT_VERSION>"
for /f "delims=" %%x in ('cscript /nologo get_date_time.vbs') do %%x
set TIME_FOR_FOLDER=%YEAR%_%MONTH%_%DAY% %HOUR%_%MIN%_%SEC%
set DUMP_FOLDER_PATH=%ROOT_DUMP_FOLDER_PATH%%TIME_FOR_FOLDER%
set LOG_FILE_PATH="%DUMP_FOLDER_PATH%\dump_log.txt"
set DUMP_FOLDER_PATH="%DUMP_FOLDER_PATH%"
set COMMAND=%PROCDUMP_PATH% %1 %DUMP_FOLDER_PATH% %ADDITIONAL_PARAMETERS%

mkdir %DUMP_FOLDER_PATH%

echo Current version=%CURRENT_VERSION%>>%LOG_FILE_PATH%
echo %DATE% %TIME%:%COMMAND% >>%LOG_FILE_PATH%
%COMMAND% >>%LOG_FILE_PATH%
echo ____________________________________________________________________________________________ >>%LOG_FILE_PATH%


Endlocal