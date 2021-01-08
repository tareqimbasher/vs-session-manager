using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionManagerExtension.Utils
{
    public class PackageAccessor
    {
        public PackageAccessor(SessionManagerPackage package)
        {
            Package = package;
        }

        public SessionManagerPackage Package { get; }
    }
}
