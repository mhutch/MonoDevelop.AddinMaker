using System;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.AddinMaker.AddinBrowser;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	class AddinReferenceNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType
		{
			get { return typeof (AddinReference); }
		}

		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return "AddinReference";
		}

		public override string ContextMenuAddinPath {
			get {
				return "/MonoDevelop/AddinMaker/ContextMenu/ProjectPad/AddinReference";
			}
		}

		public override Type CommandHandlerType
		{
			get { return typeof (AddinReferenceCommandHandler); }
		}

		public override void BuildNode (ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			var addin = (AddinReference)dataObject;

			nodeInfo.Label = GLib.Markup.EscapeText (addin.Include);

			//TODO: custom icon
			nodeInfo.Icon = Context.GetIcon ("md-reference-package");

			//TODO: get state, mark if unresolved
			//nodeInfo.StatusSeverity = TaskSeverity.Error;
			//nodeInfo.StatusMessage = GettextCatalog.GetString ("Could not resolve addin");
		}

		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}

		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			var addin = (AddinReference)dataObject;
			var dnp = (DotNetProject)addin.Project;
			foreach (var asm in dnp.References) {
				Console.WriteLine (asm);
			}
			var refs = dnp.GetReferencedAssemblies (IdeApp.Workspace.ActiveConfiguration);
			//FIXME: remove blocking call
			foreach (var r in refs.Result) {
				Console.WriteLine (r.FilePath);
			}
			base.BuildChildNodes (treeBuilder, dataObject);
		}

		class AddinReferenceCommandHandler : NodeCommandHandler
		{
			public override bool CanDeleteItem ()
			{
				return true;
			}

			[CommandUpdateHandler (Ide.Commands.EditCommands.Delete)]
			public void UpdateDelete (CommandInfo info)
			{
				info.Enabled = true;
				info.Text = GettextCatalog.GetString ("Remove");
			}

			[CommandHandler (Ide.Commands.EditCommands.Delete)]
			public override void DeleteItem ()
			{
				var addin = (AddinReference) CurrentNode.DataItem;
				var proj = addin.Project;
				addin.Project.Items.Remove (addin);
				Ide.IdeApp.ProjectOperations.SaveAsync (proj);
			}

			public override void ActivateItem ()
			{
				var addin = (AddinReference) CurrentNode.DataItem;
				var registry = addin.Project.GetFlavor<AddinProjectFlavor> ().AddinRegistry;
				var resolved = registry.GetAddin (addin.Include);
				if (resolved != null) {
					AddinBrowserViewContent.Open (registry, resolved);
				}
			}
		}
	}
}
