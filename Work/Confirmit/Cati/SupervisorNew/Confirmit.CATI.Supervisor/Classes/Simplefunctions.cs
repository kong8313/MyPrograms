using System;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class Simplefunctions
    {
        private static Simplefunctions m_Instance;

        //---------------------------------------------------------------------------
        protected Simplefunctions()
        {
        }

        //---------------------------------------------------------------------------
        public static Simplefunctions Instance()
        {
            if (m_Instance == null)
            {
                m_Instance = new Simplefunctions();
            }

            return (m_Instance);
        }

        //---------------------------------------------------------------------------
        public string prepareErrorString(string description)
        {
            string res = description;
            res = res.Replace(@"\", @"\\");
            res = res.Replace(new string(new char[] {'\r', '\n'}), @"\n");
            res = res.Replace(new string(new char[] {'\n'}), @"\n");
            res = res.Replace("\"", "\\\"");
            res = res.Replace( "'", "\\'" );

            return res;
        }

        //---------------------------------------------------------------------------
        public string prepareErrorMessage(string description)
        {
            string res = prepareErrorString(description);
            return ("<script>alert(\"" + res + "\")</script>");
        }

        //---------------------------------------------------------------------------
        public string prepareErrorMessage(Exception e)
        {
            //return ( prepareErrorMessage( e.ToString( ) ) );
            return (prepareErrorMessage(e.Message));
        }

        //---------------------------------------------------------------------------
        public string prepareErrorMessageEx(Exception e)
        {
            string message = e.Message;
            if (e.InnerException != null)
            {
                if (e.InnerException.Message != "")
                    message += "\n" + "Description: " + e.InnerException.Message;
                if (!string.IsNullOrEmpty(e.InnerException.Source))
                    message += "\n" + "Source: " + e.InnerException.Source;

                System.Runtime.InteropServices.COMException comex = e.InnerException as System.Runtime.InteropServices.COMException;
                if (comex != null)
                    message += "\n" + "Code: " + comex.ErrorCode;
            }
            else
            {
                if (!string.IsNullOrEmpty(e.Source))
                    message += "\n" + "Source: " + e.Source;
                System.Runtime.InteropServices.COMException comex = e as System.Runtime.InteropServices.COMException;
                if (comex != null)
                    message += "\n" + "Code: " + comex.ErrorCode;
            }
            return (prepareErrorMessage(message));
        }

        public string prepareErrorMessageHTML(Exception e)
        {
            string message = e.Message;
            if (e.InnerException != null)
            {
                if (e.InnerException.Message != "")
                    message += "<br/>" + "Description: " + e.InnerException.Message;
                if (!string.IsNullOrEmpty(e.InnerException.Source))
                    message += "<br/>" + "Source: " + e.InnerException.Source;

                System.Runtime.InteropServices.COMException comex = e.InnerException as System.Runtime.InteropServices.COMException;
                if (comex != null)
                    message += "<br/>" + "Code: " + comex.ErrorCode;
            }
            else
            {
                if (!string.IsNullOrEmpty(e.Source))
                    message += "<br/>" + "Source: " + e.Source;
                System.Runtime.InteropServices.COMException comex = e as System.Runtime.InteropServices.COMException;
                if (comex != null)
                    message += "<br/>" + "Code: " + comex.ErrorCode;
            }
            message = message.Replace("\r\n", "</br>");
            message = message.Replace("\n\r", "</br>");
            message = message.Replace("\n", "</br>");
            message = message.Replace("\r", "</br>");
            return message;
        }

        //---------------------------------------------------------------------------
        public string noQuotString(string value)
        {
            string res = value;
            res = res.Replace("\"", "&quot;");
            res = res.Replace("'", "&#39;");
            res = res.Replace("<", "&lt;");
            res = res.Replace(">", "&gt;");
            return (res);
        }

        //---------------------------------------------------------------------------
        public string noQuotnoAmpString(string value)
        {
            string res = value;
            res = res.Replace("&", "&amp;");
            res = res.Replace("\"", "&quot;");
            res = res.Replace("'", "&#39;");
            res = res.Replace("<", "&lt;");
            res = res.Replace(">", "&gt;");
            return (res);
        }
        //---------------------------------------------------------------------------
        public string noQuotStringForScript(string value)
        {
            string res = value;
            res = res.Replace("\\", "\\\\");
            res = res.Replace("\"", "\\\"");
            res = res.Replace("'", "\\'");
            string sTemp = new string(new char[] { Convert.ToChar(0xD), Convert.ToChar(0xA) });
            res = res.Replace(sTemp, "\\n");
            res = res.Replace(new string(new char[] { '\n' }), @"\n");
            return (res);
        }

        //---------------------------------------------------------------------------
        public string noQuotHTMLString(string value)
        {
            string res = value.Replace("\"", "\\\"");
            res = res.Replace("'", "\\'");
            return (res);
        }

        //---------------------------------------------------------------------------
        public string quotString(string value)
        {
            string res = value;
            res = res.Replace("&quot;", "\"");
            res = res.Replace("&#39;", "'");
            res = res.Replace("&lt;", "<");
            res = res.Replace("&gt;", ">");
            return (res);
        }
    }
}