#!/bin/sh

## CAUTION HACK

rm *.nupkg
nuget pack *.nuspec
nuget push *.nupkg
