pushd %~dp0

"%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MSBuild.exe" /t:Build /p:Configuration=Debug MonoDevelop.AddinMaker.sln
if not %ERRORLEVEL% == 0 goto Error

"%PROGRAMFILES(X86)%\MSBuild\14.0\Bin\MSBuild.exe" /t:InstallAddin /p:Configuration=Debug MonoDevelop.AddinMaker.csproj
if not %ERRORLEVEL% == 0 goto Error

goto Exit

:Error
echo ERROR
pause
goto Exit

:Exit
popd
