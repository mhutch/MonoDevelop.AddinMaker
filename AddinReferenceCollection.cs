using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	class AddinReferenceCollection : ProjectItemCollection<AddinReference>
	{
		public AddinProjectFlavor ProjectFlavor { get; private set; }

		internal AddinReferenceCollection (AddinProjectFlavor project)
		{
			this.ProjectFlavor = project;
		}
	}
}