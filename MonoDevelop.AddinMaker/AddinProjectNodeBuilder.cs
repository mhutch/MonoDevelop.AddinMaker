using System;
using System.Linq;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	class AddinProjectNodeBuilder: NodeBuilderExtension
	{
		public override bool CanBuildNode (Type dataType)
		{
			return typeof(DotNetProject).IsAssignableFrom (dataType);
		}

		public override Type CommandHandlerType {
			get { return typeof(AddinProjectCommandHandler); }
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return ((DotNetProject)dataObject).HasFlavor<AddinProjectFlavor> ();
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var project = ((DotNetProject)dataObject).AsFlavor<AddinProjectFlavor> ();
			if (project != null) {
				treeBuilder.AddChild (project.AddinReferences);
			}
		}

		class AddinProjectCommandHandler : NodeCommandHandler
		{
		}
	}
}
