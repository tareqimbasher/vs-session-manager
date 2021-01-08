using EnvDTE80;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SessionManagerExtension.Utils
{
    public interface IIDE
    {
        DTE2 Environment { get; }
        EnvDTE.Document ActiveDocument { get; }
        IEnumerable<EnvDTE.Document> Documents { get; }
        Task<string> GetDocumentWindowPositionsAsync();
        Task<bool> RestoreDocumentWindowsAsync(string positions);
        Task OpenFileAsync(string fullPath);
    }
}
