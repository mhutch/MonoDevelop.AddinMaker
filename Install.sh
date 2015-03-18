#!/bin/sh

xbuild /t:Build /p:Configuration=Debug $* MonoDevelop.AddinMaker.sln || exit 1
xbuild /t:InstallAddin /p:Configuration=Debug $* MonoDevelop.AddinMaker.csproj || exit 1

