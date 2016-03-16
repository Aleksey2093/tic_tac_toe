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
    public partial class Connect : Form
    {
        public Connect()
        {
            InitializeComponent();
        }
        static public string ip;
        private void button1_Click(object sender, EventArgs e)
        {
            ip = textBox1.Text;
            this.Close(); //закрыть окно
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            prov(textBox1.Text);
        }



        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            prov(textBox1.Text);
        }
        private void prov(string tex)
        {
            bool ok = true;
            if (tex.Length >= "0.0.0.0".Length && tex.Length <= "254.254.254.254".Length &&
                tex[0] != '.' && tex[tex.Length - 1] != '.')
            {
                int tochka = 0; string chisel = null;
                for (int i = 0;i<tex.Length;i++)
                {
                    if (tex[i] == '.')
                    {
                        chisel = null; tochka++; if (tochka > 3) { ok = false; goto ifi; }
                    }
                    else
                    {
                        int tmp; chisel += tex[i].ToString();
                        if (chisel.Length <= 3 && Int32.TryParse(chisel,out tmp))
                        {
                            if (tmp < 0 || tmp > 254) { ok = false; goto ifi; }
                        }
                        else { ok = false; goto ifi; }
                    }
                }
                if (tochka != 3) { ok = false; }
            }
            else { ok = false; }

            ifi:
            if (ok == true) { button1.Enabled = true; }
            else { button1.Enabled = false; }
        }
    }
}
