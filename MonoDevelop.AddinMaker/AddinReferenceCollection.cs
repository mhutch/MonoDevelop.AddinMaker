using System;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	[Serializable]
	class AddinReferenceCollection : ProjectItemCollection<AddinReference>
	{
		public AddinProjectFlavor Parent { get; private set; }

		public AddinReferenceCollection (AddinProjectFlavor parent)
		{
			Parent = parent;
		}
	}
}