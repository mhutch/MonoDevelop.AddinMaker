// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using MonoDevelop.Components.PropertyGrid;
using MonoDevelop.Core;
using Xwt;
using Xwt.Drawing;

namespace MonoDevelop.AddinMaker.Pads
{
	class RoslynSyntaxVisualizer
		//XWT paned is buggy with GTK propertygrid child
		: Gtk.VPaned
	{
		readonly TreeView treeView;
		readonly TreeStore store;
		readonly PropertyGrid propertyGrid;
		readonly DataField<string> markupField = new DataField<string> ();
		readonly DataField<TextSpan> spanField = new DataField<TextSpan> ();
		readonly DataField<object> nodeField = new DataField<object> ();

		readonly ActiveEditorTracker editorTracker;

		SourceText lastSourceText;
		bool suppressChangeEvent;

		public RoslynSyntaxVisualizer ()
		{
			store = new TreeStore (markupField, spanField, nodeField);
			treeView = new TreeView (store) {
				BorderVisible = false,
				HeadersVisible = false,
			};

			treeView.Columns.Add ("Text", new TextCellView { MarkupField = markupField });

			Add1 ((Gtk.Widget)Toolkit.CurrentEngine.GetNativeWidget (treeView));

			propertyGrid = new PropertyGrid ();
			Add2 (propertyGrid);
			ShowAll ();
			propertyGrid.ShowToolbar = false;

			EditorTagVisualizer.LoadPropertyEditorAssembly (propertyGrid, typeof (RoslynSyntaxVisualizer).Assembly);

			//FIXME: port to XWT so we can use percentage position
			Position = 600;

			editorTracker = new ActiveEditorTracker ();
			editorTracker.ActiveEditorChanged += ActiveEditorChanged;
			ActiveEditorChanged (editorTracker, new ActiveEditorChangedEventArgs (editorTracker.TextView, null, editorTracker.Document, null));

			treeView.SelectionChanged += SelectionChangedHandler;
		}

		void SelectionChangedHandler (object sender, EventArgs e)
		{
			if (suppressChangeEvent) {
				return;
			}

			var pos = treeView.SelectedRow;
			if (pos == null) {
				SetPropertyGridValue (null);
				return;
			}

			var roslynSnapshot = lastSourceText.FindCorrespondingEditorTextSnapshot ();
			if (roslynSnapshot == null) {
				SetPropertyGridValue (null);
				return;
			}

			var nav = store.GetNavigatorAt (pos);
			var span = nav.GetValue (spanField);
			var obj = nav.GetValue (nodeField);

			var editorSpan = new SnapshotSpan (roslynSnapshot, span.Start, span.Length);

			suppressChangeEvent = true;
			try {
				editorTracker.TextView.Caret.MoveTo (editorSpan.Start);
				editorTracker.TextView.Selection.Select (editorSpan, false);
				editorTracker.TextView.Caret.EnsureVisible ();
			} finally {
				suppressChangeEvent = false;
			}

			SetPropertyGridValue (obj);
		}

		void SetPropertyGridValue (object syntaxNode)
		{
			if (syntaxNode == null) {
				propertyGrid.CurrentObject = syntaxNode;
				return;
			}

			object [] propertyProviders = null;
		//	if (syntaxNode is ClassificationTag ct) {
		//		propertyProviders = new object [] { new ClassificationTagDescriptor (ct) };
		//	}
			propertyGrid.SetCurrentObject (syntaxNode, propertyProviders ?? new object [] { syntaxNode });
		}

		void ActiveEditorChanged (object sender, ActiveEditorChangedEventArgs e)
		{
			if (e.OldView != null) {
				e.OldView.Caret.PositionChanged -= CaretPositionChanged;
				e.OldView.TextBuffer.Changed -= BufferChanged;
			}

			if (e.NewView != null) {
				e.NewView.Caret.PositionChanged += CaretPositionChanged;
				e.NewView.TextBuffer.Changed += BufferChanged;
			}

			AnalysisDocumentChanged (e.NewDocument, EventArgs.Empty);
		}

		void BufferChanged (object sender, TextContentChangedEventArgs e)
		{
			Runtime.RunInMainThread (Update);
		}

		void CaretPositionChanged (object sender, Microsoft.VisualStudio.Text.Editor.CaretPositionChangedEventArgs e)
		{
			SelectBestMatchForCaret ();
		}

		CancellationTokenSource cts;

		void AnalysisDocumentChanged (object sender, EventArgs e)
		{
			Runtime.RunInMainThread (Update);
		}

		async Task Update ()
		{
			if (cts != null) {
				cts.Cancel ();
			}
			cts = new CancellationTokenSource ();
			var ct = cts.Token;

			var snapshot = editorTracker.TextView?.TextBuffer?.CurrentSnapshot;
			Document document = snapshot?.GetOpenDocumentInCurrentContextWithChanges ();

			if (document == null) {
				store.Clear ();
				lastSourceText = null;
				return;
			}

			var tree = await document.GetSyntaxTreeAsync (ct);
			var root = await tree.GetRootAsync (ct);
			var text = await document.GetTextAsync (ct);

			if (ct.IsCancellationRequested) {
				return;
			}

			store.Clear ();

			AddNode (store.AddNode (), root);
			lastSourceText = text;

			SelectBestMatchForCaret ();
		}

		void AddNode (TreeNavigator treeNavigator, SyntaxNode syntaxNode, bool hideLeadingTrivia = false)
		{
			var leadingTrivia = syntaxNode.GetLeadingTrivia ();
			var trailingTrivia = syntaxNode.GetTrailingTrivia ();

			if (!hideLeadingTrivia) {
				AddLeadingTrivia (leadingTrivia);
			}

			AddSyntaxNode (treeNavigator, syntaxNode);

			bool isFirst = true;
			foreach (var child in syntaxNode.ChildNodesAndTokens ()) {
				//leading trivia is duplicated between the compilation unit and its first child
				bool hideChildLeadingTrivia = isFirst && syntaxNode is ICompilationUnitSyntax;
				isFirst = false;

				treeNavigator.AddChild ();

				if (child.IsNode) {
					AddNode (treeNavigator, child.AsNode (), hideChildLeadingTrivia);
				} else {
					var token = child.AsToken ();
					if (token.LeadingTrivia != leadingTrivia) {
						AddLeadingTrivia (token.LeadingTrivia);
					}
					AddSyntaxToken (treeNavigator, token);
					if (token.TrailingTrivia != trailingTrivia) {
						AddTrailingTrivia (token.TrailingTrivia);
					}
				}

				treeNavigator.MoveToParent ();
			}

			AddTrailingTrivia (trailingTrivia);

			void AddLeadingTrivia (SyntaxTriviaList triviaList)
			{
				foreach (var trivia in triviaList) {
					AddSyntaxTrivia (treeNavigator, trivia);
					treeNavigator.InsertAfter ();
				}
			}

			void AddTrailingTrivia (SyntaxTriviaList triviaList)
			{
				foreach (var trivia in triviaList) {
					treeNavigator.InsertAfter ();
					AddSyntaxTrivia (treeNavigator, trivia);
				}
			}
		}

		//FIXME: we should compute the text on demand, but XWT doesn't let us do this
		void AddSyntaxTrivia (TreeNavigator nav, SyntaxTrivia trivia)
		{
			var kind = (Microsoft.CodeAnalysis.CSharp.SyntaxKind)trivia.RawKind;
			SetNodeText (nav, $"{kind}", trivia.Span, Colors.DarkRed, trivia);
		}

		void AddSyntaxToken (TreeNavigator nav, SyntaxToken token)
		{
			var kind = (Microsoft.CodeAnalysis.CSharp.SyntaxKind)token.RawKind;
			SetNodeText (nav, $"{kind}", token.Span, Colors.DarkGreen, token);
		}

		void AddSyntaxNode (TreeNavigator nav, SyntaxNode node)
		{
			var kind = (Microsoft.CodeAnalysis.CSharp.SyntaxKind)node.RawKind;
			SetNodeText (nav, $"{kind}", node.Span, Colors.DarkBlue, node);
		}

		void SetNodeText (TreeNavigator nav, string text, TextSpan span, Color color, object node)
		{
			nav.SetValues (
				markupField,
				$"<span color='{color.ToHexString ()}'>{GLib.Markup.EscapeText (text)} [{span.Start}-{span.End}]</span>",
				spanField,
				span,
				nodeField,
				node
			);
		}

		void SelectBestMatchForCaret ()
		{
			if (suppressChangeEvent || lastSourceText == null) {
				SetPropertyGridValue (null);
				return;
			}

			var roslynSnapshot = lastSourceText.FindCorrespondingEditorTextSnapshot ();
			if (roslynSnapshot == null) {
				SetPropertyGridValue (null);
				return;
			}
			var point = editorTracker.TextView.Caret.Position.BufferPosition.TranslateTo (roslynSnapshot, PointTrackingMode.Positive);

			var node = store.GetFirstNode ();
			var firstNodePos = node.CurrentPosition;
			while (true) {
				var span = node.GetValue (spanField);
				if (span.Contains (point.Position)) {
					if (!node.MoveToChild ()) {
						break;
					}
					continue;
				}
				if (!node.MoveNext ()) {
					break;
				}
			}

			suppressChangeEvent = true;
			try {
				treeView.CollapseRow (firstNodePos);
				treeView.ExpandToRow (node.CurrentPosition);
				treeView.ScrollToRow (node.CurrentPosition);
				treeView.SelectRow (node.CurrentPosition);
				SetPropertyGridValue (node.GetValue (nodeField));
			} finally {
				suppressChangeEvent = false;
			}
		}

		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();

			editorTracker?.Dispose ();
		}
	}
}