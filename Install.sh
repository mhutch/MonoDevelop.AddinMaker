#!/bin/sh

xbuild /t:Build /p:Configuration=Release MonoDevelop.AddinMaker.sln || exit 1
# xbuild /t:InstallAddin /p:Configuration=Release MonoDevelop.AddinMaker.csproj || exit 1

