using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    internal class VoiceXmlProcessor
    {
        private IEnumerable<KeyValuePair<string, string>> _variables;

        public IvrSimulatedResponse GenerateResponse(string voiceXml)
        {
            var doc = XDocument.Parse(voiceXml);

            RemoveNamespaces(doc);

            var formTag = doc.XPathSelectElement("vxml/form");

            if (formTag == null)
            {
                throw new Exception("No valid voice xml found.");
            }

            // process vars if exist
            _variables = formTag.Descendants("var").Select(
                item => new KeyValuePair<string, string>(
                    item.Attribute("name").Value,
                    item.Attribute("expr").Value));

            var recordTag = doc.XPathSelectElement("vxml/form/record"); // we treat this as an opentext question indicator

            if (recordTag != null)
            {
                return CreateResponse(ProcessOpentext(recordTag));
            }

            var blockTag = doc.XPathSelectElement("vxml/form/block"); // we treat this as an info question indicator

            if (blockTag != null)
            {
                return CreateResponse(null);
            }

            var fieldTag = doc.XPathSelectElement("vxml/form/field"); // this tag includes numeric and single, so we need to chek "digits" in the type

            if (fieldTag == null) throw new Exception("No valid types of the voice xml found.");

            if (fieldTag.Attribute("type") != null)
            {
                if (fieldTag.Attribute("type").Value.Contains("digits"))
                {
                    return CreateResponse(ProcessNumeric(fieldTag));
                }
            }

            var oneOfTag = doc.XPathSelectElements("vxml/form/field/grammar/rule/one-of").SingleOrDefault(); // this tag exist only for single in our examples

            if (oneOfTag != null)
            {
                var oneOfTagItems = oneOfTag.Descendants().ToList();

                if (oneOfTagItems.Any())
                {
                    return CreateResponse(ProcessSingle(fieldTag.Attribute("name").Value, oneOfTagItems));
                }
            }

            throw new Exception("No valid types of the voice xml found.");
        }

        private void RemoveNamespaces(XDocument doc)
        {
            foreach (var e in doc.Root.DescendantsAndSelf())
            {
                if (e.Name.Namespace != XNamespace.None)
                {
                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
                }

                e.ReplaceAttributes(
                    e.Attributes()
                        .Select(
                            a =>
                                a.IsNamespaceDeclaration
                                    ? null
                                    : a.Name.Namespace != XNamespace.None
                                        ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value)
                                        : a));
            }
        }

        private IvrSimulatedResponse CreateResponse(KeyValuePair<string, string>? response)
        {
            return new IvrSimulatedResponse
            {
                Variables = _variables,
                SimulatedUserInput = response
            };
        }

        private KeyValuePair<string, string> ProcessNumeric(XElement fieldElement)
        {
            var name = fieldElement.Attribute("name").Value;
            var type = fieldElement.Attribute("type").Value;

            var matchMinlength = Regex.Match(type, @"minlength\=(?<minlength>\d+)");
            var matchMaxlength = Regex.Match(type, @"maxlength\=(?<maxlength>\d+)");

            var min = 0;
            var max = 0;

            if (matchMinlength.Success)
            {
                min = int.Parse(matchMinlength.Groups["minlength"].Value);
            }

            if (matchMaxlength.Success)
            {
                max = int.Parse(matchMaxlength.Groups["maxlength"].Value);
            }

            return new KeyValuePair<string, string>(name, GenerateDigits(min, max));
        }

        private KeyValuePair<string, string> ProcessSingle(string name, IList<XElement> oneOfElements)
        {
            var index = new Random().Next(oneOfElements.Count);

            return new KeyValuePair<string, string>(name, oneOfElements.ElementAt(index).Value);
        }

        private KeyValuePair<string, string> ProcessOpentext(XElement recordTag)
        {
            return new KeyValuePair<string, string>(recordTag.Attribute("name").Value, "opentext_audio.wav");
        }

        private static string GenerateDigits(int min, int max)
        {
            var rand = new Random();

            min = (min == 0) ? 1 : min;
            max = (max == 0) ? 5 : max;

            var values = new int[max];

            if (min > max)
            {
                throw new Exception("Error minlength is large then maxlength for numeric question.");
            }

            for (var i = 0; i < max; i++)
            {
                values[i] = rand.Next(0, 9);
            }

            return string.Join("", values.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
        }
    }
}