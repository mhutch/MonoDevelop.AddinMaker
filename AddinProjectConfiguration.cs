using MonoDevelop.Projects;
using System.Collections.Generic;

namespace MonoDevelop.AddinMaker
{
	public class AddinProjectConfiguration : DotNetProjectConfiguration
	{
		public AddinProjectConfiguration ()
		{
		}

		public AddinProjectConfiguration (string name) : base (name)
		{
		}

		public override IEnumerable<string> GetDefineSymbols ()
		{
			foreach (var d in base.GetDefineSymbols ()) {
				yield return d;
			}

			var proj = ParentItem.GetFlavor<AddinProjectFlavor> ();

			//TODO: keep in sync with targets. eventually resolve from MSBuild
			var cv = proj.AddinRegistry.GetAddin ("MonoDevelop.Core").Description.CompatVersion;

			yield return "MD_" + cv.Replace ('.', '_');
		}
	}
}
