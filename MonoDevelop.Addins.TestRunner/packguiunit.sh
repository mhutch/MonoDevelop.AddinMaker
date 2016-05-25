#bin/sh

REVISION=ad28ae12928cce579ae2f9450291dd7648dad76b

pushd ../../guiunit
git reset --hard $REVISION || exit 1
git clean -xdf || exit 1
xbuild src/framework/GuiUnit_NET_4_5.csproj || exit 1
popd
rm MonoDevelop.Addins.GuiUnit*.nupkg
mkdir guiunit
cp ../../guiunit/bin/net_4_5/GuiUnit.exe guiunit
cp ../../guiunit/LICENSE.txt guiunit
mono nuget.exe pack MonoDevelop.Addins.GuiUnit.nuspec
rm -r guiunit
