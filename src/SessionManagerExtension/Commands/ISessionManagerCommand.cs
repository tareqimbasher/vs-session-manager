using VsShell = Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;

namespace SessionManagerExtension.Commands
{
    public interface ISessionManagerCommand
    {
        Guid CommandSet { get; }
        int CommandId { get; }
        Task ExecuteAsync();
        void OnBeforeQueryCommandStatus(VsShell.OleMenuCommand command);
    }
}
