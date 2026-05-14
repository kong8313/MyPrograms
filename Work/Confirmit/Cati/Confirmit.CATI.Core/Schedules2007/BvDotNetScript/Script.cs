using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace BvDotNetScript
{
    public enum DnLanguage
    {
        Invalid,
        JScript_Net
    }

    public class DnSourceFile : IXmlSerializable
    {
        [XmlAttribute("Name")]
        public string Name = null;
        public DnLanguage Language = DnLanguage.Invalid;
        public string Source = null;

        public DnSourceFile(){}

        public DnSourceFile( string name, DnLanguage language, string source )
        {
            Name = name;
            Language = language;
            Source = source;
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToAttribute("Name");
            Name = reader.ReadContentAsString();
            reader.MoveToAttribute("Language");
            switch(reader.ReadContentAsString())
            {
                case "JScript.Net":
                    Language = DnLanguage.JScript_Net;
                    break;
                default:
                    throw new ArgumentException("Cannot deserialize Language enum");
            }
            Source = reader.ReadString();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            switch( Language )
            {
                case DnLanguage.JScript_Net:
                    writer.WriteAttributeString("Language", "JScript.Net");
                    break;
                default:
                    throw new ArgumentException("Cannot serialize Language enum");
            }
            writer.WriteCData(Source);
            //writer.WriteEndElement();
        }
    }

    public class DnReference
    {
        [XmlAttribute("ref")]
        public string Path = null;

        public DnReference() { }

        public DnReference( string path )
        {
            Path = path;
        }
    }

    public class DnParam
    {
        [XmlAttribute("name")]
        public string Name = null;

        public DnParam() { }

        public DnParam(string name)
        {
            Name = name;
        }
    }

    public class DnEntryPoint
    {
        [XmlAttribute("namespace")]
        public string Namespace = null;
        [XmlAttribute("class")]
        public string Class = null;
        [XmlAttribute("method")]
        public string Method = null;
        [XmlArray("Params"), XmlArrayItem("Param")]
        public List<DnParam> Params = new List<DnParam>();

        public DnEntryPoint(){}
    }
    
    [XmlRoot("DotNetScript")]
    public class DnScript
    {
        [XmlArray("SourceFiles"), XmlArrayItem("SourceFile") ]
        public List<DnSourceFile> SourceFiles = new List<DnSourceFile>();

        [XmlArray("References"), XmlArrayItem("Reference")]
        public List<DnReference> References = new List<DnReference>();

        [XmlElement("EntryPoint")]
        public DnEntryPoint EntryPoint = new DnEntryPoint();
    }
}
