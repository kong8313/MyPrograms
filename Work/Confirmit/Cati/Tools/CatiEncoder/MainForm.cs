using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using CatiEncoder.Properties;
using Confirmit.CATI.Common.Monitoring;
using Confirmit.Configuration;
using Confirmit.Databases;
using Confirmit.DataServices.RDataAccess;
using Confirmit.Security.Crypto;
using Confirmit.Security.Crypto.Web;
using Microsoft.Win32;

namespace CatiEncoder
{
    public partial class MainForm : Form
    {
        private string _confirmitServerName;
        private string _confirmitCredentials;
        private string _confirmConnectionString => $"Data Source={_confirmitServerName};Database=confirm;{_confirmitCredentials}";
        private string _catiConnectionString;        

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainFormShown(object sender, EventArgs e)
        {
            FillConnectionStrings();
        }

        
        private void FillConnectionStrings()
        {
            try
            {
                ClearTexts();                

                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FIRM\ConFIRM"))
                {
                    _confirmitServerName = (string)regKey.GetValue("SQLServerName");
                }

                textBoxCurrentConfirmitSqlServerName.Text = textBoxNewConfirmitSqlServerName.Text = _confirmitServerName;

                string encryptedConfirmitCredentials = null;
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FIRM\ConFIRM\SqlSettings"))
                {
                    encryptedConfirmitCredentials = (string)regKey.GetValue("SurveyC");
                }

                _confirmitCredentials = new CryptComp().Decrypt(encryptedConfirmitCredentials);
                textBoxCurrentSurveyC.Text = encryptedConfirmitCredentials;
                textBoxNewSurveyC.Text = _confirmitCredentials;
                buttonEncryptConfirmitSettings.Enabled = true;

                var dbEngine = new DatabaseEngine(_confirmConnectionString);
                var catiServerName = dbEngine.ExecuteScalar<string>("SELECT [ConfigValue] FROM [confirm].[dbo].[CfgConfig] where ConfigId=623");
                textBoxCurrentCatiSqlServerName.Text = textBoxNewCatiSqlServerName.Text = catiServerName;
                buttonEncryptCatiSqlServerName.Enabled = true;

                FillConnectionStringsFromDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearTexts()
        {
            textBoxCurrentSurveyC.Text = textBoxCurrentCatiSqlServerName.Text = textBoxCurrentConfirmConnectionString.Text =
            textBoxCurrentConfirmlogConnectionString.Text = textBoxCurrentSessionStateConnectionString.Text = 
            textBoxNewConfirmitSqlServerName.Text = textBoxNewCatiSqlServerName.Text = textBoxNewConfirmConnectionString.Text =
            textBoxNewConfirmlogConnectionString.Text = textBoxNewSessionStateConnectionString.Text = string.Empty;

            buttonEncryptConfirmitSettings.Enabled = buttonEncryptCatiSqlServerName.Enabled = buttonEncryptConfirmConnectionString.Enabled = 
            buttonEncryptConfirmlogConnectionString.Enabled = buttonEncryptSessionStateConnectionString.Enabled = false;
        }

        private void FillConnectionStringsFromDB()
        {
            new ConfigurationLoader().ForceConfigurationRefresh();
            _catiConnectionString = DbLib.GetCatiDefaultConnectInfo().GetConnectString();

            var dbEngine = new DatabaseEngine(_catiConnectionString);
            textBoxCurrentConfirmConnectionString.Text = dbEngine.ExecuteScalar<string>("select Value from BvSystemSettings where SystemName = 'Setup.EncryptedConfirmConnectionString'");
            textBoxCurrentConfirmlogConnectionString.Text = dbEngine.ExecuteScalar<string>("select Value from BvSystemSettings where SystemName = 'Setup.EncryptedConfirmlogConnectionString'");
            textBoxCurrentSessionStateConnectionString.Text = dbEngine.ExecuteScalar<string>("select Value from BvSystemSettings where SystemName = 'Setup.EncryptedSessionStateConnectionString'");

            DecodeValue(textBoxCurrentConfirmConnectionString.Text, textBoxNewConfirmConnectionString, buttonEncryptConfirmConnectionString);
            DecodeValue(textBoxCurrentConfirmlogConnectionString.Text, textBoxNewConfirmlogConnectionString, buttonEncryptConfirmlogConnectionString);
            DecodeValue(textBoxCurrentSessionStateConnectionString.Text, textBoxNewSessionStateConnectionString, buttonEncryptSessionStateConnectionString);
        }

        private void DecodeValue(string encodedValue, TextBox textBox, Button encryptButton)
        {
            try
            {
                textBox.Text = EncryptionUsingMachineKey.Decrypt(DataProtection.All, encodedValue);
                encryptButton.Enabled = true;
            }
            catch
            {
                textBox.Text = encodedValue;
            }
        }

        private void buttonEncryptConfirmitConnectionString_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseEngine dbEngine = null;
                string newConnectionString = $"Data Source={textBoxNewConfirmitSqlServerName.Text};{textBoxNewSurveyC.Text}";
                try
                {
                    dbEngine = new DatabaseEngine(newConnectionString);
                }
                catch
                {
                    if (DialogResult.No == MessageBox.Show($"Looks like new Confirmit SQL parameters are wrong because connection to '{newConnectionString}' can't be established. Are you shure you want to change these values?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        return;
                    }
                }

                if (DialogResult.No == MessageBox.Show("These parameters are controlled by Confirmit Octopus build and will be overridden during next deployment or clean install. Are you shure you want to change these values?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return;
                }

                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FIRM\ConFIRM", true))
                {
                    regKey.SetValue("SQLServerName", textBoxNewConfirmitSqlServerName.Text);
                }

                string encryptedCredentials = new CryptComp().Encrypt(textBoxNewSurveyC.Text);
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FIRM\ConFIRM\SqlSettings", true))
                {
                    regKey.SetValue("SurveyC", encryptedCredentials);
                }

                MessageBox.Show("Value was changed successfully. Other connection strings will be updated.", Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);

                FillConnectionStrings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEncryptCatiSqlServerNameClick(object sender, EventArgs e)
        {
            try
            {
                DatabaseEngine dbEngine = null;
                var scsb = new SqlConnectionStringBuilder(_catiConnectionString)
                {
                    DataSource = textBoxNewCatiSqlServerName.Text
                };

                try
                {
                    
                    dbEngine = new DatabaseEngine(scsb.ConnectionString);
                }
                catch
                {
                    if (DialogResult.No == MessageBox.Show($"Looks like new CATI SQL server name is wrong because connection to '{scsb.ConnectionString}' can't be established. Are you shure you want to change this value?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        return;
                    }
                }

                scsb.DataSource = _confirmitServerName;
                scsb.InitialCatalog = "confirm";
                dbEngine = new DatabaseEngine(scsb.ConnectionString);
                dbEngine.ExecuteNonQuery($"update [confirm].[dbo].[CfgConfig] set [ConfigValue] = '{textBoxNewCatiSqlServerName.Text}' where ConfigId=623");

                MessageBox.Show("Value was changed successfully. Other connection strings will be updated.", Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);

                FillConnectionStrings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEncryptConfirmConnectionStringClick(object sender, EventArgs e)
        {
            try
            {
                var dbEngine = new DatabaseEngine(_catiConnectionString);
                textBoxCurrentConfirmConnectionString.Text = EncryptionUsingMachineKey.Encrypt(DataProtection.All, textBoxNewConfirmConnectionString.Text);
                dbEngine.ExecuteNonQuery($"update BvSystemSettings set Value = '{textBoxCurrentConfirmConnectionString.Text}' where SystemName = 'Setup.EncryptedConfirmConnectionString'");

                MessageBox.Show("Value was changed successfully", Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEncryptConfirmlogConnectionStringClick(object sender, EventArgs e)
        {
            try
            {
                var dbEngine = new DatabaseEngine(_catiConnectionString);
                textBoxCurrentConfirmlogConnectionString.Text = EncryptionUsingMachineKey.Encrypt(DataProtection.All, textBoxNewConfirmlogConnectionString.Text);
                dbEngine.ExecuteNonQuery($"update BvSystemSettings set Value = '{textBoxCurrentConfirmlogConnectionString.Text}' where SystemName = 'Setup.EncryptedConfirmlogConnectionString'");

                MessageBox.Show("Value was changed successfully", Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEncryptSessionStateConnectionStringClick(object sender, EventArgs e)
        {
            try
            {
                var dbEngine = new DatabaseEngine(_catiConnectionString);
                textBoxCurrentSessionStateConnectionString.Text = EncryptionUsingMachineKey.Encrypt(DataProtection.All, textBoxNewSessionStateConnectionString.Text);
                dbEngine.ExecuteNonQuery($"update BvSystemSettings set Value = '{textBoxCurrentSessionStateConnectionString.Text}' where SystemName = 'Setup.EncryptedSessionStateConnectionString'");

                MessageBox.Show("Value was changed successfully", Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonLostChangesWarning_MouseEnter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxNewCatiSqlServerName.Text))
            {
                toolTip1.Show("Warning! All changes in other text boxes will be lost!", (Button)sender, 10, -18);
            }
        }

        private void buttonLostChangesWarning_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide((Button)sender);
        }

        private void ButtonEncryptClick(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonMachineKeyEncryptionMonitorIdentInfo.Checked)
                {
                    MessageBox.Show(Resources.ThisModeSupportsDecryptionOnly, Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (radioButtonCryptComp.Checked)
                {
                    richTextBoxEncryptedString.Text = new CryptComp().Encrypt(richTextBoxDecryptedString.Text);
                }
                else if (radioButtonSecurityHelper.Checked)
                {
                    richTextBoxEncryptedString.Text = SecurityHelper.EncryptConfigPassword(richTextBoxDecryptedString.Text);
                }
                else
                {
                    richTextBoxEncryptedString.Text = WebUtility.UrlEncode(EncryptionUsingMachineKey.Encrypt(Confirmit.Security.Crypto.Web.DataProtection.All, richTextBoxDecryptedString.Text));
                }
            }
            catch (Exception ex)
            {
                richTextBoxEncryptedString.Text = ex.Message;
            }
        }

        private void ButtonDecryptClick(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonMachineKeyEncryptionMonitorIdentInfo.Checked)
                {
                    richTextBoxDecryptedString.Text = DecryptMonitoringIdentityInfo(richTextBoxEncryptedString.Text);
                }
                else if (radioButtonCryptComp.Checked)
                {
                    richTextBoxDecryptedString.Text = new CryptComp().Decrypt(richTextBoxEncryptedString.Text);
                }
                else if (radioButtonSecurityHelper.Checked)
                {
                    richTextBoxDecryptedString.Text = SecurityHelper.DecryptConfigPassword(richTextBoxEncryptedString.Text);
                }
                else
                {
                    richTextBoxDecryptedString.Text = EncryptionUsingMachineKey.Decrypt(Confirmit.Security.Crypto.Web.DataProtection.All, WebUtility.UrlDecode(richTextBoxEncryptedString.Text));
                }
            }
            catch (Exception ex)
            {
                richTextBoxDecryptedString.Text = ex.Message;
            }
        }

        private string DecryptMonitoringIdentityInfo(string encryptedString)
        {
            MonitoringIdentityInfo identityInfo;

            byte[] decryptedData = EncryptionUsingMachineKey.DecryptAsByteArray(DataProtection.All, encryptedString);

            using (var memoryStream = new MemoryStream(decryptedData))
            {
                identityInfo = ((MonitoringIdentityInfo)new BinaryFormatter().Deserialize(memoryStream));
            }

            var sb = new StringBuilder();
            PropertyInfo[] properties = identityInfo.GetType().GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                object value = propertyInfo.GetValue(identityInfo, new object[] { });

                sb.AppendFormat("{0}={1}\r\n", propertyInfo.Name,
                    value.GetType() == (typeof(DateTime))
                        ? ((DateTime)value).ToString("dd-MM-yyyy HH:mm:ss")
                        : value);
            }

            return sb.ToString().TrimEnd('\r', '\n');
        }
    }
}
