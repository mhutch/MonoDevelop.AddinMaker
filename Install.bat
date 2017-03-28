@echo off
setlocal enableextensions enabledelayedexpansion

pushd %~dp0

rem try to find MSBuild in VS2017
rem the "correct" way is to use a COM API. not easy here.

FOR %%E in (Enterprise, Professional, Community) DO (
	set "MSBUILD_EXE=%ProgramFiles(x86)%\Microsoft Visual Studio\2017\%%E\MSBuild\15.0\Bin\MSBuild.exe"
	if exist "!MSBUILD_EXE!" goto :build
)

echo Could not find MSBuild 15
popd
exit /b 1

:build

set "CONFIG=Release"
set "PLATFORM=Any CPU"

"%MSBUILD_EXE%" /target:Restore "/p:Configuration=%CONFIG%" "/p:Platform=%PLATFORM%" %* || goto :error
"%MSBUILD_EXE%" /m "/p:Configuration=%CONFIG%" "/p:Platform=%PLATFORM%" /p:InstallAddin=True %* || goto :error

goto :exit

:error

for %%x in (%CMDCMDLINE%) do if /i "%%~x" == "/c" pause
popd
exit /b %ERRORLEVEL%

:exit
popd
