using System.Collections.Specialized;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinAssembliesFolder
	{
		public StringCollection Assemblies { get; private set; }

		public AddinAssembliesFolder (StringCollection assemblies)
		{
			this.Assemblies = assemblies;
		}
	}
}
