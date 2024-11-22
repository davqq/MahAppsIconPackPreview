using MahApps.Metro.IconPacks;
using System.Reflection;
using System.Windows.Media;

namespace MahAppsIconPackPreview {
    public static class PackIconControlUtil {
        private static readonly string MAHAPPS_ICONS = "BootstrapIcons|BoxIcons|CircumIcons|Codicons|Coolicons|Core|Entypo|EvaIcons|FeatherIcons|FileIcons|FontAwesome|Fontaudio|Fontisto|ForkAwesome|GameIcons|Ionicons|JamIcons|Lucide|Material|MaterialDesign|MaterialLight|MemoryIcons|Microns|Modern|Octicons|PhosphorIcons|PicolIcons|PixelartIcons|RPGAwesome|RadixIcons|RemixIcon|SimpleIcons|Typicons|Unicons|VaadinIcons|WeatherIcons|Zondicons";
        public static readonly string CSHARP_REGAX_PATTERN = @$"(PackIcon({MAHAPPS_ICONS})+Kind)\.(\w+)";
        public static readonly string XAML_REGAX_PATTERN = $"(<|{{)([a-zA-Z]+):(PackIcon)?({MAHAPPS_ICONS})\\s+(Kind=\"?([a-zA-Z]+)\"?|([a-zA-Z]+))\\s*(\\/>|>|}})";

        public static PackIconControlBase CreateIconFromKindString(string kindString, SolidColorBrush textBrush) {
            string[] parts = kindString.Split('.');
            if (parts.Length != 2) {
                throw new ArgumentException("Invalid kind string format. Expected format: 'TypeName.KindValue'");
            }

            const string preficNameSpace = "MahApps.Metro.IconPacks";

            string enumTypeName = parts[0];
            string kindValue = parts[1];
            var typeName = enumTypeName.Replace("Kind", "");

            string assemblyName = $"{preficNameSpace}.{typeName.Replace("PackIcon", "")}";

            Assembly assembly = Assembly.Load(assemblyName);

            if (assembly == null) {
                return null;
            }

            Type iconType = assembly.GetType($"{preficNameSpace}.{typeName}") ?? throw new ArgumentException($"Icon type not found for {typeName}");

            PackIconControlBase iconInstance = Activator.CreateInstance(iconType) as PackIconControlBase ?? throw new InvalidOperationException($"Could not create an instance of {iconType}");

            Type kindEnumType = assembly.GetType($"{preficNameSpace}.{enumTypeName}") ?? throw new ArgumentException($"Enum type not found for {enumTypeName}");

            object kindEnumValue;
            try {
                kindEnumValue = Enum.Parse(kindEnumType, kindValue);
            } catch (ArgumentException) {
                return null;
            }

            PropertyInfo kindPropertyInfo = iconType.GetProperty("Kind") ?? throw new InvalidOperationException($"Property 'Kind' not found in {iconType}");
            kindPropertyInfo.SetValue(iconInstance, kindEnumValue);

            PropertyInfo foregroundPropertyInfo = iconType.GetProperty("Foreground") ?? throw new InvalidOperationException($"Property 'Foreground' not found in {iconType}");
            foregroundPropertyInfo.SetValue(iconInstance, textBrush);

            return iconInstance;
        }
    }
}
