using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.ServiceModel;
using System.ServiceModel.Description;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Activation;

using WcfTestServiceTest.ServiceReference1;

namespace WcfTestServiceTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Log(string msg)
        {
            rtbInfo.AppendText(DateTime.Now.ToString()+": ");
            rtbInfo.AppendText(msg+Environment.NewLine);
        }

        private void btnGetId_Click(object sender, EventArgs e)
        {
            try
            {
                Log("started");
                WcfTestServiceClient client = new WcfTestServiceClient("WSHttpBinding_IWcfTestService");
                {
                    if (!cbUseConfig.Checked)
                    {
                        client.Endpoint.Behaviors.Add(new ClientViaBehavior(new Uri(tbUrl.Text)));

                        WSHttpBinding b = client.Endpoint.Binding as WSHttpBinding;
                        if (b != null)
                        {
                            if (cbUseSsl.Checked)
                            {
                                b.Security.Mode = SecurityMode.Transport;
                                b.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                            }
                            else
                            {
                                b.Security.Mode = SecurityMode.None;

                            }
                        }
                    }                    
                    Log(client.GetIds());
                    client.Close();
                }

            }
            catch (Exception ex)
            {

                Log(ex.Message);
            }
            

        }

        private void cbUseConfig_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbInfo.Clear(); 
        }
    }
}
