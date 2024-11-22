using MahApps.Metro.IconPacks;
using System;
using System.Windows.Media;

namespace MahAppsIconPackPreview.Test {
    public class PackIconControlUtilTest {
        public class PackIconControlUtilTests {
            [Theory]
            [InlineData(PackIconMaterialKind.Pause, typeof(PackIconMaterial))]
            [InlineData(PackIconFontAwesomeKind.FlagSolid, typeof(PackIconFontAwesome))]
            [InlineData(PackIconModernKind.Monitor, typeof(PackIconModern))]
            public void CreateIconFromKindString_ValidInput_ReturnsExpectedIcon(object kind, Type expectedType) {
                // Arrange
                SolidColorBrush brush = new(Colors.Red);

                // Act
                PackIconControlBase icon = PackIconControlUtil.CreateIconFromKindString($"{expectedType.Name}.{kind}", brush);

                // Assert
                Assert.NotNull(icon);
                Assert.IsType(expectedType, icon);
                Assert.Equal(kind.ToString()?.Split('.')[1], expectedType.GetProperty("Kind")?.GetValue(icon));
                Assert.Equal(brush, icon.Foreground);
            }

            [Theory]
            [InlineData("InvalidKindString")]
            [InlineData("PackIconMaterialKind.InvalidEnum")]
            public void CreateIconFromKindString_InvalidInput_ThrowsArgumentException(string kindString) {
                // Arrange
                SolidColorBrush brush = new(Colors.Red);

                // Act & Assert
                Assert.Throws<ArgumentException>(() => PackIconControlUtil.CreateIconFromKindString(kindString, brush));
            }

            [Fact]
            public void CreateIconFromKindString_AssemblyNotFound_ReturnsNull() {
                // Arrange
                string kindString = "PackIconNonExistentKind.Pause";
                SolidColorBrush brush = new(Colors.Red);

                // Act
                PackIconControlBase icon = PackIconControlUtil.CreateIconFromKindString(kindString, brush);

                // Assert
                Assert.Null(icon);
            }

            [Fact]
            public void CreateIconFromKindString_InvalidEnumValue_ReturnsNull() {
                // Arrange
                string kindString = "PackIconMaterialKind.NonExistentEnum";
                SolidColorBrush brush = new(Colors.Red);

                // Act
                PackIconControlBase icon = PackIconControlUtil.CreateIconFromKindString(kindString, brush);

                // Assert
                Assert.Null(icon);
            }
        }
    }
}