using System;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;
using System.Text.RegularExpressions;
using Confirmit.CATI.Core.Repositories;
using System.Diagnostics;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Schedules2007.BvSchScriptGen;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Action = Confirmit.CATI.Core.ScheduleDom.Script.Action;
using Confirmit.CATI.Core.Schedules2007.BvSchScriptGen.Resources;
using ConfirmitDialerInterface;


namespace BvSchScriptGen
{
    public class ScriptGenerator
    {
        private string initializer = "";
        private string wrapers = "";
        private string customScriptClassName = "";

        private int guid = 0;

        private const string baseWraperClassName = "Wrapper";
        private const string baseCustomScriptInvokerName = "CustomScriptInvoker";

        private IPersonGroupService _personGroupService;

        private int GUID
        {
            get
            {
                return ++guid;
            }
        }

        public ScriptGenerator()
        {
            _personGroupService = ServiceLocator.Resolve<IPersonGroupService>();
        }

        public Schedule InitSchedule(string xml)
        {
            var xmlSerializer = new XmlSerializer(typeof(Schedule));
            var settings = new XmlReaderSettings();
            var stringReader = new StringReader(xml);

            XmlReader xmlReader = XmlReader.Create(stringReader, settings);

            return (Schedule)xmlSerializer.Deserialize(xmlReader);
        }

        #region Create names

        private static long _lastCustomScriptInvokeId = 0;

        private static string GenerateCustomScriptInvokerName()
        {
            return baseCustomScriptInvokerName + Interlocked.Increment(ref _lastCustomScriptInvokeId);
        }

        private string GetFunctionWrapperName()
        {
            return "FunctionWrapper" + GUID.ToString();
        }

        static private string GetClassName(params object[] list)
        {
            string result = "";
            foreach (object o in list)
            {
                result += o.ToString();
            }

            return result;
        }
        #endregion

        private void CheckAction(Schedule schedule, SubRuleAction action, string name)
        {
            string parameter = null;

            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            Action actionDesc = scheduleService.GetActions().GetActionById(action.ActionId.Value);

            if (actionDesc == null)
            {
                throw new UserMessageException(
                    String.Format(Strings.ActionNotInitializedMessage));
            }


            switch (action.Parameter.Type)
            {
                case Parameter.ParamType.Constant:
                    parameter = action.Parameter.Constant;
                    break;
                case Parameter.ParamType.Parameter:
                    CustomParameter customParam = schedule.CustomParameters.GetItemById(action.Parameter.ParameterID.Value);

                    if (customParam == null)
                    {
                        throw new UserMessageException(
                            String.Format(Strings.ActionParameterNotSpecifiedMessage, name));
                    }

                    if (!scheduleService.GetMatchingTypes(actionDesc.ParameterType.Value).Contains(customParam.Type.Value))
                    {
                        throw new UserMessageException(
                            String.Format(Strings.ActionParameterInvalidType, name));
                    }

                    if (!scheduleService.CheckParamValue(schedule, 0, customParam.Type.Value, customParam.Value.Value))
                    {
                        throw new UserMessageException(
                            String.Format(Strings.ActionParameterValueInvalid, name));
                    }

                    parameter = customParam.Value.ToString();
                    break;
                default:
                    throw new UserMessageException(
                        String.Format(Strings.ActionParameterTypeNotSpecifiedMessage, name));

            }
           
            switch (action.ActionId)
            {
                //Recall after number of minutes
                case 2:
                    {
                        int number = 0;
                        if (Int32.TryParse(parameter, out number))
                        {
                            if (number >= 1)
                                return;
                        }
                        break;
                    }
                //Recall after number of shifts
                case 3:
                    {
                        int number = 0;
                        if (Int32.TryParse(parameter, out number))
                        {
                            if (number >= 1)
                                return;
                        }
                        break;
                    }
                //Set new ITS
                case 26:
                {
                    int number = 0;
                    if (Int32.TryParse(parameter, out number))
                    {
                        var defaultStateGroup = StateGroupRepository.GetDefault();
                        var state = StateRepository.GetByItsAndStateGroupId(number, defaultStateGroup.ID);
                        if( state != null )
                            return;

                        throw new UserMessageException(
                            String.Format(Strings.ActionParameterValueInvalid, name));
                    }
                    break;
                }
                //Assign Resource
                case 30:
                {
                    var resources = parameter.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select((x) =>
                        {
                            int r;
                            return Int32.TryParse(x, out r) ? (int?) r : null;
                        }).ToArray();

                    if (resources.Any(x => x == null) || resources.Length == 0)
                    {
                        break;
                    }

                    if (resources.Length == 1)
                    {
                        var resource = (int) resources[0];
                        switch (resource)
                        {
                                //[Unchanged]
                            case -1:
                                //[Last Person]
                            case -2:
                                //[Survey Interviewers]
                            case -3:
                                return;
                            default:
                                var person = PersonRepository.TryGetById(resource);
                                var personGroup = PersonGroupRepository.TryGetById(resource);
                                if (person == null && personGroup == null)
                                {
                                    throw new UserMessageException(String.Format(Strings.ActionParameterUserOrGroupDoesntExistMessage, name));
                                }

                                if (personGroup != null && personGroup.IsAdministrative)
                                {
                                    throw new UserMessageException(String.Format(Strings.ActionParameterInvalidGroupMessage, name));
                                }

                                return;
                        }
                    }

                    if (!_personGroupService.IsExistsAndNotAdministrative(resources.Cast<int>().ToArray()))
                    {
                        throw new UserMessageException(String.Format(Strings.ActionParameterInvalidGroupMessage, name));
                    }
                    return;
                }
                case 38:
                {
                    int value;
                    if (!Int32.TryParse(parameter, out value) || 
                        (value != 0 && value != (int)DialingMode.Preview && value != (int)DialingMode.SpecialDial))
                    {
                        break;
                    }


                    return;
                }

                case 41://Add additional assignment on group
                case 42://Remove specific assignment on group
                {
                    var resources = parameter.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select((x) =>
                        {
                            int r;
                            return Int32.TryParse(x, out r) ? (int?) r : null;
                        }).ToArray();

                    if (resources.Any(x => x == null) || resources.Length == 0)
                    {
                        break;
                    }

                    if (!_personGroupService.IsExistsAndNotAdministrative(resources.Cast<int>().ToArray()))
                    {
                        throw new UserMessageException(String.Format(Strings.ActionParameterInvalidGroupMessage, name));
                    }
                    return;
                }
                default:
                    {
                        return;
                    }
            }

            throw new UserMessageException(
                String.Format(Strings.ActionParameterNotSpecifiedMessage, action.Id));

        }

        private string GetInitializeScript(Schedule schedule, string customScriptClassName)
        {
            string result = "";

            int ruleNumber = 0;
            int subRuleNumber = 0;
            int actionNumber = 0;

            //Create schedule
            result += String.Format("schedule = new Schedule({0});\n\n",
                    schedule.Rules.Count);

            var scheduleService = ServiceLocator.Resolve<IScheduleService>();

            foreach (Rule rule in schedule.Rules)
            {
                string ruleName = GetClassName("rule", ruleNumber++);
                result += String.Format("var {0} : Schedule.Rule = new Schedule.Rule('{1}', {2}, {3});\n",
                    ruleName, rule.Id, rule.SubRules.Count, rule.SampleUpdate.ToString().ToLower());

                result += String.Format("schedule.AddRule({0});\n", ruleName);

                foreach (SubRule subRule in rule.SubRules)
                {
                    string subRuleName = GetClassName("subRule", subRuleNumber++);
                    result += String.Format("var {0} : Schedule.Rule.SubRule = new Schedule.Rule.SubRule('{1}', {2}, {3}, {4}, {5});\n",
                        subRuleName, subRule.Id, subRule.ItsId, subRule.ShiftTypeId,  
                        subRule.FilterEnabled.ToString().ToLower(), subRule.SubRuleActions.Count);

                    result += String.Format("{0}.AddSubRule({1});\n", ruleName, subRuleName);

                    //create filter script
                    string className = WrapFilter(subRule.Filter, ref wrapers, new CustomCodeDescription(rule, subRule), customScriptClassName);

                    result += String.Format("{0}.filterFactory = new CustomCode.{1}();\n", subRuleName, className);

                    foreach (SubRuleAction action in subRule.SubRuleActions)
                    {
                        string name = String.Format("Rule: {0}, SubRule: {1}, Action: {2} - {3}", 
                                schedule.GetNumberByGuid(rule.Id.Value),
                                schedule.GetNumberByGuid(subRule.Id.Value),
                                action.Id,
                                scheduleService.GetActions().GetActionById(action.ActionId.Value).Name);
                        //check action is valid
                        CheckAction(schedule, action, name);

                        //call action's ctr 
                        string actionName = GetClassName("action", actionNumber++);
                        result += String.Format(
                            "var {0} : Schedule.Rule.SubRule.Action = new Schedule.Rule.SubRule.Action({1}, {2}, {3}, {4}, {5}, '{6}');\n",
                            actionName,
                            action.Id,
                            action.ActionId,
                            action.Enabled.ToString().ToLower(),
                            action.FilterEnabled.ToString().ToLower(),
                            (action.Parameter.Type == Parameter.ParamType.Constant).ToString().ToLower(),
                            action.Parameter.Value != null ? action.Parameter.Value.Replace("'", "\\'") : null);

                        //add action to subrule
                        result += String.Format("{0}.AddAction({1});\n", subRuleName, actionName);

                        //create filter script
                        className = WrapFilter(action.Filter, ref wrapers, new CustomCodeDescription(rule, subRule, action), customScriptClassName);

                        //add delegate to action, which create instance class. this class
                        //implement filter interface
                        result += String.Format("{0}.filterFactory = new CustomCode.{1}();\n", actionName, className);

                        //processing specify action id
                        switch (action.ActionId)
                        {
                            //run custom script
                            case 9:
                                {
                                    if (action.Parameter.Type != Parameter.ParamType.Constant)
                                        throw new NotSupportedException("assign function call result to variable action not support custom parameter");

                                    //create custom script invoker
                                    string customScriptInvokerName = CreateCustomScriptInvoker(
                                        action.Parameter.Constant,
                                        ref wrapers);

                                    result += String.Format(
                                        "{0}.customScriptInvokerFactory = new CustomCode.{1}();\n",
                                        actionName,
                                        customScriptInvokerName);
                                    break;
                                }
                            //assign function call result to variable
                            case 18:
                                {
                                    if (action.Parameter.Type != Parameter.ParamType.Constant)
                                        throw new NotSupportedException("assign function call result to variable action not support custom parameter");

                                    string functionWrapperName = WrapFunction(action.Parameter.Constant, ref wrapers);

                                    //create custom script class instance
                                    result += String.Format("{0}.customFunction = new {1}();\n",
                                        actionName, functionWrapperName);
                                    break;
                                }
                        }
                    }
                }
            }

            return result;
        }

        private string CreateCustomScriptInvoker(
            string methodInvokCode,
            ref string customScriptInvokerCode)
        {
            string customScriptInvokerName = GenerateCustomScriptInvokerName();

            string factoryClassName = CreateCustomScriptInvokerFactory(customScriptInvokerName, ref customScriptInvokerCode);

            
            customScriptInvokerCode += String.Format(
                "class {0}  extends BvDotNetScript.ScriptObjects.ExtendedSchedulingAPI implements CustomCode.ICustomScriptInvoker\n",
                customScriptInvokerName);
            customScriptInvokerCode += "{\n";

            customScriptInvokerCode += String.Format(
                "   private var customScriptWrapper : {0};\n",
                customScriptClassName);

            customScriptInvokerCode += String.Format(
                "   public function Init(wrapper : CustomCode.CustomScriptWrapper, extendedAPI : ExtendedSchedulingAPI)\n");

            customScriptInvokerCode += "   {\n";

            customScriptInvokerCode += String.Format(
                "      customScriptWrapper = ({0})(wrapper);\n",
                customScriptClassName);

            customScriptInvokerCode += "      customScriptWrapper.Init(extendedAPI);\n";

            customScriptInvokerCode += "      super.Init(extendedAPI);\n";

            customScriptInvokerCode += "   }\n";

            customScriptInvokerCode += "   public function Invoke()\n";
            customScriptInvokerCode += "   {\n";
            if (methodInvokCode.Contains("("))
            {
                customScriptInvokerCode += String.Format(
                    "      customScriptWrapper.{0};\n",
                    methodInvokCode);
            }
            else
            {
                customScriptInvokerCode += String.Format(
                    "      customScriptWrapper.{0}();\n",
                    methodInvokCode);
            }
            customScriptInvokerCode += "   }\n";

            customScriptInvokerCode += "}\n";

            return factoryClassName;
        }

        private static string CreateCustomScriptInvokerFactory(string className, ref string factoryClass)
        {
            string factoryClassName = className + "Factory";

            factoryClass += String.Format("class {0} implements CustomCode.ICustomScriptInvokerFactory\n", factoryClassName);
            factoryClass += "{\n";
            factoryClass += "   public function CreateInvoker() : ICustomScriptInvoker\n";
            factoryClass += "   {\n";
            factoryClass += String.Format("      return new {0}();\n", className);
            factoryClass += "   }\n";
            factoryClass += "}\n";

            return factoryClassName;
        }

        private static string FormatCustomCode(string customCode)
        {
            return customCode.Replace("class", "static class");
        }

        private static string CreateFilterInvokerFactory(string className, ref string factoryClass)
        {
            string factoryClassName = className + "Factory";

            factoryClass += String.Format("class {0} implements CustomCode.IFilterFactory\n", factoryClassName);
            factoryClass += "{\n";
            factoryClass += "   public function GetInstanceFilterInvoker() : FilterInvoker\n";
            factoryClass += "   {\n";
            factoryClass += String.Format("      return new {0}();\n", className);
            factoryClass += "   }\n";
            factoryClass += "}\n";

            return factoryClassName;
        }

        //return name of factory, which create wrapper class instance
        private string WrapFilter(string customCode, ref string wraperClass, CustomCodeDescription filterDescription, string customScriptClassName)
        {
            if (String.IsNullOrEmpty(customCode))
            {
                customCode = "true";
            }

            //generate wrapper class name
            string wraperClassName = GetClassName(baseWraperClassName, GUID);

            string formatCustomCode = FormatCustomCode(customCode);

            string factoryClassName = CreateFilterInvokerFactory(wraperClassName, ref wraperClass);

            wraperClass += String.Format("class {0} extends CustomCode.FilterInvoker\n", wraperClassName);
            wraperClass += "{\n";

            wraperClass += "public function Invoke(customScript : CustomCode.CustomScriptWrapper) : Boolean\n";
            wraperClass += "{\n";
            wraperClass += String.Format("    var script : CustomCode.{0} = (CustomCode.{0})(customScript);\n", customScriptClassName);
            wraperClass += "    return ExecuteFilter(script);\n";
            wraperClass += "}\n";
            wraperClass += String.Format("function ExecuteFilter(CustomScript : CustomCode.{0}) : Boolean\n", customScriptClassName);
            wraperClass += "{\n";

            wraperClass += CustomCodeMarker.FormatMarker(filterDescription.Serialize());
            if (formatCustomCode.IndexOf("return") < 0)
            {
                wraperClass += "return ";
            }
            wraperClass += formatCustomCode;
            wraperClass += ";\n";

            wraperClass += "}\n";

            wraperClass += "}\n";

            return factoryClassName;
        }

        //return name of wraper class
        private string WrapCustomCode(string customCode, ref string wraperClass, CustomCodeDescription customScriptDescription)
        {
            string wraperClassName = "CustomScript";

            string formatCustomCode = FormatCustomCode(customCode);

            wraperClass += String.Format("class {0} extends CustomCode.CustomScriptWrapper\n", wraperClassName);
            wraperClass += "{\n";
            wraperClass += CustomCodeMarker.FormatMarker(customScriptDescription.Serialize());
            wraperClass += formatCustomCode + "\n";
            wraperClass += "}\n";

            return wraperClassName;
        }

        //return name of wrapper class
        private string WrapFunction(string actionParameter, ref string wraperClass)
        {
            Match match = Regex.Match((actionParameter), "^(.*)=(.*)$");

            string functionWrapperName = GetFunctionWrapperName();

            wraperClass += String.Format("class {0} extends FunctionWrapper\n", functionWrapperName);

            wraperClass += "{\n";
            wraperClass += "   public function Invoke()\n";
            wraperClass += "   {\n";
            wraperClass += String.Format("   var wrapper : CustomCode.CustomScript =((CustomCode.{0})(CustomScript));", customScriptClassName);
            wraperClass += String.Format("f('{0}').setValue(wrapper.{1}());\n",
                match.Groups[1].Value.Trim(),
                match.Groups[2].Value.Trim()
                );
            wraperClass += "   }\n";
            wraperClass += "}\n";

            return functionWrapperName;
        }

        public string GenerateScript(string xml)
        {
            try
            {
                //Filling Schedule structure from xml file
                Schedule schedule = InitSchedule(xml);

                //wrap custom script (include all custom code in wrapper class)
                customScriptClassName = WrapCustomCode(
                    schedule.CustomScript.Body,
                    ref wrapers,
                    new CustomCodeDescription(schedule.CustomScript));

                //there are filling initializing and wrappers
                initializer = String.Format(
                    Resource.Initializer,
                    GetInitializeScript(schedule, customScriptClassName),
                    customScriptClassName);

                wrapers = String.Format(
                    Resource.Wrapers,
                    wrapers);

                return String.Format(
                    Resource.Output,
                    initializer,
                    wrapers,
                    Resource.Interpreter);
            }
            catch (System.Exception e)
            {
                Trace.TraceError("Error generate JS.Net scheduling script. Exception detailes: {0}", e.ToString());
                throw;
            }
        }

        //
        //Output: return list of implemented actions as xml.
        //xml conform next scheme: 
        //      http://wiki.firmglobal.net/default.aspx/FIRM.Multimode/ActionsXML.html
        //
        public string GetValidActions()
        {
            return Resource.Actions;
        }
    }
}


