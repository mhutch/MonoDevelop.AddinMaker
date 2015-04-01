using Mono.Addins.Description;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinAssembly
	{
		public ModuleDescription Module { get; private set; }
		public string Assembly { get; private set; }

		public AddinAssembly (ModuleDescription module, string assembly)
		{
			this.Module = module;
			this.Assembly = assembly;
		}
	}
}
