using System.Collections.Specialized;

namespace MonoDevelop.AddinMaker.AddinBrowser
{

	class AddinFile
	{
		public string File { get; private set; }

		public AddinFile (string file)
		{
			this.File = file;
		}
	}
}
