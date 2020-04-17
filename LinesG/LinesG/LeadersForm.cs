using System;
using System.Windows.Forms;

namespace LinesG
{
    public partial class LeadersForm : Form
    {
        private Leaders _leaders;

        public LeadersForm(Leaders leaders)
        {
            InitializeComponent();

            _leaders = leaders;
        }

        private void LeadersForm_Shown(object sender, EventArgs e)
        {
            dataGridViewLeaders.Rows.Clear();

            foreach (var leader in _leaders.LeadersList)
            {
                dataGridViewLeaders.Rows.Add(new object[] { leader.Name, leader.Score, TimeConverter.Convert(leader.TimeInSec) });
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
