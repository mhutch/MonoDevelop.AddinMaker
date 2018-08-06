using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Mono.Addins;

using MonoDevelop.Core;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	[ExportProjectModelExtension, AppliesTo ("AddinMaker")]
	class AddinProjectFlavor : DotNetProjectExtension
	{
		[ItemProperty("IsAddin", DefaultValue=true)]
		bool isAddin = true;

		AddinReferenceCollection addinReferences;

		public bool IsAddin {
			get { return isAddin; }
		}

		protected override void Initialize ()
		{
			base.Initialize ();

			addinReferences = new AddinReferenceCollection (this);
			Project.Items.Bind (addinReferences);

			//TODO: load the actual addin registry referenced from the project file
			AddinRegistry = AddinManager.Registry;
		}

		public AddinReferenceCollection AddinReferences {
			get { return addinReferences; }
		}

		protected override DotNetProjectFlags OnGetDotNetProjectFlags ()
		{
			return base.OnGetDotNetProjectFlags () | DotNetProjectFlags.IsLibrary;
		}

		protected override bool OnGetSupportsFramework (TargetFramework framework)
		{
			return framework.Id.Identifier == TargetFrameworkMoniker.NET_4_5.Identifier;
		}

		protected override void OnGetTypeTags (HashSet<string> types)
		{
			types.Add ("Extension");
			base.OnGetTypeTags (types);
		}

		protected override SolutionItemConfiguration OnCreateConfiguration (string id, ConfigurationKind kind)
		{
			var cfg = new AddinProjectConfiguration (id);
			cfg.CopyFrom (base.OnCreateConfiguration (id, kind));
			return cfg;
		}

		public IEnumerable<Addin> GetReferencedAddins ()
		{
			yield return AddinRegistry.GetAddin ("MonoDevelop.Core");
			yield return AddinRegistry.GetAddin ("MonoDevelop.Ide");
			foreach (var ar in Project.Items.OfType<AddinReference> ()) {
				yield return AddinRegistry.GetAddin (ar.Include);
			}
		}

		public AddinRegistry AddinRegistry { get; private set; }

		protected override ExecutionCommand OnCreateExecutionCommand (ConfigurationSelector configSel, DotNetProjectConfiguration configuration, ProjectRunConfiguration runConfiguration)
		{
			var cmd = (DotNetExecutionCommand) base.OnCreateExecutionCommand (configSel, configuration, runConfiguration);
			cmd.Command = GetLaunchAssembly ();
			cmd.Arguments = "--no-redirect";
			cmd.EnvironmentVariables["MONODEVELOP_DEV_ADDINS"] = Project.GetOutputFileName (configSel).ParentDirectory;
			cmd.EnvironmentVariables ["MONODEVELOP_CONSOLE_LOG_LEVEL"] = "All";
			return cmd;
		}

		string GetLaunchAssembly()
		{
			FilePath binDir = Project.ProjectProperties.GetValue<string> ("MDBinDir");
			if (binDir != null) {
				var exe = binDir.Combine ("VisualStudio.exe");
				if (File.Exists (exe))
					return exe;
				exe = binDir.Combine ("MonoDevelop.exe");
			}
			return Assembly.GetEntryAssembly ().Location;
		}

		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configuration, SolutionItemRunConfiguration runConfiguration)
		{
			return IsAddin;
		}

		protected override ProjectFeatures OnGetSupportedFeatures ()
		{
			var features = base.OnGetSupportedFeatures ();
			if (IsAddin) {
				features |= ProjectFeatures.Execute;
			}
			return features;
		}

		protected override IList<string> OnGetCommonBuildActions ()
		{
			var list = new List<string> (base.OnGetCommonBuildActions ()) {
				"AddinFile"
			};
			return list;
		}
	}

	class AddinReferenceEventArgs : EventArgsChain<AddinReferenceEventInfo>
	{
		public void AddInfo (AddinProjectFlavor project, AddinReference reference)
		{
			Add (new AddinReferenceEventInfo (project, reference));
		}
	}

	class AddinReferenceEventInfo
	{
		public AddinReference Reference { get; private set; }
		public AddinProjectFlavor Project { get; private set; }

		public AddinReferenceEventInfo (AddinProjectFlavor project, AddinReference reference)
		{
			Reference = reference;
			Project = project;
		}
	}
}
