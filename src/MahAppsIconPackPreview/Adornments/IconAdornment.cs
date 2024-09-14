using MahApps.Metro.IconPacks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahAppsIconPackPreview.Adornments {
    internal sealed class IconAdornment {
        private static readonly SolidColorBrush _captionTextBrush = (SolidColorBrush)Application.Current.Resources[VsBrushes.CaptionTextKey];
        private static readonly double _captionFontSize = (double)Application.Current.Resources[VsFonts.CaptionFontSizeKey];
        private static readonly Style _toolTipStyle = (Style)Application.Current.Resources[VsResourceKeys.LargeToolTipStyleKey];

        internal IconAdornment() {
            ThreadHelper.ThrowIfNotOnUIThread();
        }

        public static PackIconControlBase CreateAdornment(string iconName) {
            PackIconControlBase icon = PackIconControlUtil.CreateIconFromKindString(iconName, _captionTextBrush);

            if (icon == null) {
                return null;
            }

            icon.Width = GetFontSize() + 2;
            icon.Height = icon.Width;
            icon.Margin = new Thickness(0, -5, 2, 0);
            icon.VerticalAlignment = VerticalAlignment.Center;
            icon.HorizontalAlignment = HorizontalAlignment.Center;

            var grid = new StackPanel();

            var toolTipImage = PackIconControlUtil.CreateIconFromKindString(iconName, _captionTextBrush);

            toolTipImage.Width = 32;
            toolTipImage.Height = toolTipImage.Width;
            toolTipImage.VerticalAlignment = VerticalAlignment.Center;
            toolTipImage.HorizontalAlignment = HorizontalAlignment.Center;

            grid.Children.Add(toolTipImage);

            var textBlock = new TextBlock() {
                Text = $"{toolTipImage.Width}x{toolTipImage.Height}px",
                Margin = new Thickness(0, 3, 0, 0),
                FontSize = _captionFontSize
            };

            grid.Children.Add(textBlock);


            icon.ToolTip = new ToolTip {
                Style = _toolTipStyle,
                Content = grid,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse
            };

            ToolTipService.SetInitialShowDelay(icon, 0);

            return icon;
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
