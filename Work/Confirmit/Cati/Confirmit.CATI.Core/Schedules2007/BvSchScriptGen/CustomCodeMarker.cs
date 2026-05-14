using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Confirmit.CATI.Core.Schedules2007.BvSchScriptGen
{
    internal class CustomCodeMarker
    {
        private const string BaseMarker = @"//##CUSTOM_CODE_DESCRIPTION: ";
        
        internal static string FormatMarker(string description )
        {
            return Environment.NewLine + BaseMarker + description + Environment.NewLine;
        }

        internal string Description{ get; private set;}

        internal int StartLine { get; private set; }

        internal static CustomCodeMarker Search(string source, int lineNumber)
        {
            int     CurLineNumber = 1;

            string  LastMarker = null;
            int     LastMarkerLine = 0;

            int BaseMarkerLength = BaseMarker.Length;

            StringReader sr = new StringReader(source);

            string CurLine = sr.ReadLine();
            while (CurLine != null)
            {
                if (CurLineNumber == lineNumber)
                    break;

                if (CurLine.Length >= BaseMarkerLength && CurLine.Substring(0, BaseMarkerLength) == BaseMarker)
                {
                    LastMarker = CurLine;
                    LastMarkerLine = CurLineNumber;
                }

                CurLineNumber++;
                CurLine = sr.ReadLine();
            }

            if( LastMarker == null )
                return null;

            return new CustomCodeMarker(LastMarker.Substring(BaseMarkerLength), LastMarkerLine);
        }

        private CustomCodeMarker(string description, int startLine)
        {
            Description = description;
            StartLine = startLine;
        }
    }
}
