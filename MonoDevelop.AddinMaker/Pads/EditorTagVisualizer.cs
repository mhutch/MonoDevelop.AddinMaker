// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using MonoDevelop.Components.PropertyGrid;
using MonoDevelop.Ide.Gui;
using Xwt;

namespace MonoDevelop.AddinMaker.Pads
{
	class EditorTagVisualizer
		//XWT paned is buggy with GTK propertygrid child
		: Gtk.VPaned
	{
		readonly TreeView treeView;
		readonly TreeStore store;
		readonly Components.PropertyGrid.PropertyGrid propertyGrid;

		DataField<string> spanField = new DataField<string> ();
		DataField<string> tagNameField = new DataField<string> ();
		DataField<ITag> tagField = new DataField<ITag> ();

		ActiveEditorTracker editorTracker;
		IViewTagAggregatorFactoryService tagAggregatorFactoryService;
		ITagAggregator<ITag> aggregator;
		readonly HashSet<IMappingTagSpan<ITag>> activeTags = new HashSet<IMappingTagSpan<ITag>> ();

		public EditorTagVisualizer (IPadWindow window)
		{
			tagAggregatorFactoryService = Ide.Composition.CompositionManager.GetExportedValue<IViewTagAggregatorFactoryService> ();

			store = new TreeStore (spanField, tagNameField, tagField);
			treeView = new TreeView (store) {
				BorderVisible = false,
				HeadersVisible = false,
			};
			treeView.Columns.Add ("Span", spanField).Expands = false;
			treeView.Columns.Add ("Tag", tagNameField).Expands = true;

			Add1 ((Gtk.Widget)Toolkit.CurrentEngine.GetNativeWidget (treeView));

			propertyGrid = new PropertyGrid ();
			Add2 (propertyGrid);
			ShowAll ();

			//FIXME: port to XWT so we can use percentage position
			Position = 600;

			LoadPropertyEditorAssembly (propertyGrid, typeof (EditorTagVisualizer).Assembly);

			treeView.SelectionChanged += SelectionChanged;

			editorTracker = new ActiveEditorTracker ();
			editorTracker.ActiveEditorChanged += ActiveEditorChanged;
			if (editorTracker.TextView != null) {
				ActiveEditorChanged (editorTracker, new ActiveEditorChangedEventArgs (editorTracker.TextView, null));
			}
		}

		//FIXME: why is PropertyEditorCell public but EditorManager isn't?
		static void LoadPropertyEditorAssembly (PropertyGrid propertyGrid, Assembly assembly)
		{
			const BindingFlags bfi = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
			var managerProp = propertyGrid.GetType ().GetProperty ("EditorManager", bfi);
			var manager = managerProp.GetValue (propertyGrid);
			var loadMeth = manager.GetType ().GetMethod ("LoadEditor");
			loadMeth.Invoke (manager, new [] { assembly });
		}

		void SelectionChanged (object sender, EventArgs e)
		{
			var pos = treeView.SelectedRow;
			if (pos == null) {
				propertyGrid.CurrentObject = null;
				return;
			}

			var tag = store.GetNavigatorAt (pos)?.GetValue (tagField);
			propertyGrid.SetCurrentObject (tag, new object [] { tag, });
		}



		void ActiveEditorChanged (object sender, ActiveEditorChangedEventArgs e)
		{
			if (e.OldView != null) {
				e.OldView.Caret.PositionChanged -= CaretPositionChanged;
			}
			aggregator?.Dispose ();

			if (e.NewView != null) {
				e.NewView.Caret.PositionChanged += CaretPositionChanged;
				aggregator = tagAggregatorFactoryService.CreateTagAggregator<ITag> (e.NewView);
				aggregator.BatchedTagsChanged += BatchedTagsChanged;
			}

			Update ();
		}

		void CaretPositionChanged (object sender, CaretPositionChangedEventArgs e)
		{
			Update ();
		}

		void BatchedTagsChanged (object sender, BatchedTagsChangedEventArgs e)
		{
			if (AnyContainsCaret (editorTracker.TextView, e.Spans)) {
				Update ();
			}
		}

		void Update ()
		{
			var textView = editorTracker.TextView;
			var caretSpan = new SnapshotSpan (textView.Caret.Position.BufferPosition, 0);
			var tags = aggregator.GetTags (caretSpan).ToList ();
			Application.Invoke (() => UpdateStore (tags));
		}

		void UpdateStore (List<IMappingTagSpan<ITag>> tags)
		{
			if (activeTags.Count == tags.Count && tags.All (t => activeTags.Contains (t))) {
				Console.WriteLine ("up to date");
				return;
			}
			activeTags.Clear ();
			foreach (var t in tags) {
				activeTags.Add (t);
			}

			var textView = editorTracker.TextView;

			store.Clear ();

			if (tags.Count == 0) {
				return;
			}

			foreach (var mappingTag in tags) {
				var span = mappingTag.Span;
				var nav = store.AddNode ().SetValues (
					tagNameField, mappingTag.Tag.GetType ().ToString (),
					spanField, $"[{GetPosition (span.Start)}-{GetPosition (span.End)}]",
					tagField, mappingTag.Tag
				);
			}

			treeView.SelectRow (store.GetFirstNode ().CurrentPosition);

			string GetPosition (IMappingPoint mp)
			{
				var p = mp.GetPoint (textView.TextBuffer, PositionAffinity.Predecessor);
				if (p.HasValue) {
					return p.Value.Position.ToString ();
				}
				return "?";
			}
		}

		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();

			editorTracker?.Dispose ();
			aggregator?.Dispose ();
		}

		static bool AnyContainsCaret (ITextView textView, ReadOnlyCollection<IMappingSpan> changedSpans)
		{
			var caretSpan = new SnapshotSpan (textView.Caret.Position.BufferPosition, 0);
			foreach (var changed in changedSpans) {
				foreach (var cs in changed.GetSpans (textView.TextBuffer)) {
					if (cs.IntersectsWith (caretSpan)) {
						return true;
					}
				}
			}
			return false;
		}
	}

	[PropertyEditorType (typeof (IClassificationType))]
	class ClassificationTypeEditorCell : PropertyEditorCell
	{
		protected override string GetValueText ()
		{
			return ((IClassificationType)Value)?.Classification;
		}

		protected override IPropertyEditor CreateEditor (Gdk.Rectangle cellArea, Gtk.StateType state)
		{
			return null;
		}
	}
}