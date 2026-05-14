SC QUERY state= all |FINDSTR Confirmit.CATI.Backend |FINDSTR SERVICE_NAME > %TEMP%\srvs.txt
for /f "tokens=2" %%A in (%TEMP%\srvs.txt) do ( NET STOP %%A)