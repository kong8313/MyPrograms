using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BvDotNetScript.ScriptObjects.Cache;
using BvDotNetScript.ScriptObjects;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators.Fakes;

namespace Confirmit.CATI.Core.UnitTests.Scheduling.BvDotNetScript.StriptObjects
{
    /// <summary>
    /// Summary description for ExprObjTest
    /// </summary>
    [TestClass]
    public class ExprObjTest : BaseTest
    {
        private readonly IFormDescValidator _validatorStub = new StubIFormDescValidator();

        private FormDescBase FakeFormDesc()
        {
            FormDescBase fakeFormDesc = new SystemFormDesc(
                1, "p123", new BvReplicationColumnsEntity { ColumnName = "test", ColumnType = 3 });
            fakeFormDesc.IsReplicated = false;

            return fakeFormDesc;
        }

        #region toInt

        private int toInt_GetValue(string input)
        {
            FormDescBase fakeFormDesc = FakeFormDesc();

            var dataService = new StubIInterviewFormDataService
            {
                GetFormValueFormDescBaseStringArrayOfString = (form, cat, loops) => input
            };

            JavaScriptExpObj target = new JavaScriptExpObj(dataService, _validatorStub, fakeFormDesc, 1, null);
            
            return target.toInt();
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void toInt_DecimalPointInput_ThrowException()
        {
            toInt_GetValue("12.34");
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void toInt_DecimalCommaInput_ThrowException()
        {
            toInt_GetValue("12,34");
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        public void toInt_IntegerInput_ReturnCorrectValue()
        {
            int i = toInt_GetValue("1234");
            Assert.AreEqual<int>(1234, i);
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void toInt_EmptyString_ThrowException()
        {            
            int i = toInt_GetValue(String.Empty);
            Assert.AreEqual<int>(1234, i);
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void toInt_Null_ThrowException()
        {
            int i = toInt_GetValue(null);
        }

        #endregion toInt
        
        #region get

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void get_COMPOUNDNotReplicatedVariable_ThrowException()
        {
            FormDescBase fakeFormDesc = FakeFormDesc();
            fakeFormDesc.COMPOUND = true;
            JavaScriptExpObj target = new JavaScriptExpObj(null, _validatorStub, fakeFormDesc, 1, null);
            string s = target.get();
        }

        #endregion get
        
        #region setValue

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void setValueString_COMPOUNDNotReplicatedVariable_ThrowException()
        {
            FormDescBase fakeFormDesc = FakeFormDesc();
            fakeFormDesc.COMPOUND = true;
            JavaScriptExpObj target = new JavaScriptExpObj(null, _validatorStub, fakeFormDesc, 1, null);
            string s = target.setValue("123");
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void setValueString_NotAllowableValue_ThrowException()
        {
            FormDescBase fakeFormDesc = FakeFormDesc();
            IFormDescValidator validatorStub = new StubIFormDescValidator
            {
                ValidateObjectString = (validationData, value) => ValidationResult.Error("Test error")
            };
            
            JavaScriptExpObj target = new JavaScriptExpObj(null,validatorStub, fakeFormDesc, 1, null);
           
            string s = target.setValue("123");
        }


        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void setValueString_ReplicatedVariable_ThrowException()
        {
            FormDescBase fakeFormDesc = FakeFormDesc();
            fakeFormDesc.IsReplicated = true;
            JavaScriptExpObj target = new JavaScriptExpObj(null, _validatorStub, fakeFormDesc, 1, null);
            string s = target.setValue("123");
        }

        [TestMethod(), Owner(@"FIRM\SergeyL")]
        [ExpectedException(typeof(SchedulingScriptExecutionException))]
        public void setValueObject_COMPOUND_ThrowException()
        {
            FormDescBase fakeFormDesc = FakeFormDesc();
            fakeFormDesc.COMPOUND = true;
            JavaScriptExpObj target = new JavaScriptExpObj(null, _validatorStub, fakeFormDesc, 1, null);
            string s = target.setValue(new Object());
        }

        #endregion setValue

        #region toString
        [TestMethod(), Owner(@"FIRM\SergeyL")]
        public void toString_String_ReturnTheSameValue()
        {
            string s = "asdk212a;o1231";

            FormDescBase fakeFormDesc = FakeFormDesc();
            var dataService = new StubIInterviewFormDataService()
            {
                GetFormValueFormDescBaseStringArrayOfString = (form, cat, loops) => s
            };

            JavaScriptExpObj target = new JavaScriptExpObj(dataService, _validatorStub, fakeFormDesc, 1, null);
            string sr = target.toString();
            Assert.AreEqual<string>(s, sr);
        }
        
        #endregion toString*/

    }
}
