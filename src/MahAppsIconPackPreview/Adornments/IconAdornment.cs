using MahApps.Metro.IconPacks;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MaterialIconPreview.Adornments {
    internal sealed class IconAdornment : Image {
        private readonly ITextView _view;
        private static readonly SolidColorBrush _captionTextBrush = (SolidColorBrush)Application.Current.Resources[VsBrushes.CaptionTextKey];
        private static readonly double _captionFontSize = (double)Application.Current.Resources[VsFonts.CaptionFontSizeKey];
        private static readonly Style _toolTipStyle = (Style)Application.Current.Resources[VsResourceKeys.LargeToolTipStyleKey];

        internal IconAdornment(string iconName, ITextView view) {
            ThreadHelper.ThrowIfNotOnUIThread();
            _view = view;

            PackIconControlBase icon = PackIconControlUtil.CreateIconFromKindString(iconName, _captionTextBrush);

            if (icon == null) {
                return;
            }

            Stretch = Stretch.Fill;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Width = GetFontSize() + 2;
            Height = Width;
            Cursor = Cursors.Arrow;

            Source = icon.ToImageSource(Width, Height, renderSmall: true);

            var grid = new StackPanel();

            var toolTipImage = new Image {
                Source = icon.ToImageSource(),
                Stretch = Stretch.Uniform,
            };

            grid.Children.Add(toolTipImage);

            var textBlock = new TextBlock() {
                Text = $"{iconName} 32x32px",
                FontSize = _captionFontSize
            };

            grid.Children.Add(textBlock);


            ToolTip = new ToolTip {
                Style = _toolTipStyle,
                Content = grid,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse
            };

            ToolTipService.SetInitialShowDelay(this, 0);
        }
        
        protected override void OnMouseUp(MouseButtonEventArgs e) {
            _view.Caret.MoveToNextCaretPosition();
            VS.Commands.ExecuteAsync("Edit.ListMembers").FireAndForget();
            e.Handled = true;
        }

        private static int GetFontSize() {
            ThreadHelper.ThrowIfNotOnUIThread();

            try {
                IVsFontAndColorStorage storage = (IVsFontAndColorStorage)Package.GetGlobalService(typeof(IVsFontAndColorStorage));
                Guid guid = new("A27B4E24-A735-4d1d-B8E7-9716E1E3D8E0");
                if (storage != null && storage.OpenCategory(ref guid, (uint)(__FCSTORAGEFLAGS.FCSF_READONLY | __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS)) == VSConstants.S_OK) {
                    LOGFONTW[] Fnt = [new()];
                    FontInfo[] Info = [new()];
                    storage.GetFont(Fnt, Info);
                    return Info[0].wPointSize;
                }

            } catch { }

            return 0;
        }
    }
}

public static class PackIconMaterialExtensions {
    public static ImageSource ToImageSource(this PackIconControlBase packIcon, double width = 32, double height = 32, bool renderSmall = false) {
        // Create a DrawingGroup to hold the icon
        DrawingGroup drawing = new DrawingGroup();
        using (DrawingContext context = drawing.Open()) {
            Geometry geometry = Geometry.Parse(packIcon.Data);
            context.DrawGeometry(packIcon.Foreground, null, geometry);
        }

        // Use DrawingImage to convert the DrawingGroup to an ImageSource
        DrawingImage drawingImage = new DrawingImage(drawing);
        drawingImage.Freeze();

        // Create a RenderTargetBitmap to render the DrawingImage
        RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)width, (int)height, renderSmall ? 74 : 94, renderSmall ? 74 : 94, PixelFormats.Pbgra32);
        DrawingVisual visual = new DrawingVisual();
        using (DrawingContext context = visual.RenderOpen()) {
            context.DrawImage(drawingImage, new Rect(0, 0, width, height));
        }
        targetBitmap.Render(visual);
        targetBitmap.Freeze();

        return targetBitmap;
    }
}