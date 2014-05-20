using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using MonoDevelop.Core;
using MonoDevelop.Core.Assemblies;
using MonoDevelop.Core.Execution;
using MonoDevelop.Projects;

namespace MonoDevelop.AddinMaker
{
	public class AddinProject : DotNetProject
	{
		public AddinProject ()
		{
			AddAddinsAssemblyContext ();
		}

		public AddinProject (string languageName) : base (languageName)
		{
			AddAddinsAssemblyContext ();
		}

		public AddinProject (string lang, ProjectCreateInformation info, XmlElement options)
			: base (lang, info, options)
		{
			AddAddinsAssemblyContext ();
		}

		public override SolutionItemConfiguration CreateConfiguration (string name)
		{
			var cfg = new AddinProjectConfiguration (name);
			cfg.CopyFrom (base.CreateConfiguration (name));
			return cfg;
		}

		protected override ExecutionCommand CreateExecutionCommand (ConfigurationSelector configSel, DotNetProjectConfiguration configuration)
		{
			var cmd = (DotNetExecutionCommand) base.CreateExecutionCommand (configSel, configuration);
			cmd.Command = Assembly.GetEntryAssembly ().Location;
			cmd.Arguments = "--no-redirect";
			cmd.EnvironmentVariables["MONODEVELOP_DEV_ADDINS"] = GetOutputFileName (configSel).ParentDirectory;
			cmd.EnvironmentVariables ["MONODEVELOP_CONSOLE_LOG_LEVEL"] = "All";
			return cmd;
		}

		protected override bool OnGetCanExecute (ExecutionContext context, ConfigurationSelector configuration)
		{
			return true;
		}

		public override bool IsLibraryBasedProjectType {
			get { return true; }
		}

		void AddAddinsAssemblyContext ()
		{
			var pctx = (DirectoryAssemblyContext) this.PrivateAssemblyContext;

			var dirs = new List<string> (pctx.Directories);
			dirs.AddRange (GetAddinsAssemblyDirs ());
			pctx.Directories = dirs;
		}

		static HashSet<string> addinAssemblyDirs;

		static HashSet<string> GetAddinsAssemblyDirs ()
		{
			if (addinAssemblyDirs != null)
				return addinAssemblyDirs;

			FilePath entryAssembly = Assembly.GetEntryAssembly ().Location;
			string binDir = entryAssembly.ParentDirectory;
			string libDir = ((FilePath)binDir).ParentDirectory.Combine ("AddIns");

			var addinDirs = Directory.GetFiles (libDir, "*.dll", SearchOption.AllDirectories)
				.Select (Path.GetDirectoryName);

			addinAssemblyDirs = new HashSet<string> (addinDirs);
			addinAssemblyDirs.Add (binDir);

			return addinAssemblyDirs;
		}
	}
}
