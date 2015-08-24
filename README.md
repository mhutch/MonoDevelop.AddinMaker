# MonoDevelop.AddinMaker

The Addin Maker makes it easy to create and maintain MonoDevelop and Xamarin Studio addins.

## Features

* Full support for creating, building, running and debugging addins from within MD/XS.
* MSBuild-based build system is extensible and allows building from the commandline.
* Code completion for addin manifests, file templates and project templates.
* Automatically handles referencing core MD/XS assemblies and assemblies from referenced addins.
* The `AddinFile` build action takes care of the details of bundling files with the addin.
* Multi-targeting support allows building against any MD/XS instance.

## Installation

Most users should install this from the MD/XS Addin Gallery. However, if you wish to contribute to the addin, you will need to build it from source. This may also be necessary if you wish to use it with some unreleased version of MonoDevelop or Xamarin Studio.

Because loading the Addin Maker in MD/XS requires itself to be installed, scripts are provided to build and install it from the commandline. Simply run `Install.bat` (Windows) or `make install` (Mac, Linux).

To build against a specific version, you can provide arguments, e.g.

    make install ARGS="/p:MDBinDir=../monodevelop/main/build/bin /p:MDProfileVersion=6.0"

## Migrating Existing Projects

To migrate existing projects to the Addin Maker:

1. Add `{86F6BF2A-E449-4B3E-813B-9ACC37E5545F}` to the project's flavor GUIDs i.e. for C#: `<ProjectTypeGuids>{86F6BF2A-E449-4B3E-813B-9ACC37E5545F};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</ProjectTypeGuids>`
2. Add a reference to the NuGet package `monodevelop.addins`
3. Remove all references you have to the assemblies included with MonoDevelop/Xamarin Studio.
4. Remove all addin dependency declarations from your manifests and assembly attributes.
5. Add Addin References for all of the dependencies you remvoed in step 4.
6. [OPTIONAL] For each file that is bundled with your addin, unset the "copy to output" flag, set the build action to `AddinFile`, and remove the corresponding `<Import>` from the `<Runtime>` element of your manifest.
