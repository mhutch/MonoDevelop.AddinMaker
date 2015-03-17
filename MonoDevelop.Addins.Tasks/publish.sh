#!/bin/sh

# download nuget on Windows
# it should always be present on Mono
nuget > /dev/null || curl https://api.nuget.org/downloads/nuget.exe -o nuget.exe

## CAUTION HACK

rm *.nupkg
nuget pack *.nuspec
nuget push *.nupkg
