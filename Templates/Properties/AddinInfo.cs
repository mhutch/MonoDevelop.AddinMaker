using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"${ProjectName}", 
	Namespace = "${ProjectName}",
	Version = "1.0"
)]

[assembly:AddinName ("${ProjectName}")]
[assembly:AddinCategory ("${ProjectName}")]
[assembly:AddinDescription ("${ProjectName}")]
[assembly:AddinAuthor ("${AuthorName}")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]