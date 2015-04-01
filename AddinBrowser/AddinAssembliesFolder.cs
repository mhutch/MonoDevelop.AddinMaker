using System.Collections.Specialized;
using Mono.Addins.Description;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinAssembliesFolder
	{
		public ModuleDescription Module { get; private set; }

		public AddinAssembliesFolder (ModuleDescription module)
		{
			this.Module = module;
		}
	}
}
