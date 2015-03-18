//
// ExtensionNodeSchemaItem.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Completion;
using Mono.Addins.Description;
using MonoDevelop.Ide.CodeCompletion;
using MonoDevelop.Xml.Dom;

namespace MonoDevelop.AddinMaker.ManifestSchema
{
	class ExtensionNodeSchemaItem : SchemaItem
	{
		readonly ExtensionNodeType info;

		public ExtensionNodeSchemaItem (ExtensionNodeType info) : base (info.NodeName, info.Description)
		{
			this.info = info;
		}

		public override void GetAttributeCompletions (CompletionDataList list, IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			var required = new NodeCompletionCategory ("Required", 0);
			var optional = new NodeCompletionCategory ("Optional", 1);

			foreach (NodeTypeAttribute att in info.Attributes) {
				if (!existingAtts.ContainsKey (att.Name)) {
					var data = new NodeTypeAttributeCompletionData (att) {
						CompletionCategory = att.Required ? required : optional
					};
					list.Add (data);
				}
			}

			list.Add ("id", null, "ID for the extension, unique in this extension point.");
			list.Add ("insertbefore", null, "ID of an existing extension before which to insert this.");
			list.Add ("insertafter", null, "ID of an existing extension after which to insert this.");
		}

		public override SchemaItem GetChild (XElement el)
		{
			var node = info.GetAllowedNodeTypes ().FirstOrDefault (n => n.NodeName == el.Name.FullName);
			if (node != null) {
				return new ExtensionNodeSchemaItem (node);
			}

			return null;
		}

		public override void GetElementCompletions (CompletionDataList list, XElement element)
		{
			foreach (ExtensionNodeType n in info.GetAllowedNodeTypes ()) {
				list.Add (n.NodeName, null, n.Description);
			}
		}

		class NodeCompletionCategory : CompletionCategory
		{
			readonly int weight;

			public NodeCompletionCategory (string label, int weight)
			{
				DisplayText = label;
				this.weight = weight;
			}

			public override int CompareTo (CompletionCategory other)
			{
				var n = other as NodeCompletionCategory;
				if (n != null)
					return weight - n.weight;
				return string.Compare (DisplayText, other.DisplayText, StringComparison.Ordinal);
			}
		}

		class NodeTypeAttributeCompletionData : CompletionData
		{
			NodeTypeAttribute att;
			string markup;

			public NodeTypeAttributeCompletionData (NodeTypeAttribute att)
			{
				this.att = att;
			}

			public override string DisplayText {
				get { return att.Name; }
				set { throw new NotSupportedException (); }
			}

			public override string Description {
				get { return markup ?? (markup = GenerateDescriptionMarkup ()); }
				set { throw new NotSupportedException (); }
			}

			public override string CompletionText {
				get { return att.Name; }
				set { throw new NotSupportedException (); }
			}

			public override DisplayFlags DisplayFlags {
				get {
					var flags = DisplayFlags.DescriptionHasMarkup;
					if (att.Required) {
						flags |= DisplayFlags.MarkedBold;
					}
					return flags;
				}
				set { throw new NotSupportedException (); }
			}

			string GenerateDescriptionMarkup ()
			{
				var sb = new StringBuilder ();
				sb.AppendFormat ("{0}", GLib.Markup.EscapeText (att.Name));
				sb.AppendLine ();

				if (att.Required) {
					sb.AppendLine ("<b>Required</b>");
				}

				switch (att.ContentType) {
				case Mono.Addins.ContentType.Text:
					if (att.Localizable) {
						sb.AppendLine ("<i>Localizable</i>");
					}
					break;
				case Mono.Addins.ContentType.Class:
					sb.Append ("<i>Type");
					if (string.IsNullOrEmpty (att.Type)) {
						sb.Append (": ");
						sb.Append (GLib.Markup.EscapeText (att.Type));
					}
					sb.AppendLine ("</i>");
					break;
				case Mono.Addins.ContentType.Resource:
					sb.AppendLine ("<i>Resource</i>");
					break;
				case Mono.Addins.ContentType.File:
					sb.AppendLine ("<i>File</i>");
					break;
				}

				if (string.IsNullOrEmpty (att.Description)) {
					return sb.ToString ();
				}

				sb.AppendLine ();
				sb.AppendLine (GLib.Markup.EscapeText (att.Description));

				return sb.ToString ();
			}
		}
	}
}
