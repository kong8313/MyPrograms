// JScript source code
import System;
import System.Diagnostics;
import System.Collection;
import System.Collection.Generic;
import System.Threading;
import JsDelegates;
import CustomCode;
import Interpreter;
import Microsoft.Win32;
import BvDotNetScript.Interfaces;

package Interpreter
{{
    class Initializer implements ISchedulingScript 
    {{  
       static private var schedule : Schedule = null;

       //we needn't create instance this class
       public function Initializer()
       {{}}

       //static constructor
       static Initializer
       {{
          var r : RegistryKey = Registry.LocalMachine.OpenSubKey(
             "Software\\Confirmit\\CATI");
          if( r != null && r.GetValue("DebugDotNetScriptStartup", false ) )
             Debugger.Break();
          Initialize();
       }}
       
       //Initialize all rules, subrules and action
       static private function Initialize()
       {{
          {0}
       }}
       
       public function Execute(bevent : IEventSchedule)
       {{
          var rulesInterpreter : RulesInterpreter = new RulesInterpreter();
          
          var customScripWrapper : CustomCode.{1} = new CustomCode.{1}();
          
          rulesInterpreter.Execute(bevent, 
              Initializer.GetSchedule(), 
              customScripWrapper);
       }}
       
       //Return Schedule for this assembly
       static public function GetSchedule() : Schedule
       {{
          return schedule;
       }}
    }}
}}
