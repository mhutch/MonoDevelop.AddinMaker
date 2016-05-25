using System.Collections.Specialized;
using Mono.Addins.Description;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinFilesFolder
	{
		public ModuleDescription Module { get; private set; }

		public AddinFilesFolder (ModuleDescription module)
		{
			this.Module = module;
		}
	}
}
