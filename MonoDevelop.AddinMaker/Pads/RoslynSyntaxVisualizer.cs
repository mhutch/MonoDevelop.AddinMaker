// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using MonoDevelop.Core;
using Xwt;
using Xwt.Drawing;

namespace MonoDevelop.AddinMaker.Pads
{
	class RoslynSyntaxVisualizer : TreeView
	{
		ActiveEditorTracker editorTracker;
		readonly DataField<string> markupField = new DataField<string> ();
		readonly DataField<TextSpan> spanField = new DataField<TextSpan> ();
		readonly TreeStore store;

		SourceText lastSourceText;
		bool suppressChangeEvent;

		public RoslynSyntaxVisualizer ()
		{
			Columns.Add ("Text", new TextCellView { MarkupField = markupField });
			HeadersVisible = false;
			BorderVisible = false;

			store = new TreeStore (markupField, spanField);
			DataSource = store;

			editorTracker = new ActiveEditorTracker ();
			editorTracker.ActiveEditorChanged += ActiveEditorChanged;
			ActiveEditorChanged (editorTracker, new ActiveEditorChangedEventArgs (editorTracker.TextView, null, editorTracker.Document, null));

			SelectionChanged += SelectionChangedHandler;
		}

		void SelectionChangedHandler (object sender, EventArgs e)
		{
			if (suppressChangeEvent) {
				return;
			}

			var pos = SelectedRow;
			if (pos == null) {
				return;
			}

			var nav = store.GetNavigatorAt (pos);
			var span = nav.GetValue (spanField);

			var roslynSnapshot = lastSourceText.FindCorrespondingEditorTextSnapshot ();
			var editorSpan = new SnapshotSpan (roslynSnapshot, span.Start, span.Length);

			suppressChangeEvent = true;
			try {
				editorTracker.TextView.Selection.Select (editorSpan, false);
			} finally {
				suppressChangeEvent = false;
			}
		}

		void ActiveEditorChanged (object sender, ActiveEditorChangedEventArgs e)
		{
			if (e.OldDocument != null && e.OldDocument != e.NewDocument) {
				e.OldDocument.AnalysisDocumentChanged -= AnalysisDocumentChanged;
			}

			if (e.NewDocument != null && e.OldDocument != e.NewDocument) {
				e.NewDocument.AnalysisDocumentChanged += AnalysisDocumentChanged;
			}

			if (e.OldView != null) {
				e.OldView.Caret.PositionChanged -= CaretPositionChanged;
			}

			if (e.NewView != null) {
				e.NewView.Caret.PositionChanged += CaretPositionChanged;
			}

			AnalysisDocumentChanged (e.NewDocument, EventArgs.Empty);
		}

		void CaretPositionChanged (object sender, Microsoft.VisualStudio.Text.Editor.CaretPositionChangedEventArgs e)
		{
			SelectBestMatchForCaret ();
		}

		CancellationTokenSource cts;

		void AnalysisDocumentChanged (object sender, System.EventArgs e)
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

			if (editorTracker.Document.AnalysisDocument == null) {
				store.Clear ();
				lastSourceText = null;
				return;
			}

			var tree = await editorTracker.Document.AnalysisDocument.GetSyntaxTreeAsync (ct);
			var root = await tree.GetRootAsync (ct);
			var text = await editorTracker.Document.AnalysisDocument.GetTextAsync (ct);

			if (ct.IsCancellationRequested) {
				return;
			}

			store.Clear ();

			AddNode (store.AddNode (), root);
			lastSourceText = text;

			SelectBestMatchForCaret ();
		}

		void AddNode (TreeNavigator treeNavigator, SyntaxNode syntaxNode)
		{
			var leadingTrivia = syntaxNode.GetLeadingTrivia ();
			var trailingTrivia = syntaxNode.GetTrailingTrivia ();

			AddLeadingTrivia (leadingTrivia);

			SetNodeText (treeNavigator, syntaxNode.GetType ().Name, syntaxNode.Span, Colors.DarkBlue);

			foreach (var child in syntaxNode.ChildNodesAndTokens ()) {
				treeNavigator.AddChild ();
				if (child.IsNode) {
					AddNode (treeNavigator, child.AsNode ());
				} else {
					var token = child.AsToken ();
					if (token.LeadingTrivia != leadingTrivia) {
						AddLeadingTrivia (token.LeadingTrivia);
					}
					SetNodeText (treeNavigator, token.GetType ().Name, token.Span, Colors.DarkGreen);
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
					AddTrivia (trivia);
					treeNavigator.InsertAfter ();
				}
			}

			void AddTrailingTrivia (SyntaxTriviaList triviaList)
			{
				foreach (var trivia in triviaList) {
					treeNavigator.InsertAfter ();
					AddTrivia (trivia);
				}
			}

			void AddTrivia (SyntaxTrivia trivia) => SetNodeText (treeNavigator, trivia.GetType ().Name, trivia.Span, Colors.DarkRed);
		}

		void SetNodeText (TreeNavigator nav, string text, TextSpan span, Color color)
		{
			nav.SetValues (
				markupField,
				$"<span color='{color.ToHexString ()}'>{GLib.Markup.EscapeText (text)} [{span.Start}-{span.End}]</span>",
				spanField,
				span
			);
		}

		void SelectBestMatchForCaret ()
		{
			if (suppressChangeEvent || lastSourceText == null) {
				return;
			}

			var roslynSnapshot = lastSourceText.FindCorrespondingEditorTextSnapshot ();
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
				CollapseRow (firstNodePos);
				ExpandToRow (node.CurrentPosition);
				ScrollToRow (node.CurrentPosition);
				SelectRow (node.CurrentPosition);
			} finally {
				suppressChangeEvent = false;
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				editorTracker?.Dispose ();
				editorTracker = null;
			}

			base.Dispose (disposing);
		}
	}
}