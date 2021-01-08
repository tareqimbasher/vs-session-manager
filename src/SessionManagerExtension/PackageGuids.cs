using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionManagerExtension
{
    internal class PackageGuids
    {
        public const string PackageGuidString = "b530e711-8043-49b5-b82d-7b94565cfc87";
        public const string PackageCmdSetGuidString = "e5e58af2-0280-4a35-89ff-547922fb526f";
    }

    internal class SessionManagerToolWindowCommandSet
    {
        public const string GuidString = "b8dc732b-7c3f-46a0-ac12-93eff5e58c01";
        public static Guid Guid => new Guid(GuidString);

        public const int ToolbarId = 0x0100;
        public const int SaveCurrentSessionId = 0x0200;
        public const int RestoreSessionId = 0x0300;
        public const int OpenSessionId = 0x0400;
        public const int DeleteSessionId = 0x0500;
        public const int CloseSessionDocumentsId = 0x0600;
    }
}
