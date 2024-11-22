using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace MahAppsIconPackPreview.Adornments.Xaml {
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("XAML")]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class XamlIconAdornmentTaggerProvider : IViewTaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            if (buffer.CurrentSnapshot.Length < 10000) {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new XamlIconTagger(buffer)) as ITagger<T>;
            }
            return null;
        }
    }
}
