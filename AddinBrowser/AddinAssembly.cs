using System.Collections.Specialized;

namespace MonoDevelop.AddinMaker.AddinBrowser
{

	class AddinAssembly
	{
		public string Assembly { get; private set; }

		public AddinAssembly (string assembly)
		{
			this.Assembly = assembly;
		}
	}
}
