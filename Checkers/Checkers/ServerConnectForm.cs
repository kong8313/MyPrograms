using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections;

namespace Checkers
{
    public partial class ServerConnectForm : Form
    {
        MainForm mf;

        public ServerConnectForm(MainForm mainForm)
        {
            InitializeComponent();
            mf = mainForm;
            mf.ipConnect = "";
            // Загрузка откуда-нибудь 10-ти последних введённых адресов
            GetLastValues(comboBoxIpConnect);
        }

        // Закрытие формы
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Подключение к серверу
        private void button1_Click(object sender, EventArgs e)
        {
            mf.ipConnect = comboBoxIpConnect.Text;
            SetLastValues(comboBoxIpConnect.Text);
            this.Close();
        }

        // Отлов нажатия кнопок на форме
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!button1.Focused && !button2.Focused)
            {
                if (keyData == Keys.Enter)
                {
                    button1_Click(new object(), new EventArgs());
                    return true;
                }
                else if (keyData == Keys.Escape)
                {
                    button2_Click(new object(), new EventArgs());
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        #region Сохранение и получение 10 последних введённых ip-адресов
        /// <summary>
        /// Запись десяти последних использованных значений в переданный ComboBox
        /// </summary>
        /// <param name="comboBox">ComboBox, в который записывать значения</param>
        public void GetLastValues(ComboBox comboBox)
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.CreateSubKey(@"Software\GrigoriyCheckers\LastUsed");

                string s = "";
                s = (string)regKey.GetValue("ipConnect", s);

                if (s != "")
                {
                    comboBox.Items.Clear();
                    ArrayList values = ParseString(s, "^#!^");
                    for (int i = 0; i < values.Count; i++)
                        comboBox.Items.Add((string)values[i]);
                    if (comboBox.Items.Count > 0)
                        comboBox.Text = comboBox.Items[0].ToString();
                }
            }
            catch { }
        }

        /// <summary>
        /// Добавление нового значения к списку 10-ти последних использованных значений 
        /// </summary>
        /// <param name="value">Добавляемое значение</param>
        public void SetLastValues(string value)
        {
            if (value == "")
                return;

            try
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey = regKey.CreateSubKey(@"Software\GrigoriyCheckers\LastUsed");

                string s = "";
                s = (string)regKey.GetValue("ipConnect", s);

                if (s != "")
                {
                    ArrayList values = ParseString(s, "^#!^");

                    for (int i = 0; i < values.Count; i++)
                        if (value == (string)values[i])
                            values.RemoveAt(i);

                    s = value;
                    for (int i = 0; i < values.Count && i < 9; i++)
                        s += "^#!^" + (string)values[i];

                    regKey.SetValue("ipConnect", s);
                }
                else
                {
                    regKey.SetValue("ipConnect", value);
                }
            }
            catch { }
        }

        /// <summary>
        /// Разделяет строку s на подстроки, разделенные подстрокой subS
        /// </summary>
        /// <param name="s">Строка</param>
        /// <param name="subS">Подстрока для разделения</param>
        /// <returns>Массив строк, записанный в динамическом массиве</returns>
        public ArrayList ParseString(string s, string subS)
        {
            ArrayList mas = new ArrayList();
            if (subS == "")
            {
                mas.Add(s);
                return mas;
            }

            while (s != "")
            {
                int n = s.IndexOf(subS, 0);
                if (n == -1)
                {
                    mas.Add(s);
                    break;
                }
                else
                {
                    mas.Add(s.Substring(0, n));
                    s = s.Substring(n + subS.Length, s.Length - n - subS.Length);
                }
            }
            return mas;
        }
        #endregion
    }
}