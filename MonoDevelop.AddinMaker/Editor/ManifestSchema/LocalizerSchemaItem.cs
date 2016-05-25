//
// ExtensionSchemaItem.cs
//
// Author:
//       Mikayla Hutchinson <m.j.hutchinson@gmail.com>
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
using MonoDevelop.Xml.Dom;
using System.Collections.Generic;
using MonoDevelop.Ide.CodeCompletion;

namespace MonoDevelop.AddinMaker.Editor.ManifestSchema
{
	//TODO: Completion for custom localizers. Not really possible right now as there is no metadata.
	class LocalizerSchemaElement : SchemaElement
	{
		public LocalizerSchemaElement () : base (
			"Localizer",
			"Declares a localizer for the add-in")
		{
		}

		readonly SchemaElement localeItem = new SchemaElement (
			"Locale",
			"Defines a string table for a language.",
			new [] {
				new SchemaElement (
					"Msg",
					"Defines a message translation",
					null,
					new [] {
						new SchemaAttribute ("id", "Identifier of the message."),
						new SchemaAttribute ("str", "Translation of the message.")
					}
				)
			},
			new [] {
				new SchemaAttribute ("id", "The identifier of the language. May include the country code.")
			}
		);

		public override void GetAttributeCompletions (CompletionDataList list, IAttributedXObject attributedOb, Dictionary<string, string> existingAtts)
		{
			if (!existingAtts.ContainsKey ("type")) {
				list.Add ("Gettext", null, "Localizes the add-in with a Gettext catalog.");
				list.Add ("StringResource", null, "Localizes the add-in with .NET string resources.");
				list.Add ("StringTable", null, "Localizes the add-in with string table defined in the manifest.");
				return;
			}

			string type;
			if (!existingAtts.TryGetValue ("type", out type)) {
				return;
			}

			if (type == "Gettext") {
				if (!existingAtts.ContainsKey ("catalog")) {
					list.Add ("catalog", null, "Name of the catalog which contains the strings.");
				}
				if (!existingAtts.ContainsKey ("location")) {
					list.Add ("location", null, "Relative path to the location of the catalog. This path must be relative to the add-in location..");
				}
			}
		}

		static string GetTypeAttributeValue (XElement el)
		{
			var typeAtt = el.Attributes.Get (new XName ("type"), true);
			if (typeAtt == null || string.IsNullOrEmpty (typeAtt.Value)) {
				return null;
			}
			return typeAtt.Value;
		}

		public override SchemaElement GetChild (XElement element)
		{
			var type = GetTypeAttributeValue (element);

			if (type == "StringTable") {
				if (element.Name.FullName == localeItem.Name) {
					return localeItem;
				}
			}

			return null;
		}

		public override void GetElementCompletions (CompletionDataList list, XElement element)
		{
			var type = GetTypeAttributeValue (element);

			if (type == "StringTable") {
				list.Add (localeItem.Name, null, localeItem.Description);
			}
		}
	}
}
