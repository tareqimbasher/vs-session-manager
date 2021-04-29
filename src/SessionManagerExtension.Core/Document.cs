using System;
using System.Collections.Generic;
using System.Text;

namespace SessionManagerExtension
{
    /// <summary>
    /// Represents a document.
    /// </summary>
    public class Document
    {
        public Document()
        {
        }

        public Document(string name, string fullPath, bool isProjectItem) : this()
        {
            Name = name;
            FullPath = fullPath;
            IsProjectItem = isProjectItem;
        }

        public string? Name { get; set; }
        public string? FullPath { get; set; }
        public bool IsProjectItem { get; set; }
    }
}
