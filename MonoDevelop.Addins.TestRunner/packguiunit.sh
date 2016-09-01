#bin/sh

REVISION=2670780396856f043ab5cea9ab856641f56de5ae
VERSION=0.1.1

pushd ../../guiunit
git reset --hard $REVISION || exit 1
git clean -xdf || exit 1
xbuild src/framework/GuiUnit_NET_4_5.csproj || exit 1
popd
rm MonoDevelop.Addins.GuiUnit*.nupkg
mkdir guiunit
cp ../../guiunit/bin/net_4_5/GuiUnit.exe guiunit
cp ../../guiunit/LICENSE.txt guiunit
mono nuget.exe pack MonoDevelop.Addins.GuiUnit.nuspec -Properties version=$VERSION
rm -r guiunit
