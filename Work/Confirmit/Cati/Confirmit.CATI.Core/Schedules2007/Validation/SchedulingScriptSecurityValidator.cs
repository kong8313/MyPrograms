using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.SystemSettings;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Confirmit.CATI.Core.Schedules2007.Validation
{
    public class SchedulingScriptSecurityValidator : ISchedulingScriptSecurityValidator
    {
        private readonly ISchedulingScriptSettings _schedulingScriptSettings;

        private readonly string[] _secureTypes = new[]
        {
            "Interpreter.Initializer",
            "Interpreter.Schedule",
            "Interpreter.Schedule/Rule",
            "Interpreter.Schedule/Rule/SubRule",
            "Interpreter.Schedule/Rule/SubRule/Action",
            "Interpreter.RulesInterpreter",
            "Interpreter.RulesInterpreter/Point"
        };

        // Namespaces/types that are fully whitelisted (all methods allowed)
        // Using explicit whitelist approach for security - only safe namespaces are allowed
        private readonly string[] _whitelistedNamespaces = new[]
        {
            // Script interpreters
            "Microsoft.JScript",
            "BvDotNetScript",
            
            // Confirmit CATI specific types
            "Confirmit.CATI.Core.DAL.Generated.Entity.Table.BvInterviewEntity",
            "Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure.BvCallEntity",
            "Confirmit.CATI.Core.DAL.Generated.Entity.Table.BvAppointmentEntity",
            "Confirmit.CATI.Core.Services.ShiftService/MatchingShift",
            "Confirmit.CATI.Core.Repositories.AppointmentRepository",

            // Safe System primitive types
            "System.Boolean",
            "System.Byte",
            "System.Char",
            "System.Decimal",
            "System.Double",
            "System.Int16",
            "System.Int32",
            "System.Int64",
            "System.SByte",
            "System.Single",
            "System.UInt16",
            "System.UInt32",
            "System.UInt64",
            "System.IntPtr",
            "System.UIntPtr",

            // Safe System utility types
            "System.Math",
            "System.String",
            "System.DateTime",
            "System.DateTimeOffset",
            "System.TimeSpan",
            "System.TimeZoneInfo",
            "System.Guid",
            "System.Convert",
            "System.Array",
            "System.Nullable",
            "System.Nullable`1",
            "System.Tuple",
            "System.ValueTuple",
            "System.Enum",
            "System.StringComparer",
            "System.StringComparison",
            "System.DateTimeKind",
            "System.DayOfWeek",
            "System.ConsoleColor",
            "System.Version",
            "System.Random",
            "System.BitConverter",
            "System.Buffer",
            "System.Lazy",
            "System.Action",
            "System.Func",
            "System.Predicate",
            "System.Comparison",
            "System.Converter",
            "System.EventArgs",
            "System.EventHandler",
            "System.IComparable",
            "System.IEquatable",
            "System.IFormattable",
            "System.IConvertible",
            "System.ICloneable",
            "System.IDisposable",
            "System.Object",
            "System.ValueType",
            "System.Void",
            "System.DBNull",

            // Safe System.Collections
            "System.Collections",
            "System.Collections.Generic",
            "System.Collections.ObjectModel",
            "System.Collections.Specialized",

            // Safe System.Linq
            "System.Linq",

            // Safe System.Text (limited)
            "System.Text.StringBuilder",
            "System.Text.Encoding",
            "System.Text.ASCIIEncoding",
            "System.Text.UTF8Encoding",
            "System.Text.UnicodeEncoding",
            "System.Text.UTF32Encoding",

            // Safe System.Globalization
            "System.Globalization",

            // Safe System.Text.RegularExpressions (with caution for ReDoS, but generally needed)
            "System.Text.RegularExpressions",
        };

        // Dangerous namespaces/types that should always be blocked (defense in depth)
        // These are checked even if somehow a type matches a whitelisted pattern
        private readonly string[] _excludedNamespaces = new[]
        {
            // File system access
            "System.IO",
            
            // Network access
            "System.Net",
            
            // Reflection and dynamic code execution
            "System.Reflection",
            "System.Activator",
            "System.AppDomain",
            "System.Type",
            "System.Delegate",
            
            // Runtime and interop
            "System.Runtime",
            
            // Process and diagnostics
            "System.Diagnostics",
            
            // Security manipulation
            "System.Security",
            
            // Threading (can cause DoS)
            "System.Threading",
            
            // Database access
            "System.Data",
            
            // Web access
            "System.Web",
            
            // UI access
            "System.Windows",
            "System.Drawing",
            
            // Code generation
            "System.CodeDom",
            
            // Configuration access
            "System.Configuration",
            
            // Directory services (LDAP/AD)
            "System.DirectoryServices",
            
            // WMI access
            "System.Management",
            
            // Message queue access
            "System.Messaging",
            
            // Service control
            "System.ServiceProcess",
            
            // Environment access
            "System.Environment",
            
            // XML (XXE attacks)
            "System.Xml",
            
            // Serialization (deserialization attacks)
            "System.Runtime.Serialization",
            
            // Component model (dynamic invocation)
            "System.ComponentModel",
            
            // Resources
            "System.Resources",
            
            // Transactions
            "System.Transactions",
            
            // Enterprise services
            "System.EnterpriseServices",
            
            // Media
            "System.Media",
            
            // Printing
            "System.Printing",
            
            // Speech
            "System.Speech",
        };

        public SchedulingScriptSecurityValidator(ISchedulingScriptSettings schedulingScriptSettings)
        {
            _schedulingScriptSettings = schedulingScriptSettings;
        }

        public SchedulingScriptSecurityValidatorResult Validate(string assemblyFileName)
        {
            var unsecureCalls = new List<MethodReference>();

            var assembly = AssemblyDefinition.ReadAssembly(assemblyFileName);

            var allTypes = assembly.Modules.SelectMany(m => GetRecursiveTypes(m.Types))
                .Where(t => !_secureTypes.Contains(t.FullName)).ToArray();

            var allMethods = allTypes.SelectMany(t => t.Methods).Where(m => m.HasBody).ToArray();

            var allCallInstructions = allMethods.SelectMany(GetCallInstructions).ToArray();

            foreach (var instruction in allCallInstructions)
            {
                var calledMethod = instruction.Operand as MethodReference;

                if (!IsSecureMethod(assembly.Modules, calledMethod))
                {
                    if (!unsecureCalls.Any(x => x.FullName == calledMethod.FullName))
                    {
                        unsecureCalls.Add(calledMethod);
                    }
                }
            }
            return new SchedulingScriptSecurityValidatorResult(unsecureCalls.Select(x => x.FullName).ToArray());
        }

        private bool IsSecureMethod(Collection<ModuleDefinition> secureModules, MethodReference method)
        {
            if (method == null)
                return false;

            if (method is MethodDefinition)
                return true;

            if (secureModules.Any(x => x.FullyQualifiedName == method.DeclaringType.Scope.Name))
                return true;

            var declaringTypeFullName = method.DeclaringType?.FullName ?? string.Empty;

            // Check if method is in the explicit whitelist (for backward compatibility and Confirmit.CATI methods)
            if (_schedulingScriptSettings.SecureExternalMethodList.Contains(method.FullName))
                return true;

            // Check if the method belongs to a whitelisted namespace
            if (IsInWhitelistedNamespace(declaringTypeFullName) && !IsInExcludedNamespace(declaringTypeFullName))
                return true;

            // Block everything else by default
            return false;
        }

        private bool IsInWhitelistedNamespace(string typeFullName)
        {
            foreach (var ns in _whitelistedNamespaces)
            {
                if (typeFullName.StartsWith(ns + ".") || typeFullName.StartsWith(ns + "<") || typeFullName == ns)
                    return true;
            }
            return false;
        }

        private bool IsInExcludedNamespace(string typeFullName)
        {
            foreach (var excludedNs in _excludedNamespaces)
            {
                if (typeFullName.StartsWith(excludedNs + ".") || typeFullName == excludedNs)
                    return true;
            }
            return false;
        }

        private IEnumerable<Instruction> GetCallInstructions(MethodDefinition method)
        {
            foreach (var instruction in method.Body.Instructions)
            {
                switch (instruction.OpCode.Code)
                {
                    case Code.Call:
                    case Code.Calli:
                    case Code.Callvirt:
                    {
                        yield return instruction;
                    }
                    break;
                }
            }
        }

        private static IEnumerable<TypeDefinition> GetRecursiveTypes(Collection<TypeDefinition> types)
        {
            foreach (var type in types)
            {
                yield return type;
                foreach (var nestedType in GetRecursiveTypes(type.NestedTypes))
                {
                    yield return nestedType;
                }
            }
        }
    }
}
