using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace MonoDevelop.Addins.Tasks
{
	public class CollectOutputFiles : Task
	{
		[Required]
		public ITaskItem [] AddinFiles { get; set; }

		[Output]
		public ITaskItem [] AddinFilesWithLinkMetadata { get; set; }

		public override bool Execute ()
		{
			var items = new List<ITaskItem> ();

			foreach (var file in AddinFiles) {
				string path = file.GetMetadata ("FullPath");
				var link = GetLinkPath (file, path);
				var item = new TaskItem (path);
				item.SetMetadata ("Link", link);
				items.Add (item);
			}

			AddinFilesWithLinkMetadata = items.ToArray ();

			return true;
		}

		static string GetLinkPath (ITaskItem file, string path)
		{
			string link = file.GetMetadata ("Link");
			if (!string.IsNullOrEmpty (link)) {
				return link;
			}

			string projectDir;
			var definingProject = file.GetMetadata ("DefiningProjectFullPath");
			if (!string.IsNullOrEmpty (definingProject)) {
				projectDir = Path.GetDirectoryName (definingProject);
			} else {
				projectDir = Environment.CurrentDirectory;
			}

			projectDir = Path.GetFullPath (projectDir);
			if (projectDir [projectDir.Length - 1] != Path.DirectorySeparatorChar) {
				projectDir += Path.DirectorySeparatorChar;
			}

			if (path.StartsWith (projectDir, StringComparison.Ordinal)) {
				link = path.Substring (projectDir.Length);
			} else {
				link = Path.GetFileName (path);
			}

			return link;
		}
	}
}

