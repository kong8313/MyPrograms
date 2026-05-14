using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DotNetXmlToAdocConverter.Interfaces;

namespace DotNetXmlToAdocConverter
{
    public class XmlEngine : IXmlEngine
    {
        private readonly XmlDocument _xmlDocument;

        public XmlEngine(string inputData)
        {
            _xmlDocument = new XmlDocument
            {
                PreserveWhitespace = true
            };

            _xmlDocument.Load(inputData);
        }

        public List<ClassInfo> GetClasses()
        {
            var classInfo = new ClassInfo();
            var result = new List<ClassInfo>();

            var members = _xmlDocument.SelectNodes("//doc/members");
            if (members == null)
            {
                throw new Exception("Xml file with help information is corrupt. No members node.");
            }

            foreach (XmlNode member in members[0].ChildNodes)
            {
                if (member.Name != "member")
                    continue;

                if (member.Attributes?["name"] == null)
                {
                    throw new Exception("Xml file with help information is corrupt. No attribute name for one of members node.");
                }

                var fullName = member.Attributes["name"].Value;

                if (fullName.StartsWith("T"))
                {
                    if (classInfo.Name != null)
                    {
                        result.Add(classInfo);
                        classInfo = new ClassInfo();
                    }

                    classInfo.Name = GetClassNameFromFullName(fullName);
                    classInfo.Description = GetDescription(member, fullName);
                    classInfo.Namespace = GetNamespaceFromFullName(fullName);
                }

                else if (fullName.StartsWith("M"))
                {
                    List<PropertyInfo> parameters = GetParameters(member, fullName);

                    if (fullName.Contains("#ctor"))
                    {
                        var constructInfo = new ConstructorInfo
                        {
                            Name = GetConstructorName(fullName, classInfo.Name, parameters),
                            Description = GetDescription(member, fullName),
                            Parameters = parameters
                        };

                        classInfo.Constructors.Add(constructInfo);
                    }
                    else
                    {
                        var methodInfo = new MethodInfo
                        {
                            Name = GetMethodName(fullName, parameters),
                            Description = GetDescription(member, fullName),
                            Parameters = parameters,
                            Returns = GetReturns(member)
                        };

                        classInfo.Methods.Add(methodInfo);
                    }
                }

                else
                {
                    var propertyInfo = new PropertyInfo
                    {
                        Name = GetClassNameFromFullName(fullName), 
                        Description = GetDescription(member, fullName)
                    };

                    classInfo.Properties.Add(propertyInfo);
                }
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        private string GetReturns(XmlNode member)
        {
            foreach (XmlNode child in member.ChildNodes)
            {
                if (child.Name != "returns")
                    continue;

                return child.InnerText.Trim('\r', '\n', ' ');
            }

            return string.Empty;
        }

        private string GetConstructorName(string fullName, string classInfoName, List<PropertyInfo> parameters)
        {
            return GetMethodName(fullName.Replace("#ctor", classInfoName), parameters);
        }

        private string GetMethodName(string fullName, List<PropertyInfo> parameters)
        {
            if (!fullName.Contains('('))
            {
                return GetClassNameFromFullName(fullName) + "()";
            }

            int startPos = fullName.IndexOf('(') + 1;
            int endPos = fullName.IndexOf(')');
            string paramStr = fullName.Substring(startPos, endPos - startPos);

            string[] paramsArr = paramStr.Split(',');

            if (parameters.Count != paramsArr.Length)
            {
                throw new Exception($"Xml file with help information is corrupt. Not all parameters of {fullName} have description");
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                paramsArr[i] = SimplifyType(paramsArr[i]) + " " + parameters[i].Name;
            }

            return GetClassNameFromFullName(fullName.Substring(0, startPos - 1)) + "(" + string.Join(", ", paramsArr) + ")";
        }

        private string SimplifyType(string longType)
        {
            string type = longType
                .Replace("System.String", "string")
                .Replace("System.Int32", "int")
                .Replace("System.Boolean", "bool")
                .Replace("System.DateTime", "DateTime")
                .Replace("``0" , "T")
                .Replace("``1", "<T>");

            type = SimplifyNullable(type);
            type = SimplifyList(type);
            type = ChangeOurTypeToLink(type);

            return type;
        }

        private string SimplifyNullable(string type)
        {
            if (type.StartsWith("System.Nullable"))
            {
                type = type.Replace("System.Nullable{", "").Replace("}", "?");
            }

            return type;
        }

        private string SimplifyList(string type)
        {
            if (type.StartsWith("System.Collections.Generic.List"))
            {
                type = type.Replace("System.Collections.Generic.List{", " List<").Replace("}", ">");
            }

            return type;
        }

        private string ChangeOurTypeToLink(string type)
        {
            if (type.StartsWith("Confirmit.CATI.REST"))
            {
                var parts = type.Split('.');
                var nameSpace = parts[parts.Length - 2];
                var className = parts[parts.Length - 1];
                type = $"xref:{nameSpace}/{className}.adoc[{className}]";
            }

            return type;
        }

        private List<PropertyInfo> GetParameters(XmlNode member, string fullName)
        {
            var result = new List<PropertyInfo>();
            foreach (XmlNode child in member.ChildNodes)
            {
                if (child.Name != "param")
                    continue;

                if (child.Attributes?["name"] == null)
                {
                    throw new Exception("Xml file with help information is corrupt. No attribute name for one of param node.");
                }

                var name = child.Attributes["name"].Value;
                var description = child.InnerText.Trim('\r', '\n', ' ');

                if (string.IsNullOrEmpty(description))
                {
                    throw new Exception($"No comment for the parameter '{name}' in the method '{fullName}'");
                }

                result.Add(new PropertyInfo { Name = name,  Description = description });
            }

            return result;
        }

        private string GetDescription(XmlNode member, string fullName)
        {
            foreach(XmlNode child in member.ChildNodes)
            {
                if (child.Name != "summary")
                    continue;

                return child.InnerText.Trim('\r', '\n', ' ');
            }

            throw new Exception($"Xml file with help information is corrupt. No summary for {fullName}.");
        }

        private string GetClassNameFromFullName(string fullName)
        {
            var nameParts = GetNameParts(fullName);

            return nameParts[nameParts.Length - 1].Replace("``1", "<T>");
        }

        private string GetNamespaceFromFullName(string fullName)
        {
            var nameParts = GetNameParts(fullName);

            return nameParts[nameParts.Length - 2];
        }

        private string[] GetNameParts(string fullName)
        {
            int pos = fullName.IndexOf("(", StringComparison.Ordinal);
            var  fullnameWithoutParameters = pos > -1 ? fullName.Substring(0, pos) : fullName;

            return fullnameWithoutParameters.Split('.');
        }
    }
}