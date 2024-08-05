using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace MaterialIconPreview.Adornments {
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class IconAdornmentTaggerProvider : IViewTaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            if (buffer.CurrentSnapshot.Length < 10000) {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new IconTagger(buffer, textView)) as ITagger<T>;
            }
            return null;
        }
    }
}
