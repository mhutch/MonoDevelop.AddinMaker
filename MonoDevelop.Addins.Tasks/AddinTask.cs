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
		public string BinDir { get; set; }

		protected bool InitializeAddinRegistry ()
		{
			if (string.IsNullOrEmpty (ConfigDir))
				Log.LogError ("ConfigDir must be specified");

			if (string.IsNullOrEmpty (AddinsDir))
				Log.LogError ("AddinsDir must be specified");

			if (string.IsNullOrEmpty (DatabaseDir))
				Log.LogError ("DatabaseDir must be specified");

			if (string.IsNullOrEmpty (BinDir))
				Log.LogError ("BinDir must be specified");

			Registry = new AddinRegistry (
				ConfigDir,
				BinDir,
				AddinsDir,
				DatabaseDir
			);

			Log.LogMessage (MessageImportance.Normal, "Updating addin database at {0}", DatabaseDir);
			Registry.Update (new LogProgressStatus (Log, 2));

			return !Log.HasLoggedErrors;
		}

		protected AddinRegistry Registry { get; private set; }
	}
}

