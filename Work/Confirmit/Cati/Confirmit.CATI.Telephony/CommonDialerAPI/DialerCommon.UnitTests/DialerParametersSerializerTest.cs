using System;
using System.Reflection;
using System.Collections.Generic;
using DialerCommon.DialerParameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerCommon.UnitTests
{
    [TestClass]
    public class DialerParametersSerializerTest
    {
        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void SerializeDialerParameters_DoTwoSerialization_CountOfAssembliesIsNotGrow()
        {
            var parameters = new List<DialerParameter> {
                new DialerParameter {
                    Id = "id",
                    Name = "name",
                    Type = "type",
                    Value = "value"
                }
            };

            var serializedParameters = DialerParametersSerializer.SerializeDialerParameters(parameters);
            Assert.IsNotNull(serializedParameters);
            var assemblyCountAfterOneSerialization = AppDomain.CurrentDomain.GetAssemblies().Length;

            serializedParameters = DialerParametersSerializer.SerializeDialerParameters(parameters);
            Assert.IsNotNull(serializedParameters);
            var assemblyCountAfterTwoSerializations = AppDomain.CurrentDomain.GetAssemblies().Length;
            Assert.AreEqual(assemblyCountAfterOneSerialization, assemblyCountAfterTwoSerializations, "Wrong count of assemblies in domain after two serializations");
        }
    }
}