﻿using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MahAppsIconPackPreview.Adornments.CSharp {
    internal class CSharpIconTagger : ITagger<IntraTextAdornmentTag>, IDisposable {
        private readonly ITextBuffer _buffer;
        private bool _isProcessing;
        private bool _isDisposed;
        public CSharpIconTagger(ITextBuffer buffer) {
            _buffer = buffer;
            _buffer.Changed += OnBufferChange;
        }

        private void OnBufferChange(object sender, TextContentChangedEventArgs e) {
            if (_isProcessing || e.Changes.Count == 0)
                return;

            try {
                _isProcessing = true;
                int start = e.Changes.First().NewSpan.Start;
                int end = e.Changes.Last().NewSpan.End;

                ITextSnapshotLine startLine = e.After.GetLineFromPosition(start);
                ITextSnapshotLine endLine = e.After.GetLineFromPosition(end);

                SnapshotSpan span = new(e.After, Span.FromBounds(startLine.Start, endLine.End));
                TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(span));
            } finally {
                _isProcessing = false;
            }
        }


        public IEnumerable<ITagSpan<IntraTextAdornmentTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            if (_isProcessing)
                yield break;

            foreach (SnapshotSpan span in spans) {
                SnapshotSpan currentSpan = span;
                string text = currentSpan.GetText();
                MatchCollection matches = Regex.Matches(text, PackIconControlUtil.CSHARP_REGAX_PATTERN);

                foreach (Match match in matches) {
                    var adornment = IconAdornment.CreateAdornment(match.Value);

                    if (adornment == null) {
                        continue;
                    }

                    IntraTextAdornmentTag tag = new(adornment, null, PositionAffinity.Successor);
                    SnapshotSpan colorSpan = new(currentSpan.Snapshot, currentSpan.Start + match.Index, 0);

                    yield return new TagSpan<IntraTextAdornmentTag>(colorSpan, tag);
                }
            }
        }

        public void Dispose() {
            if (!_isDisposed) {
                _buffer.Changed -= OnBufferChange;
            }

            _isDisposed = true;
        }


        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }
}
