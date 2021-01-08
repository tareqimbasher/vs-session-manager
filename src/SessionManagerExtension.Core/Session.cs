using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace SessionManagerExtension
{
    public class Session : NotifiesPropertyChanged
    {
        private Guid _id;
        private string? _name;
        private ObservableCollection<Document> _documents;
        private DateTime _createdDate;
        private DateTime _lastModifiedDate;

        public Session()
        {
            Id = Guid.NewGuid();
            _documents = new ObservableCollection<Document>();
            _createdDate = DateTime.Now;
            _lastModifiedDate = DateTime.Now;
        }

        public Session(string name) : this()
        {
            Name = name;
        }

        public Guid Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        public string? Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public ObservableCollection<Document> Documents
        {
            get => _documents;
            set => SetField(ref _documents, value);
        }

        public string? DocumentPositions { get; set; }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetField(ref _createdDate, value);
        }

        public DateTime LastModifiedDate
        {
            get => _lastModifiedDate;
            set => SetField(ref _lastModifiedDate, value);
        }


        public Document AddDocument(string name, string fullPath, bool isProjectItem)
        {
            var document = new Document(name, fullPath, isProjectItem);
            Documents.Add(document);
            return document;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = obj.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return (obj is Session session && Id.Equals(session.Id) == true);
        }

        public override int GetHashCode()
        {
            if (Id == null)
            {
                return 0;
            }

            return Id.GetHashCode();
        }

        public static bool operator ==(Session? left, Session? right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            if (Equals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Session? left, Session? right)
        {
            return !(left == right);
        }
    }
}
