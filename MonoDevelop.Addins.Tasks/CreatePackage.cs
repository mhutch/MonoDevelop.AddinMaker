using System;
using Microsoft.Build.Framework;
using Mono.Addins.Setup;

namespace MonoDevelop.Addins.Tasks
{
	public class CreatePackage : AddinTask
	{
		[Required]
		public string OutputDir { get; set; }

		[Required]
		public string AddinFile { get; set; }

		[Output]
		public string PackageFile { get; set; }

		public override bool Execute ()
		{
			if (!InitializeAddinRegistry ())
				return false;

			string[] result;
			try {
				var service = new SetupService (Registry);
				result = service.BuildPackage (
					new LogProgressStatus (Log, 0),
					OutputDir,
					AddinFile
				);
			} catch (Exception ex) {
				Log.LogError ("Internal error: {0}", ex);
				return false;
			}

			if (Log.HasLoggedErrors)
				return false;

			if (result.Length != 1) {
				Log.LogError ("Unexpected number of packaging results: {0}", result.Length);
				return false;
			}

			PackageFile = result [0];

			Log.LogMessage (MessageImportance.Normal, "Created package: {0}", PackageFile);

			return true;
		}
	}
}