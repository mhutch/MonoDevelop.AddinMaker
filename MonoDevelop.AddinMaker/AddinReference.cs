using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;

namespace MonoDevelop.AddinMaker
{
	[DataItem]
	[ExportProjectItemType("AddinReference")]
	class AddinReference : ProjectItem
	{
		[ItemProperty]
		public string Version { get; set; }

		public AddinReference (string id)
		{
			Include = id;
		}

		AddinReference ()
		{
		}
	}
}