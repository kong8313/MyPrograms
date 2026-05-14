using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Encryption;

namespace Confirmit.CATI.Common.Encryption.Fakes
{
    public class StubICatiSecretKeyHasher : ICatiSecretKeyHasher 
    {
        private ICatiSecretKeyHasher _inner;

        public StubICatiSecretKeyHasher()
        {
            _inner = null;
        }

        public ICatiSecretKeyHasher Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate byte[] ComputeHashArrayOfByteStringClientErrorSourceStringDelegate(byte[] secretKey, string companyAlias, ClientErrorSource source, string message);
        public ComputeHashArrayOfByteStringClientErrorSourceStringDelegate ComputeHashArrayOfByteStringClientErrorSourceString;

        byte[] ICatiSecretKeyHasher.ComputeHash(byte[] secretKey, string companyAlias, ClientErrorSource source, string message)
        {


            if (ComputeHashArrayOfByteStringClientErrorSourceString != null)
            {
                return ComputeHashArrayOfByteStringClientErrorSourceString(secretKey, companyAlias, source, message);
            } else if (_inner != null)
            {
                return ((ICatiSecretKeyHasher)_inner).ComputeHash(secretKey, companyAlias, source, message);
            }

            return default(byte[]);
        }

        public delegate bool VerifyComputedHashArrayOfByteArrayOfByteStringClientErrorSourceStringDelegate(byte[] secretKey, byte[] expectedHash, string companyAlias, ClientErrorSource source, string message);
        public VerifyComputedHashArrayOfByteArrayOfByteStringClientErrorSourceStringDelegate VerifyComputedHashArrayOfByteArrayOfByteStringClientErrorSourceString;

        bool ICatiSecretKeyHasher.VerifyComputedHash(byte[] secretKey, byte[] expectedHash, string companyAlias, ClientErrorSource source, string message)
        {


            if (VerifyComputedHashArrayOfByteArrayOfByteStringClientErrorSourceString != null)
            {
                return VerifyComputedHashArrayOfByteArrayOfByteStringClientErrorSourceString(secretKey, expectedHash, companyAlias, source, message);
            } else if (_inner != null)
            {
                return ((ICatiSecretKeyHasher)_inner).VerifyComputedHash(secretKey, expectedHash, companyAlias, source, message);
            }

            return default(bool);
        }

    }
}