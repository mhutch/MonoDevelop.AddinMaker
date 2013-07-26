using Mono.Addins;

[assembly:Addin (
	"AddinMaker",
	Namespace = "MonoDevelop",
	Version = "1.0",
	Category = "Addin Development",
	Url = "http://github.com/mhutch/MonoDevelop.AddinMaker"
)]

[assembly:AddinName ("Addin Maker")]
[assembly:AddinDescription ("Makes it easy to create and edit addins")]
[assembly:AddinAuthor ("Michael Hutchinson")]

[assembly:AddinDependency ("Core", "4.0")]
[assembly:AddinDependency ("Ide", "4.0")]