using System;
using System.Xml;
using Mono.Addins;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	//not possible - why? [Extension("/MonoDevelop/ProjectModel/ProjectBindings")]
	public class AddinProjectBinding : IProjectBinding
	{
		public Project CreateProject (ProjectCreateInformation info, XmlElement projectOptions)
		{
			string lang = projectOptions.GetAttribute ("language");
			return new AddinProject (lang, info, projectOptions);
		}

		public Project CreateSingleFileProject (string sourceFile)
		{
			throw new InvalidOperationException ();
		}

		public bool CanCreateSingleFileProject (string sourceFile)
		{
			return false;
		}

		public string Name {
			get { return "Addin"; }
		}
	}
}
