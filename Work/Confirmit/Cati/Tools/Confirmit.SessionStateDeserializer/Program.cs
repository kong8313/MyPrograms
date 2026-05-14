using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.SessionState;

namespace Confirmit.SessionStateDeserializer
{
    class Program
    {
        static void TraceSessionState(byte[] blob)
        {
            using (var ms = new MemoryStream(blob))
            using (var reader = new BinaryReader(ms))
            {
                int len = reader.ReadInt32();
                bool f1 = reader.ReadBoolean(), f2 = reader.ReadBoolean();
                SessionStateItemCollection items = null;
                HttpStaticObjectsCollection sitems = null;

                if (f1)
                {
                    items = SessionStateItemCollection.Deserialize(reader);

                    var itemsInfos = new Dictionary<string, long>();

                    long total = 0;
                    foreach (string key in items.Keys)
                    {
                        var item = items[key];

                        if (item == null)
                        {
                            continue;
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            var formatter = new BinaryFormatter();
                            formatter.Serialize(memoryStream, item);

                            itemsInfos.Add(key, memoryStream.Length);

                            total += memoryStream.Length;
                        }
                    }

                    var sortedInfos = itemsInfos.OrderByDescending(x => x.Value);

                    foreach (var keyValuePair in sortedInfos)
                    {
                        Console.WriteLine(
                            "{0} {1} {2}", 
                            keyValuePair.Key, 
                            keyValuePair.Value,
                            items[keyValuePair.Key]);
                    }
                }

                //if (f2)
                //{
                //    sitems = HttpStaticObjectsCollection.Deserialize(reader);
                //}

                //if (reader.ReadByte() != 0xFF)
                //{
                //    throw new InvalidOperationException("corrupt");
                //}

                //if (items != null)
                //{
                //    int max = items.Count;
                //    for (int i = 0; i < max; i++)
                //    {
                //        object obj = items[i];
                //        Console.WriteLine("{0}\t{1}", items.Keys[i],
                //            obj == null ? "n/a" : obj.GetType().FullName);
                //    }
                //}
            }
        }

        static void Main(string[] args)
        {
            try
            {

                string blob;
                using (var sr = new StreamReader(args[0]))
                {
                    blob = sr.ReadToEnd();
                }

                var shb = SoapHexBinary.Parse(blob);

                TraceSessionState(shb.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
