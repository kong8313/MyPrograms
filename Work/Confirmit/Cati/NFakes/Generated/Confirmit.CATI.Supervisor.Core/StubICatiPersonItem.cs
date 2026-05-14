using System;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation.Fakes
{
    public class StubICatiPersonItem : ICatiPersonItem 
    {
        private ICatiPersonItem _inner;

        public StubICatiPersonItem()
        {
            _inner = null;
        }

        public ICatiPersonItem Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitDelegate();
        public InitDelegate Init;

        void ICatiPersonItem.Init()
        {

            if (Init != null)
            {
                Init();
            } else if (_inner != null)
            {
                ((ICatiPersonItem)_inner).Init();
            }
        }

        private int _Id;
        public Func<int> IdGet;
        public Action<int> IdSetInt32;

        int ICatiPersonItem.Id
        {
            get
            {
                if (IdGet != null)
                {
                    return IdGet();
                } else if (_inner != null)
                {
                    return ((ICatiPersonItem)_inner).Id;
                }

                if (IdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Id;
                }

                return default(int);
            }

        }

        private string _Name;
        public Func<string> NameGet;
        public Action<string> NameSetString;

        string ICatiPersonItem.Name
        {
            get
            {
                if (NameGet != null)
                {
                    return NameGet();
                } else if (_inner != null)
                {
                    return ((ICatiPersonItem)_inner).Name;
                }

                if (NameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Name;
                }

                return default(string);
            }

            set
            {
                if (NameSetString != null)
                {
                    NameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiPersonItem)_inner).Name = value;
                    return;
                }

                if (NameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Name = value;
                }

            }
        }

        private string _Description;
        public Func<string> DescriptionGet;
        public Action<string> DescriptionSetString;

        string ICatiPersonItem.Description
        {
            get
            {
                if (DescriptionGet != null)
                {
                    return DescriptionGet();
                } else if (_inner != null)
                {
                    return ((ICatiPersonItem)_inner).Description;
                }

                if (DescriptionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Description;
                }

                return default(string);
            }

            set
            {
                if (DescriptionSetString != null)
                {
                    DescriptionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiPersonItem)_inner).Description = value;
                    return;
                }

                if (DescriptionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Description = value;
                }

            }
        }

    }
}