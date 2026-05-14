using System;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation.Fakes
{
    public class StubIPasswordSaver : IPasswordSaver 
    {
        private IPasswordSaver _inner;

        public StubIPasswordSaver()
        {
            _inner = null;
        }

        public IPasswordSaver Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SaveInt32StringDelegate(int personId, string password);
        public SaveInt32StringDelegate SaveInt32String;

        void IPasswordSaver.Save(int personId, string password)
        {

            if (SaveInt32String != null)
            {
                SaveInt32String(personId, password);
            } else if (_inner != null)
            {
                ((IPasswordSaver)_inner).Save(personId, password);
            }
        }

        public delegate void SaveBvPersonEntityStringDelegate(BvPersonEntity person, string password);
        public SaveBvPersonEntityStringDelegate SaveBvPersonEntityString;

        void IPasswordSaver.Save(BvPersonEntity person, string password)
        {

            if (SaveBvPersonEntityString != null)
            {
                SaveBvPersonEntityString(person, password);
            } else if (_inner != null)
            {
                ((IPasswordSaver)_inner).Save(person, password);
            }
        }

    }
}