using MonoDevelop.Projects;
using MonoDevelop.Core;

namespace MonoDevelop.AddinMaker
{
	class AddinProjectNeedsMigrationFlavor : DotNetProjectExtension
	{
		public AddinProjectNeedsMigrationFlavor ()
		{
			throw new UserException (
				"This project must be migrated to the new format." +
				"For details, see https://mhut.ch/addinmaker/1.2"
			);
		}
	}
}
