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
		public string ManifestFile { get; set; }

		[Required]
		public ITaskItem[] AddinFiles { get; set; }

		[Output]
		public ITaskItem[] AddinFilesWithLinkMetadata { get; set; }

		public override bool Execute ()
		{
			var names = new List<string> ();
			var items = new List<ITaskItem> ();

			foreach (var file in AddinFiles) {
				string path = file.GetMetadata ("FullPath");
				var link = GetLinkPath (file, path);
				var item = new TaskItem (path);
				item.SetMetadata ("Link", link);
				items.Add (item);
				names.Add (link);
			}

			AddinFilesWithLinkMetadata = items.ToArray ();

			names.Sort (StringComparer.Ordinal);

			var doc = new XDocument (
				new XElement ("ExtensionModel",
					new XElement ("Runtime",
						names.Select (n => new XElement ("Import", new XAttribute ("file", n.Replace ('\\', '/'))))
					)
				)
			);

			//Save to memory stream so we get UTF-8 preamble. StringWriter does UTF-16 preamble.
			var ms = new MemoryStream ();
			doc.Save (ms);

			ms.Position = 0;
			var txt = new StreamReader (ms).ReadToEnd ();

			if (File.Exists (ManifestFile)) {
				var existing = File.ReadAllText (ManifestFile);
				if (existing == txt) {
					return true;
				}
			}

			File.WriteAllText (ManifestFile, txt);

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

