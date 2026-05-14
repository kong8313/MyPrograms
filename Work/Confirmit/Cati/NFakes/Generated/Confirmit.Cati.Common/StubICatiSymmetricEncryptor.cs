using System;
using Confirmit.CATI.Common.Encryption;

namespace Confirmit.CATI.Common.Encryption.Fakes
{
    public class StubICatiSymmetricEncryptor : ICatiSymmetricEncryptor 
    {
        private ICatiSymmetricEncryptor _inner;

        public StubICatiSymmetricEncryptor()
        {
            _inner = null;
        }

        public ICatiSymmetricEncryptor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DisposeDelegate();
        public DisposeDelegate Dispose;

        void IDisposable.Dispose()
        {

            if (Dispose != null)
            {
                Dispose();
            } else if (_inner != null)
            {
                ((IDisposable)_inner).Dispose();
            }
        }

        public delegate string EncryptStringStringDelegate(string text);
        public EncryptStringStringDelegate EncryptStringString;

        string ICatiSymmetricEncryptor.EncryptString(string text)
        {


            if (EncryptStringString != null)
            {
                return EncryptStringString(text);
            } else if (_inner != null)
            {
                return ((ICatiSymmetricEncryptor)_inner).EncryptString(text);
            }

            return default(string);
        }

        public delegate string DecryptStringStringDelegate(string cipherText);
        public DecryptStringStringDelegate DecryptStringString;

        string ICatiSymmetricEncryptor.DecryptString(string cipherText)
        {


            if (DecryptStringString != null)
            {
                return DecryptStringString(cipherText);
            } else if (_inner != null)
            {
                return ((ICatiSymmetricEncryptor)_inner).DecryptString(cipherText);
            }

            return default(string);
        }

        public delegate void ClearDelegate();
        public ClearDelegate Clear;

        void ICatiSymmetricEncryptor.Clear()
        {

            if (Clear != null)
            {
                Clear();
            } else if (_inner != null)
            {
                ((ICatiSymmetricEncryptor)_inner).Clear();
            }
        }

        private byte[] _Key;
        public Func<byte[]> KeyGet;
        public Action<byte[]> KeySetArrayOfByte;

        byte[] ICatiSymmetricEncryptor.Key
        {
            get
            {
                if (KeyGet != null)
                {
                    return KeyGet();
                } else if (_inner != null)
                {
                    return ((ICatiSymmetricEncryptor)_inner).Key;
                }

                if (KeySetArrayOfByte == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Key;
                }

                return default(byte[]);
            }

            set
            {
                if (KeySetArrayOfByte != null)
                {
                    KeySetArrayOfByte(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiSymmetricEncryptor)_inner).Key = value;
                    return;
                }

                if (KeyGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Key = value;
                }

            }
        }

        private byte[] _IV;
        public Func<byte[]> IVGet;
        public Action<byte[]> IVSetArrayOfByte;

        byte[] ICatiSymmetricEncryptor.IV
        {
            get
            {
                if (IVGet != null)
                {
                    return IVGet();
                } else if (_inner != null)
                {
                    return ((ICatiSymmetricEncryptor)_inner).IV;
                }

                if (IVSetArrayOfByte == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IV;
                }

                return default(byte[]);
            }

            set
            {
                if (IVSetArrayOfByte != null)
                {
                    IVSetArrayOfByte(value);
                    return;
                } else if (_inner != null)
                {
                    ((ICatiSymmetricEncryptor)_inner).IV = value;
                    return;
                }

                if (IVGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IV = value;
                }

            }
        }

    }
}