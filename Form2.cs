using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsGetTgs
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public int DieX { get { return int.Parse(tb_dieX.Text); } }
        public int DieY { get { return int.Parse(tb_dieY.Text); } }
        public int Scribe { get { return int.Parse(tb_scribe.Text); } }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
