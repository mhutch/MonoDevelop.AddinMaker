using Mono.Addins;

[assembly:Addin (
	"${ProjectName}",
	Namespace = "${ProjectName}",
	Version = "1.0",
	Category = "Ide extensions"
	)]

[assembly:AddinName ("${ProjectName}")]
[assembly:AddinDescription ("${ProjectName}")]
[assembly:AddinAuthor ("${UserName}")]

[assembly:AddinDependency ("::MonoDevelop.Core", "4.0")]
[assembly:AddinDependency ("::MonoDevelop.Ide", "4.0")]