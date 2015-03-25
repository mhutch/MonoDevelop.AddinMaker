//
// FileTemplateEditorExtension.cs
//
// Author:
//       Michael Hutchinson <m.j.hutchinson@gmail.com>
//
// Copyright (c) 2015 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace MonoDevelop.AddinMaker.Editor
{
	class FileTemplateEditorExtension : SchemaBasedEditorExtension
	{
		public override bool ExtendsEditor (MonoDevelop.Ide.Gui.Document doc, MonoDevelop.Ide.Gui.Content.IEditableTextBuffer editor)
		{
			return base.ExtendsEditor (doc, editor) && doc.HasProject && doc.Project is AddinProject;
		}

		protected override SchemaElement CreateSchema ()
		{
			return new SchemaElement (null, null, new[] {
				new SchemaElement ("Template", "Root element for file templates",
					new[] {
						new SchemaElement ("TemplateConfiguration", "Metadata for the template", new[] {
							//TODO: any way we can introspect completion for custom file template types?
							new SchemaElement ("Type", "Custom template type. Must be subclass of MonoDevelop.Ide.Templates.FileTemplate."),
							new SchemaElement ("_Name", "The name of the template, displayed in the New File dialog."),
							new SchemaElement ("_Category", "The category under which to group this template in the New File dialog."),
							new SchemaElement ("_Description", "The description for this template in the New File dialog"),
							//TODO: language ID completion.
							new SchemaElement ("LanguageName", "If specified, the template will only be displayed for projects targeting this language."),
							//TODO: project type IDE completion. Don't think this is currently introspectable.
							new SchemaElement ("ProjectType", "If specified, the template will only be displayed for projects of this type."),
							//TODO: icon ID completion
							new SchemaElement ("Icon", "ID of the template's icon."),
							//File wizard codepath currently commented out
							//new SchemaAttribute ("Wizard", ""),
							new SchemaElement ("DefaultFilename", "The default filename, if the user does not specific one.", null, new[] {
								new BoolSchemaAttribute ("IsFixed", "Whether the filename cannot be changed by the user")
							}),
						}),
						//TODO: completion for files
						new SchemaElement ("TemplateFiles", "Files and directories to be created as part of this template/"),
						//TODO: completion for conditions
						new SchemaElement ("Conditions", "Conditions that control when the template is displayed."),
					},
					new[] {
						new SchemaAttribute ("Originator", "The originator of the template (optional, unused)."),
						new SchemaAttribute ("Created", "The creation date of the template (optional, unused)."),
						new SchemaAttribute ("LastModified", "The date the template was last modified (optional, unused)."),
					}
				)
			});
		}
	}
}