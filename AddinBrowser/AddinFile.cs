using Mono.Addins.Description;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinFile
	{
		public ModuleDescription Module { get; private set; }
		public string File { get; private set; }

		public AddinFile (ModuleDescription module, string file)
		{
			this.Module = module;
			this.File = file;
		}
	}
}
