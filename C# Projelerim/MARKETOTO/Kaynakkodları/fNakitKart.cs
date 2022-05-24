using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MARKET
{
    public partial class fNakitKart : Form
    {
        public fNakitKart()
        {
            InitializeComponent();
        }

        private void tNakit_KeyDown(object sender, KeyEventArgs e)
        {
            if (tNakit.Text!="")
            {
                if (e.KeyCode==Keys.Enter)
                {
                    fSatis f = (fSatis)Application.OpenForms["Fsatis"];
                    double nakit = Islemler.DoubleYap(tNakit.Text);
                    double geneltoplam = Islemler.DoubleYap(f.tGenelToplam.Text);
                    double kart = geneltoplam - nakit;
                    f.lNakit.Text = nakit.ToString("C2");
                    f.lKart.Text = kart.ToString("C2");
                    f.SatisYap("Kart-Nakit");
                    this.Hide();
                }
            }
        }
    }
}
