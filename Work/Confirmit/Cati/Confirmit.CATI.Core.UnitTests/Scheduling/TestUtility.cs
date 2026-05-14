using System;
using System.IO;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Confirmit.CATI.Core.UnitTests.Scheduling
{
    /// <summary>
    /// Utility class for tests.
    /// </summary>
    internal class TestUtility
    {
        public static Schedule DeserializeSchedule( string stringSchedule )
        {
            XmlSerializer serialiazer = new XmlSerializer( typeof( Schedule ) );

            using (StringReader reader = new StringReader(stringSchedule))
            {
                return (Schedule)serialiazer.Deserialize(reader);
            }
        }

        public static string SerializeSchedule(Schedule schedule)
        {
            XmlSerializer serializer = new XmlSerializer( typeof( Schedule ) );

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, schedule);

                return writer.ToString();
            }
        }

        public static bool CompareScheduleWithSerialization(Schedule first, Schedule second)
        {
            return ( SerializeSchedule( first ) == SerializeSchedule( second ) );
        }

        /// <summary>
        /// Creates a test assembly with specified method calls for testing
        /// </summary>
        public static void CreateTestAssembly(string assemblyPath, (string TypeName, string MethodName, Type[] ParameterTypes)[] methodCalls)
        {
            var assemblyName = new AssemblyNameDefinition("TestAssembly", new Version(1, 0, 0, 0));
            var assembly = AssemblyDefinition.CreateAssembly(assemblyName, "TestModule", ModuleKind.Dll);
            var module = assembly.MainModule;

            // Create a test class
            var testClass = new TypeDefinition("TestNamespace", "TestClass",
                TypeAttributes.Public | TypeAttributes.Class,
                module.TypeSystem.Object);
            module.Types.Add(testClass);

            // Create a test method
            var testMethod = new MethodDefinition("TestMethod",
                MethodAttributes.Public | MethodAttributes.Static,
                module.TypeSystem.Void);
            testClass.Methods.Add(testMethod);

            var il = testMethod.Body.GetILProcessor();

            // Add method calls
            foreach (var (typeName, methodName, parameterTypes) in methodCalls)
            {
                // Import the type
                var typeRef = new TypeReference(
                    GetNamespace(typeName),
                    GetClassName(typeName),
                    module,
                    GetAssemblyNameReference(typeName, module));

                // Create method reference
                var methodRef = new MethodReference(methodName, module.TypeSystem.Void, typeRef);
                foreach (var paramType in parameterTypes)
                {
                    methodRef.Parameters.Add(new ParameterDefinition(module.Import(paramType)));
                }

                // Add default parameter values to stack
                foreach (var paramType in parameterTypes)
                {
                    if (paramType == typeof(string))
                    {
                        il.Append(il.Create(OpCodes.Ldstr, "test"));
                    }
                    else if (paramType == typeof(int))
                    {
                        il.Append(il.Create(OpCodes.Ldc_I4_0));
                    }
                    else if (paramType == typeof(bool))
                    {
                        il.Append(il.Create(OpCodes.Ldc_I4_0));
                    }
                    else if (paramType == typeof(object))
                    {
                        il.Append(il.Create(OpCodes.Ldnull));
                    }
                    else
                    {
                        il.Append(il.Create(OpCodes.Ldnull));
                    }
                }

                // Add call instruction
                il.Append(il.Create(OpCodes.Call, methodRef));

                // Pop return value if any
                if (methodRef.ReturnType != module.TypeSystem.Void)
                {
                    il.Append(il.Create(OpCodes.Pop));
                }
            }

            il.Append(il.Create(OpCodes.Ret));

            // Write assembly to disk
            assembly.Write(assemblyPath);
        }

        public static string GetNamespace(string fullTypeName)
        {
            var lastDot = fullTypeName.LastIndexOf('.');
            return lastDot > 0 ? fullTypeName.Substring(0, lastDot) : string.Empty;
        }

        public static string GetClassName(string fullTypeName)
        {
            var lastDot = fullTypeName.LastIndexOf('.');
            return lastDot > 0 ? fullTypeName.Substring(lastDot + 1) : fullTypeName;
        }

        public static AssemblyNameReference GetAssemblyNameReference(string typeName, ModuleDefinition module)
        {
            // Map common namespaces to their assemblies
            if (typeName.StartsWith("System.IO") || typeName.StartsWith("System.Net") ||
                typeName.StartsWith("System.Data") || typeName.StartsWith("System.Reflection") ||
                typeName.StartsWith("System.Diagnostics") || typeName.StartsWith("System."))
            {
                return new AssemblyNameReference("mscorlib", new Version(4, 0, 0, 0))
                {
                    PublicKeyToken = new byte[] { 0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89 }
                };
            }
            else if (typeName.StartsWith("Microsoft.JScript"))
            {
                return new AssemblyNameReference("Microsoft.JScript", new Version(10, 0, 0, 0))
                {
                    PublicKeyToken = new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }
                };
            }
            else if (typeName.StartsWith("Confirmit.CATI"))
            {
                return new AssemblyNameReference("Confirmit.CATI.Core", new Version(1, 0, 0, 0));
            }
            else
            {
                return new AssemblyNameReference("UnknownAssembly", new Version(1, 0, 0, 0));
            }
        }
    }
}