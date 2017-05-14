using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"AddinMaker",
	Namespace = "MonoDevelop",
	Version = "1.3.7",
	Url = "http://github.com/mhutch/MonoDevelop.AddinMaker"
)]

[assembly:AddinName ("AddinMaker")]
[assembly:AddinCategory ("Extension Development")]
[assembly:AddinDescription ("Makes it easy to create and edit IDE extensions")]
[assembly:AddinAuthor ("Mikayla Hutchinson")]
