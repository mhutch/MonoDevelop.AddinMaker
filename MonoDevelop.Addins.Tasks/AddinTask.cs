using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Addins;

namespace MonoDevelop.Addins.Tasks
{
	public abstract class AddinTask : Task
	{
		[Required]
		public string ConfigDir { get; set; }

		[Required]
		public string AddinsDir { get; set; }

		[Required]
		public string DatabaseDir { get; set; }

		[Required]
		public string InstallRoot { get; set; }

		protected bool InitializeAddinRegistry ()
		{
			if (string.IsNullOrEmpty (ConfigDir))
				Log.LogError ("ConfigDir must be specified");

			if (string.IsNullOrEmpty (AddinsDir))
				Log.LogError ("AddinsDir must be specified");

			if (string.IsNullOrEmpty (DatabaseDir))
				Log.LogError ("DatabaseDir must be specified");

			if (string.IsNullOrEmpty (InstallRoot))
				Log.LogError ("InstallRoot must be specified");

			var binDir = Path.GetFullPath (Path.Combine (InstallRoot, "bin"));

			Registry = new AddinRegistry (
				ConfigDir,
				binDir,
				AddinsDir,
				DatabaseDir
			);

			return true;
		}

		protected AddinRegistry Registry { get; private set; }
	}
}

