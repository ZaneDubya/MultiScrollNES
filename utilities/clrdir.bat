set "lastdir=%cd%"
cd /d %1
del *.* /q
for /F "delims=" %%i in ('dir /b') do (rmdir "%%i" /s/q || del "%%i" /s/q)
cd %lastdir%
goto :eof