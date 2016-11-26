using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"AddinMaker",
	Namespace = "MonoDevelop",
	Version = "1.3.3",
	Url = "http://github.com/mhutch/MonoDevelop.AddinMaker"
)]

[assembly:AddinName ("Addin Maker")]
[assembly:AddinCategory ("Addin Development")]
[assembly:AddinDescription ("Makes it easy to create and edit addins")]
[assembly:AddinAuthor ("Mikayla Hutchinson")]
