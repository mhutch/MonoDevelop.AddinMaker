pushd %~dp0

"%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MSBuild.exe" /p:InstallAddin=True %*
if not %ERRORLEVEL% == 0 goto Error

goto Exit

:Error
echo ERROR
pause
goto Exit

:Exit
popd
