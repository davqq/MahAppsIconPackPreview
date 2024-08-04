global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;

using System.Runtime.InteropServices;
using System.Threading;

namespace MaterialIconPreview {
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    //[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(MaterialIconPreviewPackage.PackageGuidString)]
    public sealed class MaterialIconPreviewPackage : ToolkitPackage {
        public const string PackageGuidString = "ef97b277-55bb-4199-918c-fa9b3fe7faa2";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
    }
}
