using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MonoDevelop.Addins.Tasks
{
	public class GenerateDependencyAttributes : Task
	{
		[Required]
		public string Language { get; set; }

		[Required]
		public string Filename { get; set; }

		[Output]
		public ITaskItem[] AddinReferences { get; set; }

		public override bool Execute ()
		{
			var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider (Language);
			if (provider == null) {
				Log.LogError ("Could not create CodeDOM provider for language '{0}'", Language);
				return false;
			}

			var ccu = new CodeCompileUnit ();
			foreach (var addin in AddinReferences) {
				//assembly:Mono.Addins.AddinDependency ("::%(Identity)", MonoDevelop.BuildInfo.Version)]
				ccu.AssemblyCustomAttributes.Add (
					new CodeAttributeDeclaration (
						new CodeTypeReference ("Mono.Addins.AddinDependencyAttribute"),
						new [] {
							new CodeAttributeArgument (new CodePrimitiveExpression ("::" + addin.ItemSpec)),
							new CodeAttributeArgument (new CodePrimitiveExpression (addin.GetMetadata ("Version")))
						}
					)
				);
			}

			Directory.CreateDirectory (Path.GetDirectoryName (Filename));

			using (var sw = new StreamWriter (Filename)) {
				provider.GenerateCodeFromCompileUnit (ccu, sw, new CodeGeneratorOptions ());
			}

			return false;
		}
	}
}

