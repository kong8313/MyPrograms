using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace NFakes
{
    public class Generator
    {
        public const string NoProj = "noproj";

        private static readonly StringBuilder _parametersBuffer = new StringBuilder(512);
        private static readonly StringBuilder _argumentsBuffer = new StringBuilder(512);
        private static readonly StringBuilder _delegateNameBuffer = new StringBuilder(512);
        private static readonly StringBuilder _outParametersInitializeBuffer = new StringBuilder(512);
        private static readonly HashSet<string> _keywords = new HashSet<string>{"event"};

        private MethodProps GenerateMethodProps(
            TypeInfo interfaceType,
            MethodInfo methodType,
            HashSet<string> usedNamespaces)
        {
            _parametersBuffer.Clear();
            _argumentsBuffer.Clear();
            _delegateNameBuffer.Clear();
            _outParametersInitializeBuffer.Clear();

            bool not1StParameter = false;

            _delegateNameBuffer.Append(methodType.Name);

            foreach (var parameterInfo in methodType.GetParameters())
            {
                if (not1StParameter)
                {
                    _parametersBuffer.Append(", ");
                    _argumentsBuffer.Append(", ");
                }

                var parameterDirection = GetParameterDirection(parameterInfo);
                var parameterTypeName = GetCSharpTypeName(parameterInfo.ParameterType, usedNamespaces);

                _delegateNameBuffer.Append(GetTypeDescription(parameterInfo.ParameterType));

                if (parameterDirection == ParameterDirection.DirectionOut)
                {
                    _delegateNameBuffer.Append("Out");
                    _parametersBuffer.Append("out ");
                    _argumentsBuffer.Append("out ");

                    _outParametersInitializeBuffer.Append("            ");
                    _outParametersInitializeBuffer.Append(parameterInfo.Name);
                    _outParametersInitializeBuffer.Append(" = default(");
                    _outParametersInitializeBuffer.Append(parameterTypeName);
                    _outParametersInitializeBuffer.AppendLine(");");
                }
                else if (parameterDirection == ParameterDirection.DirectionRef)
                {
                    _delegateNameBuffer.Append("Ref");
                    _parametersBuffer.Append("ref ");
                    _argumentsBuffer.Append("ref ");
                }

                _parametersBuffer.Append(parameterTypeName);
                _parametersBuffer.Append(" ");

                if (_keywords.Contains(parameterInfo.Name))
                {
                    _parametersBuffer.Append("@");
                    _argumentsBuffer.Append("@");
                }

                _parametersBuffer.Append(parameterInfo.Name);
                _argumentsBuffer.Append(parameterInfo.Name);

                not1StParameter = true;
            }

            if (methodType.IsGenericMethod)
            {
                var methodProps = new MethodProps
                {
                    InterfaceName = GetCSharpTypeName(interfaceType.UnderlyingSystemType, usedNamespaces),
                    ReturnType = GetCSharpTypeName(methodType.ReturnType, usedNamespaces),
                    MethodName = methodType.Name,
                    Parameters = _parametersBuffer.ToString(),
                    Arguments = _argumentsBuffer.ToString(),
                    DelegateName = "", //TODO
                    DelegateType = "", //TODO
                    OutParametersInitialisation = _outParametersInitializeBuffer.ToString(),
                    IsVoid = IsVoid(methodType.ReturnType),
                    GenericArguments = GenerateCSharpGenericArgumentsList(methodType.GetGenericArguments(), usedNamespaces),
                    IsGeneric = true

                };

                return methodProps;
            }
            else
            {
                var methodProps = new MethodProps
                {
                    InterfaceName = GetCSharpTypeName(interfaceType.UnderlyingSystemType, usedNamespaces),
                    ReturnType = GetCSharpTypeName(methodType.ReturnType, usedNamespaces),
                    MethodName = methodType.Name,
                    Parameters = _parametersBuffer.ToString(),
                    Arguments = _argumentsBuffer.ToString(),
                    DelegateName = _delegateNameBuffer.ToString(),
                    DelegateType = _delegateNameBuffer + "Delegate",
                    OutParametersInitialisation = _outParametersInitializeBuffer.ToString(),
                    IsVoid = IsVoid(methodType.ReturnType),
                    GenericArguments = "",
                    IsGeneric = false
                };

                return methodProps;
            }

        }

        private PropertyProps GenerateFieldProps(
            TypeInfo interfaceType,
            PropertyInfo propertyInfo,
            HashSet<string> usedNamespaces)
        {
            var propertyProps = new PropertyProps
            {
                InterfaceName = GetCSharpTypeName(interfaceType.UnderlyingSystemType, usedNamespaces),
                PropertyName = propertyInfo.Name,
                PropertyDescription = GetTypeDescription(propertyInfo.PropertyType),
                PropertyCSharpType = GetCSharpTypeName(propertyInfo.PropertyType, usedNamespaces),
                HasGetter = propertyInfo.CanRead,
                HasSetter = propertyInfo.CanWrite
            };

            return propertyProps;
        }

        private EventProps GenerateEventProps(EventInfo eventInfo, HashSet<string> usedNamespaces)
        {
            string eventType;
            var parameters = string.Empty;
            var arguments = string.Empty;
            
            if (eventInfo.EventHandlerType.GenericTypeArguments.Length == 0)
            {
                string eventHandlerTypeName = eventInfo.EventHandlerType.Name;
                if (eventHandlerTypeName == "EventHandler")
                {
                    eventType = "EventHandler";
                    parameters = "EventArgs args";
                    arguments = "this, args";
                }
                else
                {
                    eventType = eventHandlerTypeName;
                    TypeInfo eventHandlerInfo = eventInfo.EventHandlerType.Assembly.DefinedTypes.FirstOrDefault(x => x.Name == eventHandlerTypeName);
                    if (eventHandlerInfo == null)
                    {
                        throw new Exception("Internal error: the utility couldn't find information about custom event handler: " + eventHandlerTypeName);
                    }
                    
                    usedNamespaces.Add(eventHandlerInfo.Namespace);

                    ParameterInfo[] parameterInfos = eventHandlerInfo.GetMethod("Invoke")?.GetParameters();
                    if (parameterInfos != null)
                    {
                        foreach (var parameterInfo in parameterInfos)
                        { 
                            if (parameterInfo.Name == "sender")
                            {
                                arguments += "this, ";
                                continue;
                            }

                            var parameterTypeName = GetCSharpTypeName(parameterInfo.ParameterType, usedNamespaces);
                            parameters += parameterTypeName + " " + parameterInfo.Name + ", ";

                            arguments += parameterInfo.Name + ", ";
                        }
                    }

                    parameters = parameters.TrimEnd(',', ' ');
                    arguments = arguments.TrimEnd(',', ' ');
                }
            }
            else
            {
                usedNamespaces.Add(eventInfo.EventHandlerType.GenericTypeArguments[0].Namespace);

                eventType = "EventHandler<" + eventInfo.EventHandlerType.GenericTypeArguments[0].Name + ">";
                parameters = eventInfo.EventHandlerType.GenericTypeArguments[0].Name + " args";
                arguments = "this, args";
            }

            var eventProps = new EventProps
            {
                EventName = eventInfo.Name,
                EventType = eventType,
                Arguments = arguments,
                Parameters = parameters
            };
            
            return eventProps;
        }

        private InterfaceProps GenerateInterfaceProps(TypeInfo interfaceType)
        {
            var usedNamespaces = new HashSet<string> { "System" };

            var interfaceProps = new InterfaceProps
            {
                InterfaceName = interfaceType.Name.Split('`')[0],
                Namespace = interfaceType.Namespace,
                MethodsProps = new List<MethodProps>(),
                PropertyProps = new List<PropertyProps>(),
                EventProps = new List<EventProps>(),
                GenericArguments = GenerateCSharpGenericArgumentsList(interfaceType.GetGenericArguments(), usedNamespaces),
                Constraints = GenerateConstraints(interfaceType.GetGenericArguments(), usedNamespaces),
                UsedNamespaces = usedNamespaces
            };

            foreach (var implementedInterface in interfaceType.ImplementedInterfaces)
            {
                GenerateInterfaceMethodsAndProperties(
                    implementedInterface.GetTypeInfo(),
                    usedNamespaces,
                    interfaceProps);
            }

            GenerateInterfaceMethodsAndProperties(
                interfaceType,
                usedNamespaces,
                interfaceProps);

            return interfaceProps;
        }

        private string GenerateConstraints(IEnumerable<Type> genericArguments, HashSet<string> usedNamespaces)
        {
            string constraints = null;

            foreach (var genericArgument in genericArguments)
            {
                var parameterConstraints = "";

                foreach (var parameterConstraint in genericArgument.GetGenericParameterConstraints())
                {
                    if (parameterConstraint.Name == "ValueType")
                    {
                        continue;    
                    }

                    if (!string.IsNullOrEmpty(parameterConstraints))
                    {
                        parameterConstraints += ", ";
                    }

                    parameterConstraints += GetCSharpTypeName(parameterConstraint, usedNamespaces);
                }

                if (genericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                {
                    if (!string.IsNullOrEmpty(parameterConstraints))
                    {
                        parameterConstraints += ", ";
                    }

                    parameterConstraints += "class ";
                } else if (genericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                {
                    if (!string.IsNullOrEmpty(parameterConstraints))
                    {
                        parameterConstraints += ", ";
                    }

                    parameterConstraints += "struct ";
                }
                else if (genericArgument.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                {
                    if (!string.IsNullOrEmpty(parameterConstraints))
                    {
                        parameterConstraints += ", ";
                    }

                    parameterConstraints += "new() ";
                }

                if (!string.IsNullOrEmpty(parameterConstraints))
                {
                    constraints += " where " + genericArgument.Name + " : " + parameterConstraints;
                }
            }

            return constraints;
        }

        private void GenerateInterfaceMethodsAndProperties(TypeInfo interfaceType, HashSet<string> usedNamespaces, InterfaceProps interfaceProps)
        {
            foreach (var declaredMethod in interfaceType.GetMethods())
            {
                if (declaredMethod.IsSpecialName)
                {
                    // Hopefully property methods
                    continue;
                }

                var methodProps = GenerateMethodProps(
                    interfaceType,
                    declaredMethod,
                    usedNamespaces);

                interfaceProps.MethodsProps.Add(methodProps);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // Fix methods with same name
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            var methodResults = from methodProps in interfaceProps.MethodsProps
                group methodProps by methodProps.DelegateName
                into g
                where g.Count() > 1
                select new {DelegateName = g.Key, MethodProps = g.ToList()};

            foreach (var r in methodResults)
            {
                var index = 0;
                foreach (var m in r.MethodProps)
                {
                    if (index > 0)
                    {
                        m.DelegateName += index;
                        m.DelegateType += index;
                    }
                    ++index;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////


            foreach (var declaredProperty in interfaceType.DeclaredProperties)
            {
                // We do not support indexers at the moment, so we skip interfaces with having indexers
                if (declaredProperty.GetIndexParameters().Any())
                {
                    interfaceProps.Ignore = true;
                    break;
                }

                var propeprtyProps = GenerateFieldProps(
                    interfaceType,
                    declaredProperty,
                    usedNamespaces);

                interfaceProps.PropertyProps.Add(propeprtyProps);
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            // Fix props with same name
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            var propsResults = from propertyProps in interfaceProps.PropertyProps
                               group propertyProps by propertyProps.PropertyName
                               into g
                               where g.Count() > 1
                                   select new { PropertyName = g.Key, PropertyProps = g.ToList() };

            foreach (var r in propsResults)
            {
                var index = 0;
                foreach (var m in r.PropertyProps)
                {
                    if (index > 0)
                    {
                        m.Index = index.ToString();
                    }
                    ++index;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////


            foreach (var declaredEvent in interfaceType.DeclaredEvents)
            {
                var eventProps = GenerateEventProps(
                    declaredEvent,
                    usedNamespaces);

                interfaceProps.EventProps.Add(eventProps);
            }
        }

        private void AddNamespace(Type type, HashSet<string> namespaces)
        {
            namespaces.Add(type.Namespace);
        }

        private ParameterDirection GetParameterDirection(ParameterInfo parameter)
        {
            // Reference parameter, out or ref
            if (parameter.ParameterType.Name.EndsWith("&"))
            {
                if (parameter.IsOut)
                {
                    return ParameterDirection.DirectionOut;
                }

                return ParameterDirection.DirectionRef;
            }

            // In parameter, so return nothing
            return ParameterDirection.DirectionIn;
        }

        private string GetCSharpTypeName(Type type, HashSet<string> usedNamespaces)
        {
            AddNamespace(type, usedNamespaces);

            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            if (type.IsArray)
            {
                return GetCSharpTypeName(type.GetElementType(), usedNamespaces) + "[]";
            }

            if (type.IsGenericType)
            {
                if (FixGenericTypeName(type.Name) == "Nullable")
                {
                    return GetCSharpTypeName(type.GetGenericArguments()[0], usedNamespaces) + "?";
                }
            }

            if (type == typeof(void))
                return "void";

            if (type == typeof(int))
                return "int";

            if (type == typeof(short))
                return "short";

            if (type == typeof(byte))
                return "byte";

            if (type == typeof(bool))
                return "bool";

            if (type == typeof(long))
                return "long";

            if (type == typeof(float))
                return "float";

            if (type == typeof(double))
                return "double";

            if (type == typeof(decimal))
                return "decimal";

            if (type == typeof(string))
                return "string";

            if (type == typeof(DateTime))
                return "DateTime";

            return GetClrTypeName(type, usedNamespaces);
        }

        private string GetClrTypeName(Type type, HashSet<string> usedNamespaces)
        {
            string typeName = "";

            if ((type.DeclaringType != null) && (!type.IsGenericParameter))
            {
                typeName = GetClrTypeName(type.DeclaringType, usedNamespaces) + ".";
            }

            if (type.IsGenericType)
            {
                typeName += FixGenericTypeName(type.Name) + GenerateCSharpGenericArgumentsList(type.GetGenericArguments(), usedNamespaces);
                return typeName;
            }

            typeName += FixArray(FixOutAndRefTypeNames(type.Name));

            return typeName;
        }

        private string GetTypeDescription(Type type)
        {
            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            if (type.IsArray)
            {
                return "ArrayOf" + GetTypeDescription(type.GetElementType());
            }

            if (type.IsGenericType)
            {
                return FixGenericTypeName(type.Name) + "Of"+  GenerateGenericArgumentsListDescription(type.GetGenericArguments());
            }

            var typeName = FixOutAndRefTypeNames(type.Name);

            typeName = FixArray(typeName);

            return typeName;
        }
        
        private string FixArray(string typeName)
        {
            return typeName.Replace("[]", "Array");
        }

        private string FixOutAndRefTypeNames(string typeName)
        {
            // Fix the name for the out or ref parameters
            if (typeName.EndsWith("&"))
            {
                typeName = typeName.Remove(typeName.Length - 1, 1);
            }
            return typeName;
        }

        private string FixGenericTypeName(string genericTypeName)
        {
            return genericTypeName.Split('`')[0];
        }

        private string GenerateCSharpGenericArgumentsList(IEnumerable<Type> types, HashSet<string> usedNamespaces)
        {
            if (!types.Any())
                return "";

            return "<" + string.Join(", ", types.Select((x) => GetCSharpTypeName(x, usedNamespaces)).ToArray()) + ">";
        }

        private string GenerateGenericArgumentsListDescription(IEnumerable<Type> types)
        {
            if (!types.Any())
                return "";

            return string.Concat(types.Select(GetTypeDescription));
        }

        private bool IsVoid(Type type)
        {
            return type == typeof(void);
        }

        private static Assembly LoadAssemblyFromFile(string filePath)
        {
            using (Stream stream = File.OpenRead(filePath))
            {
                var assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
        
        private static Assembly LoadReferencedAssembly(AssemblyName referencedAssembly, string assemblyDirectory)
        {
            try
            {
                if (referencedAssembly.Name == "System.Web.Http")
                {
                    return LoadAssemblyFromFile(Path.Combine(assemblyDirectory, @"..\_3rdpart\Microsoft\System.Web.Http.4.0.0.0\System.Web.Http.dll"));
                }
                else
                {
                    return LoadAssemblyFromFile(Path.Combine(assemblyDirectory, referencedAssembly.Name) + ".dll");
                }
            }
            catch(FileNotFoundException)
            {
                return Assembly.Load(referencedAssembly.FullName);
            }
        }

        private static List<AssemblyName> LoadAllReferencedAssemblies(string assemblyPath)
        {
            var rootAssembly = LoadAssemblyFromFile(assemblyPath);
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath) ?? string.Empty;

            var queue = new Queue<AssemblyName>(rootAssembly.GetReferencedAssemblies());
            var processedAssemblies = new HashSet<string>();
            var result = new List<AssemblyName>();
            
            processedAssemblies.Add(rootAssembly.FullName);

            while (queue.Any())
            {
                var referencedAssembly = queue.Dequeue();
                var fullName = referencedAssembly.FullName;

                // Use the full name for tracking to avoid issues with different versions/cultures
                if (processedAssemblies.Contains(fullName))
                {
                    continue;
                }

                processedAssemblies.Add(fullName);
                result.Add(referencedAssembly);

                try
                {
                    var loadedAssembly = LoadReferencedAssembly(referencedAssembly, assemblyDirectory);
                    
                    foreach (var innerAssemblyName in loadedAssembly.GetReferencedAssemblies())
                    {
                        if (!processedAssemblies.Contains(innerAssemblyName.FullName))
                        {
                            queue.Enqueue(innerAssemblyName);
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not load assembly {fullName}: {e.Message}");
                }
            }

            return result;
        }

        public void ProcessAssembly(string assemblyPath, string projectPath, string outputDirectory, string strongNameKeyFile)
        {
            var assembly = LoadAssemblyFromFile(assemblyPath);
            
            LoadAllReferencedAssemblies(assemblyPath);
                
            var outputAssemblyDirectory = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(assemblyPath));

            try
            {
                var interfacesTypes =
                    from type in assembly.DefinedTypes
                    where type.IsInterface && type.IsPublic && type.Namespace != "mshtml"
                    select type;

                var projectFiles = new List<string>();
                foreach (var interfaceType in interfacesTypes)
                {
                    var interfaceProps = GenerateInterfaceProps(interfaceType);

                    if (interfaceProps.Ignore)
                    {
                        continue;
                    }

                    var interfaceStub = GenerateInterfaceStub(interfaceProps);

                    var stubFilePath = SaveInterfaceStub(interfaceType, interfaceProps, outputAssemblyDirectory, interfaceStub);

                    projectFiles.Add(stubFilePath);
                }

                SaveProjectFile(assembly, outputAssemblyDirectory, assemblyPath, projectPath, strongNameKeyFile, projectFiles);
            }
            catch (ReflectionTypeLoadException ex)
            {
                throw ex.LoaderExceptions[0];
            }
        }

        private string SaveInterfaceStub(TypeInfo interfaceType, InterfaceProps interfaceProps, string outputDirectory, string interfaceStub)
        {
            return SaveFile(
                interfaceStub, 
                outputDirectory, 
                "Stub" +
                GetTypeDescription(interfaceType) + ".cs");
        }

        private string SaveFile(string content, string directory, string fileName)
        {
            Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, fileName);

            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath))
                {
                    var existingContent = sr.ReadToEnd();

                    if (existingContent == content)
                    {
                        // Do not override file if it is not changed, so it won't recompile
                        return filePath;
                    }
                }
            }

            Console.WriteLine("File {0} regenerated successfully.", filePath);
            using (var sw = new StreamWriter(filePath))
            {
                sw.Write(content);
            }

            return filePath;
        }        

        private void SaveProjectFile(Assembly assembly, string outputDirectory, string assemblyPath, string projectPath, string strongNameKeyFile, IEnumerable<string> projectFiles)
        {
            // Sort things
            projectFiles = projectFiles.OrderBy(x => x);

            var projectFormat =
"<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
"<Project ToolsVersion=\"12.0\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">\r\n" +
"  <Import Project=\"$(MSBuildExtensionsPath)/$(MSBuildToolsVersion)/Microsoft.Common.props\" Condition=\"Exists('$(MSBuildExtensionsPath)/$(MSBuildToolsVersion)/Microsoft.Common.props')\" />\r\n" +
"  <PropertyGroup>\r\n" +
"    <Configuration>Debug</Configuration>\r\n" +
"    <Platform>AnyCPU</Platform>\r\n" +
"    <OutputType>Library</OutputType>\r\n" +
"    <OutputPath><output></OutputPath>\r\n" +
"    <TargetFrameworkVersion><targetFramework></TargetFrameworkVersion>\r\n" +
"    <RootNamespace><assemblyName></RootNamespace>\r\n" +
"    <AssemblyName><assemblyName>.Fakes</AssemblyName>\r\n" +
"    <UseVSHostingProcess>false</UseVSHostingProcess>\r\n" +
"    <DebugSymbols>true</DebugSymbols>\r\n" +
"    <DebugType>full</DebugType>\r\n" +
"    <Optimize>false</Optimize>\r\n" +
"    <DefineConstants>DEBUG;TRACE</DefineConstants>\r\n" +
"    <WarningLevel>4</WarningLevel>\r\n" +
"    <ProjectGuid>{<projectGuid>}</ProjectGuid>\r\n"+
"<strongNameKeyFile>" +
"  </PropertyGroup>\r\n" +
"  <ItemGroup>\r\n" +
"<systemReferences>\r\n" +
"<references>\r\n" +
"  </ItemGroup>\r\n" +
"<projectReferences>\r\n" +
"  <ItemGroup>\r\n" +
"<projectFiles>\r\n" +
"  </ItemGroup>\r\n" +
"  <Import Project=\"$(MSBuildToolsPath)/Microsoft.CSharp.targets\" />\r\n" +
"</Project>";

            string projectReferences = "";
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            var assemblyNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyPath);
            var relativeAssemblyDirectory = PathEx.GetRelativePath(outputDirectory, assemblyDirectory);
            var projectGuid = GenerateDeterministicGuid(assemblyNameWithoutExtension);
            var targetFrameworkVersion = "4.7.2";

            var filesToCompile = new StringBuilder();
            
            foreach (var projectFile in projectFiles)
            {
                filesToCompile.AppendFormat(
                    "    <compile Include=\"{0}\" />\r\n", 
                    PathEx.GetRelativePath(outputDirectory, projectFile));
            }
            
            var references = new StringBuilder();
            var systemReferences = new StringBuilder();

            if (projectPath != NoProj)
            {
                var project = new XmlDocument();
                project.Load(projectPath);
                var nsmgr = new XmlNamespaceManager(project.NameTable);
                nsmgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

                var referenceProjectGuid = project.SelectSingleNode("//x:ProjectGuid", nsmgr).InnerText;
                var referenceName = project.SelectSingleNode("//x:AssemblyName", nsmgr).InnerText;
                targetFrameworkVersion = project.SelectSingleNode("//x:TargetFrameworkVersion", nsmgr).InnerText;
                var relativeProjectPath = PathEx.GetRelativePath(outputDirectory, projectPath);

                FillReferences(project, nsmgr, references, systemReferences, outputDirectory, projectPath);

                projectReferences = String.Format(
                    "  <ItemGroup>\r\n" +
                    "    <ProjectReference Include=\"{0}\">\r\n" +
                    "      <Project>{1}</Project>\r\n" +
                    "      <Name>{2}</Name>\r\n" +
                    "    </ProjectReference>\r\n" +
                    "  </ItemGroup>\r\n",
                    relativeProjectPath,
                    referenceProjectGuid,
                    referenceName);
            }
            else
            {
                foreach (var reference in assembly.GetReferencedAssemblies().OrderBy(x => x.Name))
                {
                    if (reference.Name == "mscorlib" || reference.Name == "TypeMock.Interceptors")
                    {
                        continue;
                    }

                    try
                    {
                        // Try to load it from assembly directory
                        var relativeReferenceAssemblyPath = Path.Combine(relativeAssemblyDirectory, reference.Name + ".dll");
                        var fullReferenceAssemblyPaty = Path.Combine(assemblyDirectory, reference.Name + ".dll");

                        Assembly.ReflectionOnlyLoadFrom(fullReferenceAssemblyPaty);

                        references.AppendFormat(
                            "    <Reference Include=\"{0}\" >\r\n" +
                            "      <SpecificVersion>False</SpecificVersion>\r\n" +
                            "      <HintPath>{1}</HintPath>\r\n" +
                            "    </Reference>\r\n",
                            reference.Name,
                            relativeReferenceAssemblyPath);
                    }
                    catch
                    {
                        // Loading from directory is failed, so let's Try to load assembly from GAC
                        Assembly.ReflectionOnlyLoad(reference.FullName);
                        systemReferences.AppendFormat("    <Reference Include=\"{0}\" />\r\n", reference.Name);
                    }
                }

                references.AppendFormat(
                        "    <Reference Include=\"{0}\" >\r\n" +
                        "      <SpecificVersion>False</SpecificVersion>\r\n" +
                        "      <HintPath>{1}</HintPath>\r\n" +
                        "    </Reference>\r\n",
                        assembly.GetName().Name,
                        Path.Combine(relativeAssemblyDirectory, assembly.GetName().Name + ".dll"));
            }

            Console.WriteLine(relativeAssemblyDirectory);

            string strongName = "";
            if (!string.IsNullOrEmpty(strongNameKeyFile))
            {
                var relativeStrongNameKeyFile = PathEx.GetRelativePath(outputDirectory, strongNameKeyFile);

                strongName = string.Format(
                    "    <SignAssembly>true</SignAssembly>\r\n" +
                    "    <AssemblyOriginatorKeyFile>{0}</AssemblyOriginatorKeyFile>\r\n",
                    relativeStrongNameKeyFile);
            }

            var substitutes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("<systemReferences>", systemReferences.ToString()),
                new KeyValuePair<string, string>("<references>", references.ToString()),
                new KeyValuePair<string, string>("<assemblyName>", assemblyNameWithoutExtension),
                new KeyValuePair<string, string>("<targetFramework>", targetFrameworkVersion),
                new KeyValuePair<string, string>("<output>", relativeAssemblyDirectory),
                new KeyValuePair<string, string>("<strongNameKeyFile>", strongName),
                new KeyValuePair<string, string>("<projectReferences>", projectReferences),
                new KeyValuePair<string, string>("<projectGuid>", projectGuid),
                new KeyValuePair<string, string>("<projectFiles>", filesToCompile.ToString()),
            };

            var content = Replace(projectFormat, substitutes);

            SaveFile(content, outputDirectory, assemblyNameWithoutExtension + ".Fakes.csproj");
        }

        private void FillReferences(
            XmlDocument project, XmlNamespaceManager nsmgr, StringBuilder references, StringBuilder systemReferences, string outputDirectory, string projectPath)
        {
            XmlNodeList referencesList = project.SelectNodes("/x:Project/x:ItemGroup/x:Reference", nsmgr);
            XmlNodeList projectReferencesList = project.SelectNodes("/x:Project/x:ItemGroup/x:ProjectReference", nsmgr);

            if (referencesList != null)
            {
                foreach (XmlNode referenceNode in referencesList)
                {
                    references.Append(GetFullNode(referenceNode, outputDirectory, projectPath));
                }
            }

            if (projectReferencesList != null)
            {
                foreach (XmlNode projectReferenceNode in projectReferencesList)
                {
                    systemReferences.Append(GetFullNode(projectReferenceNode, outputDirectory, projectPath));
                }
            }
        }

        private string GetFullNode(XmlNode node, string outputDirectory, string projectPath)
        {
            var projectDirectoryPath = Path.GetDirectoryName(projectPath);

            var result = new StringBuilder("    <" + node.Name);

            if (node.Attributes != null)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (node.Name.ToLowerInvariant() == "projectreference" && attribute.Name.ToLowerInvariant() == "include")
                    {
                        string referencedProjectPath = Path.GetFullPath(Path.Combine(projectDirectoryPath, attribute.Value));
                        string relativeReferencedProjectPath = PathEx.GetRelativePath(outputDirectory, referencedProjectPath);
                        result.AppendFormat(" {0}=\"{1}\"", attribute.Name, relativeReferencedProjectPath);
                    }
                    else
                    {
                        result.AppendFormat(" {0}=\"{1}\"", attribute.Name, attribute.Value);
                    }
                }
            }

            if (node.ChildNodes.Count == 0)
            {
                result.AppendLine(" />");
                return result.ToString();
            }

            result.AppendLine(">");

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name.ToLowerInvariant() == "hintpath" && !childNode.InnerText.Contains("$"))
                {
                    string referencedProjectPath = Path.GetFullPath(Path.Combine(projectDirectoryPath, childNode.InnerText));
                    string relativeReferencedProjectPath = PathEx.GetRelativePath(outputDirectory, referencedProjectPath);
                    result.AppendFormat("      <{0}>{1}</{0}>\r\n", childNode.Name, relativeReferencedProjectPath);
                }
                else
                {
                    result.AppendFormat("      <{0}>{1}</{0}>\r\n", childNode.Name, childNode.InnerText);
                }
            }

            result.AppendFormat("    </{0}>\r\n", node.Name);

            return result.ToString();
        }

        private string GenerateDeterministicGuid(string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(value));
                var result = new Guid(hash);
                return result.ToString();
            }
        }

        private string GenerateInterfaceStub(InterfaceProps interfaceProps)
        {
            var generatedInterfaceStub = new StringBuilder();

            GenerateUsings(
                interfaceProps,
                generatedInterfaceStub);

            GenerateClassHeader(
                interfaceProps,
                generatedInterfaceStub);

            GenerateMethodsStub(
                interfaceProps.MethodsProps, 
                generatedInterfaceStub);

            GeneratePropertiesStub(
                interfaceProps.PropertyProps,
                generatedInterfaceStub);

            GenerateEventStub(
                interfaceProps.EventProps,
                generatedInterfaceStub);

            GenerateClassFooter(
                generatedInterfaceStub);

            return generatedInterfaceStub.ToString();
        }

        private void GenerateClassFooter(StringBuilder generatedInterfaceStub)
        {
            generatedInterfaceStub.Append("    }\r\n}");
        }

        private void GenerateClassHeader(InterfaceProps interfaceProps, StringBuilder generatedInterfaceStub)
        {
            const string classDeclarationFormat =
                "\r\n" +
                "namespace <namespace>.Fakes\r\n" +
                "{\r\n" +
                "    public class Stub<interfaceName><genericArguments> : <interfaceName><genericArguments> <constraints>\r\n" +
                "    {\r\n" +
                "        private <interfaceName><genericArguments> _inner;\r\n" +
                "\r\n" +
                "        public Stub<interfaceName>()\r\n" +
                "        {\r\n" +
                "            _inner = null;\r\n" +
                "        }\r\n" +
                "\r\n" +
                "        public <interfaceName><genericArguments> Inner\r\n" +
                "        {\r\n" +
                "            set {_inner = value;} get {return _inner;}\r\n" +
                "        }\r\n" +
                "\r\n";

            var substitutes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("<namespace>", interfaceProps.Namespace),
                new KeyValuePair<string, string>("<interfaceName>", interfaceProps.InterfaceName),
                new KeyValuePair<string, string>("<genericArguments>", interfaceProps.GenericArguments),
                new KeyValuePair<string, string>("<constraints>", interfaceProps.Constraints)
            };

            generatedInterfaceStub.Append(Replace(classDeclarationFormat, substitutes));
        }

        private void GenerateUsings(InterfaceProps interfaceProps, StringBuilder output)
        {
            foreach (var usedNamespace in interfaceProps.UsedNamespaces)
            {
                output.Append("using " + usedNamespace + ";\r\n");
            }
        }

        private void GenerateMethodsStub(IEnumerable<MethodProps> methodsProp, StringBuilder output)
        {
            foreach (var methodProps in methodsProp)
            {
                string methodStub;

                if (methodProps.IsGeneric)
                {
                    methodStub = methodProps.IsVoid 
                        ? GenerateGenericVoidMethodStub(methodProps) 
                        : GenerateGenericNotVoidMethodStub(methodProps);
                }
                else
                {
                    methodStub = methodProps.IsVoid 
                        ? GenerateVoidMethodStub(methodProps) 
                        : GenerateNotVoidMethodStub(methodProps);
                }

                output.AppendLine(methodStub);
            }
        }

        private void GeneratePropertiesStub(IEnumerable<PropertyProps> propertiesProps, StringBuilder output)
        {
            foreach (var propertiesProp in propertiesProps)
            {
                output.AppendLine(GeneratePropertyStub(propertiesProp));
            }
        }

        private void GenerateEventStub(IEnumerable<EventProps> eventProps, StringBuilder output)
        {
            foreach (var eventProp in eventProps)
            {
                output.AppendLine(GenerateEventStub(eventProp));
            }
        }

        private string GenerateEventStub(EventProps eventProp)
        {
            var methodFormat =
                    "        public event <eventType> <eventName>;\r\n" +
                    "        public void On<eventName>(<parameters>)\r\n" + 
                    "        {\r\n" + 
                    "            if (<eventName> != null)\r\n" + 
                    "            {\r\n" +
                    "                <eventName>(<arguments>);\r\n" + 
                    "            }\r\n" + 
                    "        }\r\n";

            return GenerateEventText(methodFormat, eventProp);
        }
                
        private string GenerateGenericVoidMethodStub(MethodProps methodProps)
        {
            const string methodFormat =
                "        void <interfaceName>.<methodName><genericArguments>(<parameters>)\r\n" +
                "        {\r\n" +
                "<outParametersInitialisation>\r\n"+
                "        }\r\n";

            return GenerateMethodText(methodFormat, methodProps);
        }

        private string GenerateGenericNotVoidMethodStub(MethodProps methodProps)
        {
            const string methodFormat =
                "        <returnType> <interfaceName>.<methodName><genericArguments>(<parameters>)\r\n" +
                "        {\r\n" +
                "<outParametersInitialisation>\r\n"+
                "\r\n" +
                "            return default(<returnType>);\r\n" +
                "        }\r\n";
           
            return GenerateMethodText(methodFormat, methodProps);
        }

        private string GenerateVoidMethodStub(MethodProps methodProps)
        {
            const string methodFormat =
                "        public delegate <returnType> <delegateType><genericArguments>(<parameters>);\r\n" +
                "        public <delegateType> <delegateName>;\r\n" +
                "\r\n" +
                "        void <interfaceName>.<methodName><genericArguments>(<parameters>)\r\n" +
                "        {\r\n" +
                "<outParametersInitialisation>\r\n" +
                "            if (<delegateName> != null)\r\n" +
                "            {\r\n" +
                "                <delegateName>(<arguments>);\r\n" +
                "            } else if (_inner != null)\r\n" +
                "            {\r\n" +
                "                ((<interfaceName>)_inner).<methodName>(<arguments>);\r\n" +
                "            }\r\n" +
                "        }\r\n";

            return GenerateMethodText(methodFormat, methodProps);
        }

        private string GenerateNotVoidMethodStub(MethodProps methodProps)
        {
            const string methodFormat =
                "        public delegate <returnType> <delegateType><genericArguments>(<parameters>);\r\n" +
                "        public <delegateType> <delegateName>;\r\n" +
                "\r\n" +
                "        <returnType> <interfaceName>.<methodName><genericArguments>(<parameters>)\r\n" +
                "        {\r\n" +
                "<outParametersInitialisation>\r\n"+
                "\r\n"+
                "            if (<delegateName> != null)\r\n" +
                "            {\r\n" +
                "                return <delegateName>(<arguments>);\r\n" +
                "            } else if (_inner != null)\r\n" +
                "            {\r\n" +
                "                return ((<interfaceName>)_inner).<methodName>(<arguments>);\r\n" +
                "            }\r\n" +
                "\r\n" +
                "            return default(<returnType>);\r\n" +
                "        }\r\n";

            return GenerateMethodText(methodFormat, methodProps);
        }

        private string GeneratePropertyStub(PropertyProps propertyProps)
        {
            var methodFormat =
                    "        private <propertyCSharpType> _<propertyName><index>;\r\n" +
                    "        public Func<<propertyCSharpType>> <propertyName>Get<index>;\r\n" +
                    "        public Action<<propertyCSharpType>> <propertyName>Set<propertyDescription><index>;\r\n" +
                    "\r\n" +
                    "        <propertyCSharpType> <interfaceName>.<propertyName>\r\n" +
                    "        {\r\n";

            if (propertyProps.HasGetter)
            {
                methodFormat +=
                    "            get\r\n" +
                    "            {\r\n" +
                    "                if (<propertyName>Get<index> != null)\r\n" +
                    "                {\r\n" +
                    "                    return <propertyName>Get<index>();\r\n" +
                    "                } else if (_inner != null)\r\n" +
                    "                {\r\n" +
                    "                    return ((<interfaceName>)_inner).<propertyName>;\r\n" +
                    "                }\r\n" +
                    "\r\n" +
                    "                if (<propertyName>Set<propertyDescription><index> == null)\r\n" +
                    "                {\r\n" +
                    "                     // If both setter and getter delegates are not set then implement same way as autoproperty\r\n" +
                    "                    return _<propertyName><index>;\r\n" +
                    "                }\r\n" +
                    "\r\n" +
                    "                return default(<propertyCSharpType>);\r\n" +
                    "            }\r\n" +
                    "\r\n";
            }

            if (propertyProps.HasSetter)
            {
                methodFormat +=
                    "            set\r\n"+
                    "            {\r\n" +
                    "                if (<propertyName>Set<propertyDescription><index> != null)\r\n" +
                    "                {\r\n" +
                    "                    <propertyName>Set<propertyDescription><index>(value);\r\n" +
                    "                    return;\r\n" +
                    "                } else if (_inner != null)\r\n" +
                    "                {\r\n" +
                    "                    ((<interfaceName>)_inner).<propertyName> = value;\r\n" +
                    "                    return;\r\n" +
                    "                }\r\n" +
                    "\r\n" +
                    "                if (<propertyName>Get<index> == null)\r\n" +
                    "                {\r\n" +
                    "                     // If both setter and getter delegates are not set then implement same way as autoproperty\r\n"+
                    "                    _<propertyName><index> = value;\r\n" +
                    "                }\r\n" +
                    "\r\n" +
                    "            }\r\n";
            }

            methodFormat +=
                    "        }\r\n";

            return GeneratePropertyText(methodFormat, propertyProps);
        }

        private string GenerateMethodText(string format, MethodProps methodProps)
        {
            var substitutes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("<interfaceName>", methodProps.InterfaceName),
                new KeyValuePair<string, string>("<methodName>", methodProps.MethodName),
                new KeyValuePair<string, string>("<returnType>", methodProps.ReturnType),
                new KeyValuePair<string, string>("<parameters>", methodProps.Parameters),
                new KeyValuePair<string, string>("<arguments>", methodProps.Arguments),
                new KeyValuePair<string, string>("<delegateName>", methodProps.DelegateName),
                new KeyValuePair<string, string>("<delegateType>", methodProps.DelegateType),
                new KeyValuePair<string, string>("<outParametersInitialisation>", methodProps.OutParametersInitialisation),
                new KeyValuePair<string, string>("<genericArguments>", methodProps.GenericArguments)
            };

            return Replace(format, substitutes);
        }

        private string GeneratePropertyText(string format, PropertyProps methodProps)
        {
            var substitutes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("<interfaceName>", methodProps.InterfaceName),
                new KeyValuePair<string, string>("<propertyName>", methodProps.PropertyName),
                new KeyValuePair<string, string>("<propertyDescription>", methodProps.PropertyDescription),
                new KeyValuePair<string, string>("<propertyCSharpType>", methodProps.PropertyCSharpType),
                new KeyValuePair<string, string>("<index>", methodProps.Index),
            };

            return Replace(format, substitutes);
        }

        private string GenerateEventText(string format, EventProps eventProps)
        {
            var substitutes = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("<eventName>", eventProps.EventName),
                new KeyValuePair<string, string>("<eventType>", eventProps.EventType),
                new KeyValuePair<string, string>("<arguments>", eventProps.Arguments),
                new KeyValuePair<string, string>("<parameters>", eventProps.Parameters)
            };

            return Replace(format, substitutes);
        }

        private string Replace(string format, IEnumerable<KeyValuePair<string, string>> substitutes)
        {
            var result = format;
            foreach (var substitute in substitutes)
            {
                result = result.Replace(substitute.Key, substitute.Value);
            }

            return result;
        }
    }
}
