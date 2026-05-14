using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Confirmit.CATI.Common.Validators
{
    public class InputParameterValidator : IInputParameterValidator
    {
        private const string _emailMask = @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$";
        private const string _invalidSymbols = @"[<>&';\x00-\x1F\x7F-\x9F]";
        private const string _validStringMask = @"^[^<>&';\x00-\x1F\x7F-\x9F]*$";
        private const string _validQuestionIdMask = @"^[^\d\W]\w*$";
        private readonly string[] _reservedWords =
        {
            "con", "r", "s", "rid", "interviewer", "c", "page", "projectid", "env", "state"
        };

        public string InvalidSymbols
        {
            get { return _invalidSymbols; }
        }

        public string ValidStringMask
        {
            get { return _validStringMask; }
        }

        /// <summary>
        /// Validates an email string.
        /// Allows letter or number to be first
        /// Allows last part of domain name to 2-6 lettres
        /// Disallows underline to be the first letter
        /// Disallows dot to be the first letter
        /// Allows dot to be the middle of name 
        /// Allow domain name to contain several parts
        /// </summary>
        /// <param name="emailString">String to validate</param>
        /// <returns>If specified string is valid email</returns>
        public bool IsValidEmail(string emailString)
        {
            if (!String.IsNullOrEmpty(emailString))
            {
                return Regex.Match(emailString, _emailMask).Success;
            }

            return false;
        }

        public bool IsValidQuestionId(string questionId)
        {
            if (String.IsNullOrEmpty(questionId) || Regex.Match(questionId, _validQuestionIdMask).Success == false ||
                _reservedWords.Contains(questionId))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates a text string.
        /// Disallows letters that can be used for xss attacks
        /// </summary>
        /// <param name="value">String to validate</param>
        /// <returns>If specified string is valid string</returns>
        public bool IsValid(string value)
        {
            if (value != null)
            {
                return Regex.Match(value, ValidStringMask).Success;
            }

            return false;
        }
    }
}