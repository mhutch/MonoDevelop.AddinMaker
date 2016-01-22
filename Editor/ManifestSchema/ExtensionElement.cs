//
// ExtensionSchemaItem.cs
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

using System.Collections.Generic;
using System.Linq;
using Mono.Addins.Description;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;

namespace MonoDevelop.AddinMaker.Editor.ManifestSchema
{
	//TODO: completion for extension points defined in this addin
	class ExtensionElement : SchemaElement
	{
		readonly AddinProjectFlavor project;

		public ExtensionElement (AddinProjectFlavor project) : base ("Extension", "Declares an extension")
		{
			this.project = project;
		}

		public override void GetAttributeCompletions (CompletionDataList list, IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			if (!existingAtts.ContainsKey ("path")) {
				list.Add ("path", null, "The path of the extension");
			}
		}

		//TODO: get these from the module, not the project
		IEnumerable<Mono.Addins.Addin> GetReferencedAddins ()
		{
			return project.GetReferencedAddins ();
		}

		public override void GetAttributeValueCompletions (CompletionDataList list, IAttributedXObject attributedOb, XAttribute att)
		{
			if (att.Name.FullName != "path") {
				return;
			}

			foreach (var addin in GetReferencedAddins ()) {
				foreach (ExtensionPoint ep in addin.Description.ExtensionPoints) {
					list.Add (ep.Path, null, ep.Name + "\n" + ep.Description);
				}
			}
		}

		public ICompletionDataList GetPathCompletions (string subPath)
		{
			var paths = new HashSet<string> ();
			var cp = new CompletionDataList ();
			var addins = GetReferencedAddins ().ToList ();
			foreach (var addin in addins) {
				foreach (ExtensionPoint ep in addin.Description.ExtensionPoints) {
					if (ep.Path.StartsWith (subPath, System.StringComparison.Ordinal)) {
						string spath = ep.Path.Substring (subPath.Length);
						int i = spath.IndexOf ('/');
						if (i != -1)
							spath = spath.Substring (0, i);
						if (paths.Add (spath)) {
							if (i == -1)
								cp.Add (spath, null, ep.Name + "\n" + ep.Description);
							else
								cp.Add (spath);
						}
					}
					if (ep.Path == subPath.TrimEnd ('/')) {
						var extensions = ExtensionNodeElement.GetExtensions (project, ep).ToList ();
						foreach (ExtensionNodeType nodeType in ep.NodeSet.GetAllowedNodeTypes ()) {
							var allowedChildren = nodeType.GetAllowedNodeTypes ();
							if (allowedChildren.Count == 0)
								continue;
							foreach (var ext in extensions) {
								foreach (ExtensionNodeDescription node in ext.ExtensionNodes) {
									if (node.GetNodeType () == nodeType && !string.IsNullOrEmpty (node.Id)) {
										cp.Add (node.Id);
									}
								}
							}
						}
					}
				}
			}
			return cp;
		}

		ExtensionPoint GetExtensionPoint (XElement element)
		{
			if (element == null) {
				return null;
			}

			var pathAtt = element.Attributes.Get (new XName ("path"), true);
			if (pathAtt == null || pathAtt.Value == null) {
				return null;
			}

			return GetExtensionPoint (pathAtt.Value);
		}

		ExtensionPoint GetExtensionPoint (string path)
		{
			foreach (var addin in project.GetReferencedAddins ()) {
				foreach (ExtensionPoint ep in addin.Description.ExtensionPoints) {
					if (ep.Path == path) {
						return ep;
					}
				}
			}
			return null;
		}

		public override void GetElementCompletions (CompletionDataList list, XElement element)
		{
			var ep = GetExtensionPoint (element);
			if (ep == null) {
				return;
			}

			foreach (ExtensionNodeType n in ep.NodeSet.GetAllowedNodeTypes ()) {
				list.Add (n.NodeName, null, n.Description);
			}
		}

		public override SchemaElement GetChild (XElement element)
		{
			var ep = GetExtensionPoint (element.Parent as XElement);
			if (ep == null) {
				return null;
			}

			var node = ep.NodeSet.GetAllowedNodeTypes ().FirstOrDefault (n => n.NodeName == element.Name.FullName);
			if (node != null) {
				return new ExtensionNodeElement (project, ep, node);
			}

			return null;
		}
	}
}
