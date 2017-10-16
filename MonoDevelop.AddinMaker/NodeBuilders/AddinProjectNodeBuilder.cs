using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;
using BF = System.Reflection.BindingFlags;

namespace MonoDevelop.AddinMaker
{
	class AddinProjectNodeBuilder: NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return dataType.FullName == "MonoDevelop.DotNetCore.NodeBuilders.DependenciesNode"
				|| typeof(DotNetProject).IsAssignableFrom (dataType);
		}

		public override Type CommandHandlerType {
			get { return typeof(AddinProjectCommandHandler); }
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			var project = GetProject (dataObject);
			if (project == null || !project.HasFlavor<AddinProjectFlavor> ()){
				return false;
			}

			var isSdkStyleProject = project.MSBuildProject.Sdk != null;

			//bind either to deps node for sdk type, or to project for legacy
			return !(dataObject is DotNetProject) || !isSdkStyleProject;
		}

		static DotNetProject GetProject (object dataObject)
		{
			return dataObject as DotNetProject
				?? dataObject
					.GetType ().GetProperty ("Project", BF.NonPublic | BF.Instance)
					?.GetValue (dataObject) as DotNetProject;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var flavor = GetProject (dataObject)?.GetFlavor<AddinProjectFlavor> ();
			if (flavor == null) {
				return;
			}
			var isSdkStyleProject = flavor.Project.MSBuildProject.Sdk != null;

			//bind either to deps node for sdk type, or to project for legacy
			if (!(dataObject is DotNetProject) || !isSdkStyleProject) {
				treeBuilder.AddChild (flavor.AddinReferences);
			}
		}

		class AddinProjectCommandHandler : NodeCommandHandler
		{
		}
	}
}
