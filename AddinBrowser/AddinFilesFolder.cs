using System.Collections.Specialized;

namespace MonoDevelop.AddinMaker.AddinBrowser
{
	class AddinFilesFolder
	{
		public StringCollection Files { get; private set; }

		public AddinFilesFolder (StringCollection files)
		{
			this.Files = files;
		}
	}
}
