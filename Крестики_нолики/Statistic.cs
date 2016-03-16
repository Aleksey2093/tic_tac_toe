using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Крестики_нолики
{
    public partial class Statistic : Form
    {
        public Statistic()
        {
            InitializeComponent();
        }

        private void Statistic_Load(object sender, EventArgs e)
        {
            label1.Text = "Побед \n " + Properties.Settings.Default.Game_Win.ToString();
            label2.Text = "Поражений \n" + Properties.Settings.Default.Game_over.ToString();
            label3.Text = "Ничьи \n" + Properties.Settings.Default.Game_draw.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите сбросить статистику игры?", "Сброс", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Properties.Settings.Default.Game_Win = 0;
                Properties.Settings.Default.Game_over = 0;
                Properties.Settings.Default.Game_draw = 0;
                Properties.Settings.Default.Save();
                label1.Text = "Побед \n " + Properties.Settings.Default.Game_Win.ToString();
                label2.Text = "Поражений \n" + Properties.Settings.Default.Game_over.ToString();
            }
            else
            {

            }
        }
    }
}
