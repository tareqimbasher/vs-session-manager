using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SessionManagerExtension.Utils
{
    public class IDE : IIDE
    {
        private readonly IVsUIShellDocumentWindowMgr _vsDocumentWindowMgr;
        private readonly SessionManagerPackage _package;

        public IDE(PackageAccessor packageAccessor, DTE2 environment, IVsUIShellDocumentWindowMgr vsDocumentWindowMgr)
        {
            _package = packageAccessor.Package;
            Environment = environment;
            _vsDocumentWindowMgr = vsDocumentWindowMgr;
        }

        public DTE2 Environment { get; }
        public EnvDTE.Document ActiveDocument => Environment.ActiveDocument;
        public IEnumerable<EnvDTE.Document> Documents => Environment.Documents.Cast<EnvDTE.Document>();


        public async Task<string> GetDocumentWindowPositionsAsync()
        {
            using (var stream = new VsOleStream())
            {
                await _package.JoinableTaskFactory.SwitchToMainThreadAsync();
                var result = _vsDocumentWindowMgr.SaveDocumentWindowPositions(0, stream);
                if (result != VSConstants.S_OK)
                {
                    return null;
                }

                stream.Seek(0, SeekOrigin.Begin);

                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public async Task<bool> RestoreDocumentWindowsAsync(string positions)
        {
            using (var stream = new VsOleStream())
            {
                var bytes = Convert.FromBase64String(positions);
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);

                await _package.JoinableTaskFactory.SwitchToMainThreadAsync();
                var hr = _vsDocumentWindowMgr.ReopenDocumentWindows(stream);
                if (hr != VSConstants.S_OK)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task OpenFileAsync(string fullPath)
        {
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync();
            Environment.ItemOperations.OpenFile(fullPath);
        }
    }
}
