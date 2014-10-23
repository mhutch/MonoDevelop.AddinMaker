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

		protected override void OnItemAdded (AddinReference item)
		{
			item.OwnerProject = Project;
			base.OnItemAdded (item);
		}

		protected override void OnItemRemoved (AddinReference item)
		{
			item.OwnerProject = null;
			base.OnItemRemoved (item);
		}
	}
}