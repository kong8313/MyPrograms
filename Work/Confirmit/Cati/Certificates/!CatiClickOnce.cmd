del CatiClickOnce.cer 
del CatiClickOnce.pvk
del CatiClickOnce.pfx

rem Run to get help
rem makecert.exe -? > !makecert.basic.txt 2>>&1
rem makecert.exe -! > !makecert.advanced.txt 2>>&1
rem pvk2pfx.exe -? > !pvk2pfx.txt 2>>&1

rem password is: Cati99ClickOnce543Password8
rem See http://blogs.msdn.com/b/maximelamure/archive/2007/01/24/create-your-own-pfx-file-for-clickonce.aspx?PageIndex=2#comments
rem See http://social.microsoft.com/Forums/en/Offtopic/thread/c449501d-57a2-4860-ad26-43af4752be29

..\_3rdpart\Microsoft\makecert.exe -len 2048 -r -sv CatiClickOnce.pvk -n "CN=Cati Click Once" CatiClickOnce.cer
..\_3rdpart\Microsoft\pvk2pfx.exe -pvk CatiClickOnce.pvk -spc CatiClickOnce.cer -pfx CatiClickOnce.pfx -po Cati99ClickOnce543Password8 -pi Cati99ClickOnce543Password8