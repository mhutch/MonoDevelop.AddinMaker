#!/bin/sh

# download nuget on Windows
# it should always be present on Mono
nuget > /dev/null || curl https://api.nuget.org/downloads/nuget.exe -o nuget.exe

## CAUTION HACK

nuget push bin/Release/*.nupkg -Source https://www.nuget.org/api/v2/package
