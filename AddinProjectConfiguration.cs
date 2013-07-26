using System;
using MonoDevelop.Projects;
using System.Xml;

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
	}
}
