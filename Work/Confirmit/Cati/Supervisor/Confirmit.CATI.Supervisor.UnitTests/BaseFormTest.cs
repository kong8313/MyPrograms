using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;
using System.Web.UI.WebControls;
using System;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Supervisor.Classes.Fakes;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class BaseFormTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void FindControlsRecursive_SearchingParentControl_ReturnParentControl()
        {
            Control control = new Control { ID = "parent" };
            control.Controls.Add(new Control { ID = "child1" });

            List<Control> result = BaseForm.FindControlsRecursive(control, "parent");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("parent", result[0].ID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void FindControlsRecursive_SearchingChildControl_ReturnChildControl()
        {
            Control control = new Control { ID = "parent" };
            TextBox child2 = new TextBox { ID = "child2" };
            child2.Controls.Add(new TextBox { ID = "child3" });

            control.Controls.Add(new Control { ID = "child1" });
            control.Controls.Add(child2);

            List<Control> result = BaseForm.FindControlsRecursive(control, "child3");

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0] is TextBox);
            Assert.AreEqual("child3", result[0].ID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void FindControlsRecursive_SearchingChildControls_ReturnMultipleChildControls()
        {
            Control control = new Control { ID = "parent" };
            TextBox child2 = new TextBox { ID = "textbox" };
            child2.Controls.Add(new Control { ID = "child3" });
            child2.Controls.Add(new TextBox { ID = "textbox" });
            control.Controls.Add(new Control { ID = "child1" });
            control.Controls.Add(new TextBox { ID = "textbox" });
            control.Controls.Add(child2);

            List<Control> result = BaseForm.FindControlsRecursive(control, "textbox");

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void FindControlsRecursive_NoControlsWithSpecifiedID_ReturnEmptyList()
        {
            Control control = new Control { ID = "parent" };
            TextBox child2 = new TextBox { ID = "textbox" };
            child2.Controls.Add(new Control { ID = "child3" });
            child2.Controls.Add(new TextBox { ID = "textbox" });
            control.Controls.Add(new Control { ID = "child1" });
            control.Controls.Add(new TextBox { ID = "textbox" });
            control.Controls.Add(child2);

            List<Control> result = BaseForm.FindControlsRecursive(control, "label");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindControlsRecursive_NullParent_ArgumentNullException()
        {
            BaseForm.FindControlsRecursive(null, "label");
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindControlsRecursive_NullID_ArgumentNullException()
        {
            BaseForm.FindControlsRecursive(new Control(), null);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindControlsRecursive_EmptyID_ArgumentNullException()
        {
            BaseForm.FindControlsRecursive(new Control(), string.Empty);
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SendToClient_SendFile_FileDeletedAfterSending()
        {
            string tempFileNameWithPath = string.Empty;
            try
            {
                tempFileNameWithPath = Path.GetTempFileName();

                var stub = new StubIFileToBrowserSender();
                var baseForm = new BaseForm(stub, new PgpEncryptionService(new StubISecuritySettings(), new StubIConfirmitEncryptionSettingProvider()));
                stub.SendBaseFormArrayOfByteStringBoolean = (page, buffer, name, inline) => { };

                baseForm.FileToClientSender.SendFileContent(tempFileNameWithPath, Path.GetFileName(tempFileNameWithPath));

                Assert.IsFalse(File.Exists(tempFileNameWithPath), "File was not deleted after sending to client.");

            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFileNameWithPath) && File.Exists(tempFileNameWithPath))
                {
                    File.Delete(tempFileNameWithPath);
                }
            }
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SendToClient_SendFileWithTimeStamp_FileDeletedAfterSending()
        {
            string tempFileNameWithPath = string.Empty;
            try
            {
                tempFileNameWithPath = Path.GetTempFileName();

                var stub = new StubIFileToBrowserSender();
                var baseForm = new BaseForm(stub, new PgpEncryptionService(new StubISecuritySettings(), new StubIConfirmitEncryptionSettingProvider()));
                stub.SendBaseFormArrayOfByteStringBoolean = (page, buffer, name, inline) => { };

                baseForm.FileToClientSender.SendWithTimeStamp(tempFileNameWithPath, Path.GetFileName(tempFileNameWithPath));

                Assert.IsFalse(File.Exists(tempFileNameWithPath), "File was not deleted after sending to client.");

            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFileNameWithPath) && File.Exists(tempFileNameWithPath))
                {
                    File.Delete(tempFileNameWithPath);
                }
            }
        }
    }
}