using System;
using Confirmit.CATI.Core.Security;

namespace Confirmit.CATI.Core.Security.Fakes
{
    public class StubIPasswordHash : IPasswordHash 
    {
        private IPasswordHash _inner;

        public StubIPasswordHash()
        {
            _inner = null;
        }

        public IPasswordHash Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string ComputeHashStringStringDelegate(string password, string salt);
        public ComputeHashStringStringDelegate ComputeHashStringString;

        string IPasswordHash.ComputeHash(string password, string salt)
        {


            if (ComputeHashStringString != null)
            {
                return ComputeHashStringString(password, salt);
            } else if (_inner != null)
            {
                return ((IPasswordHash)_inner).ComputeHash(password, salt);
            }

            return default(string);
        }

        public delegate string GenerateSaltValueDelegate();
        public GenerateSaltValueDelegate GenerateSaltValue;

        string IPasswordHash.GenerateSaltValue()
        {


            if (GenerateSaltValue != null)
            {
                return GenerateSaltValue();
            } else if (_inner != null)
            {
                return ((IPasswordHash)_inner).GenerateSaltValue();
            }

            return default(string);
        }

        public delegate bool ValidateHashStringStringStringDelegate(string password, string salt, string hash);
        public ValidateHashStringStringStringDelegate ValidateHashStringStringString;

        bool IPasswordHash.ValidateHash(string password, string salt, string hash)
        {


            if (ValidateHashStringStringString != null)
            {
                return ValidateHashStringStringString(password, salt, hash);
            } else if (_inner != null)
            {
                return ((IPasswordHash)_inner).ValidateHash(password, salt, hash);
            }

            return default(bool);
        }

        public delegate bool IsLegacyHashStringDelegate(string hash);
        public IsLegacyHashStringDelegate IsLegacyHashString;

        bool IPasswordHash.IsLegacyHash(string hash)
        {


            if (IsLegacyHashString != null)
            {
                return IsLegacyHashString(hash);
            } else if (_inner != null)
            {
                return ((IPasswordHash)_inner).IsLegacyHash(hash);
            }

            return default(bool);
        }

        public delegate bool ValidateLegacyHashInt32StringStringStringDelegate(int personId, string password, string salt, string hash);
        public ValidateLegacyHashInt32StringStringStringDelegate ValidateLegacyHashInt32StringStringString;

        bool IPasswordHash.ValidateLegacyHash(int personId, string password, string salt, string hash)
        {


            if (ValidateLegacyHashInt32StringStringString != null)
            {
                return ValidateLegacyHashInt32StringStringString(personId, password, salt, hash);
            } else if (_inner != null)
            {
                return ((IPasswordHash)_inner).ValidateLegacyHash(personId, password, salt, hash);
            }

            return default(bool);
        }

    }
}