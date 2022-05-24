using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;

namespace MARKET
{
    
    public partial class giriscs : Form
    {
       
        public giriscs()
        {
            InitializeComponent();
        }

        private void giriscs_Load(object sender, EventArgs e)
        {

        }
        
        private void button1_Click(object sender, EventArgs e)

        {
            fSatis frm = new fSatis();
            try
            {
                using (var db = new marketotoEntities())
                {
                    if (db.Kullanici.Any())
                    {
                        var bak = db.Kullanici.Where(x => x.kullanici_adi == textBox1.Text && x.sifre == textBox2.Text).FirstOrDefault();
                        if (bak != null)
                        {
                            //*MessageBox.Show("asd");

                            
                            frm.Show();
                            this.Hide();



                            /* Cursor.Current = Cursors.WaitCursor;
                             fSatis f = new fSatis();
                             Cursor.Current = Cursors.Default;
                            */
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
