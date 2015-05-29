using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;

namespace MonoDevelop.AddinMaker
{
	[DataItem("AddinReference")]
	class AddinReference : ProjectItem
	{
		[ItemProperty ("Include")]
		public string Id { get; set; }

		[ItemProperty]
		public string Version { get; set; }

		public AddinProjectFlavor OwnerProject { get; internal set; }
	}
}