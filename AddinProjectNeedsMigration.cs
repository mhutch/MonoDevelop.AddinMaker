using MonoDevelop.Projects;
using MonoDevelop.Core;

namespace MonoDevelop.AddinMaker
{
	class AddinProjectNeedsMigration : DotNetProject
	{
		public AddinProjectNeedsMigration ()
		{
			Init ();
		}

		public AddinProjectNeedsMigration (string languageName) : base (languageName)
		{
			Init ();
		}

		void Init ()
		{
			throw new UserException (
				"This project must be migrated to the new format." +
				"For details, see https://mhut.ch/addinmaker/1.2"
			);
		}
	}
}

