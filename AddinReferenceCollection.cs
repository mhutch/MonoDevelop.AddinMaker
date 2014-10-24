using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	class AddinReferenceCollection : ProjectItemCollection<AddinReference>
	{
		public AddinProject Project { get; private set; }

		internal AddinReferenceCollection (AddinProject project)
		{
			this.Project = project;
		}
	}
}