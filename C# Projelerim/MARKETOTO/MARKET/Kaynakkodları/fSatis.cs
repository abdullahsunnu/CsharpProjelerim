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
    public partial class fSatis : Form
    {
        public fSatis()
        {
            InitializeComponent();
        }

        private void fSatis_Load(object sender, EventArgs e)
        {
            HizliButonDoldur();
            b5.Text = 5.ToString("C2"); // sağ tablo para birimi ayarı
            b10.Text = 10.ToString("C2");
            b20.Text = 20.ToString("C2");
            b50.Text = 50.ToString("C2");
            b100.Text = 100.ToString("C2");
            b200.Text = 200.ToString("C2");
        }
        private void HizliButonDoldur()
        {// foreachle listeyi gezdiriyoruz
            var hizliurun = db.HizliUrun.ToList();
            foreach (var item in hizliurun)
            {
                Button bH = this.Controls.Find("bH" + item.Id, true).FirstOrDefault() as Button;
                if (bH != null)
                {
                    double fiyat = Islemler.DoubleYap(item.Fiyat.ToString());
                    bH.Text = item.Urunad + "\n" + fiyat.ToString("C2");
                }
            }
        }
        private void HizliButonClick(object sender, EventArgs e)/***************************************************************/
        // hızlı ıslem butonları ıcın buton oluşturuldu
        {
            Button b = (Button)sender;
            int butonid = Convert.ToInt16(b.Name.ToString().Substring(2, b.Name.Length - 2));

            if (b.Text.ToString().StartsWith("-"))
            {
                fHizliUrunEkle f = new fHizliUrunEkle();
                f.lButonId.Text = butonid.ToString();
                f.ShowDialog();

            }
            else
            {

                var urunbarkod = db.HizliUrun.Where(a => a.Id == butonid).Select(a => a.Barkod).FirstOrDefault();
                var urun = db.Urun.Where(a => a.Barkod == urunbarkod).FirstOrDefault();
                UrunGetirListeye(urun, urunbarkod, Convert.ToDouble(tMiktar.Text));
                GenelToplam();

            }


        }

        marketotoEntities db = new marketotoEntities();
        private void tBarkod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barkod = tBarkod.Text.Trim();
                if (barkod.Length <= 2)
                {
                    tMiktar.Text = barkod;
                    tBarkod.Clear();
                    tBarkod.Focus();
                }
                else
                {
                    if (db.Urun.Any(a => a.Barkod == barkod))
                    {
                        var urun = db.Urun.Where(a => a.Barkod == barkod).FirstOrDefault();
                        UrunGetirListeye(urun, barkod, Convert.ToDouble(tMiktar.Text));
                        // Barkodlu Terazide de aNı kodlara ihtiyac duyuldugundan metod icinde aldım

                    }
                    //NORMAL BARKODLU URUNLERDE URUN YOKSA TERAZİ VERİ TABANINDAKİ URUNLERİ KONTROL ETMESİ İÇİN ELSE YAPISINA GİRİP KONTROL EDİYOR
                    else
                    {
                        int onek = Convert.ToInt32(barkod.Substring(0, 2));
                        if (db.Terazi.Any(a => a.TeraziOnEk == onek))
                        {
                            string teraziurunno = barkod.Substring(2, 5);//Urun on ekinden sonra ilk bes haneyi alması için 5 girdik
                            if (db.Urun.Any(a => a.Barkod == teraziurunno))
                            {
                                var urunterazi = db.Urun.Where(a => a.Barkod == teraziurunno).FirstOrDefault();
                                double miktarkg = Convert.ToDouble(barkod.Substring(7, 5)) / 1000;//Bin e bolme sebebimiz gram cinsini kg ye cevirme
                                UrunGetirListeye(urunterazi, teraziurunno, miktarkg);
                            }
                            else
                            {
                                Console.Beep(900, 2000);
                                MessageBox.Show("Kg urun ekleme sayfası");
                            }


                        }
                        else
                        {
                            Console.Beep(900, 2000);
                            MessageBox.Show("Normal urun ekleme sayfası");
                        }

                    }

                }
                gridSatisListesi.ClearSelection();
                

            }

        }

        private void UrunGetirListeye(Urun urun, string barkod, double miktar)
        {
            int satirsayisi = gridSatisListesi.Rows.Count;
            //double miktar = Convert.ToDouble(tMiktar.Text); YUKARDAN GELECEGİ İÇİN ARTIK İHTİYAC YOK
            bool eklenmismi = false;


            if (satirsayisi > 0)
            {
                for (int i = 0; i < satirsayisi; i++)
                {
                    if (gridSatisListesi.Rows[i].Cells["Barkod"].Value.ToString() == barkod)
                    {
                        gridSatisListesi.Rows[i].Cells["Miktar"].Value = miktar + Convert.ToDouble(gridSatisListesi.Rows[i].Cells["Miktar"].Value);
                        gridSatisListesi.Rows[i].Cells["Toplam"].Value = Math.Round(Convert.ToDouble(gridSatisListesi.Rows[i].Cells["Miktar"].Value) * Convert.ToDouble(gridSatisListesi.Rows[i].Cells["Fiyat"].Value), 2);
                        eklenmismi = true;
                    }

                }
            }
            if (!eklenmismi)
            {
                gridSatisListesi.Rows.Add();
                gridSatisListesi.Rows[satirsayisi].Cells["Barkod"].Value = barkod;
                gridSatisListesi.Rows[satirsayisi].Cells["UrunAdi"].Value = urun.UrunAd;
                gridSatisListesi.Rows[satirsayisi].Cells["UrunGrup"].Value = urun.UrunGrup;
                gridSatisListesi.Rows[satirsayisi].Cells["Birim"].Value = urun.Birim;
                gridSatisListesi.Rows[satirsayisi].Cells["Fiyat"].Value = urun.SatisFiyat;
                gridSatisListesi.Rows[satirsayisi].Cells["Miktar"].Value = miktar;
                gridSatisListesi.Rows[satirsayisi].Cells["Toplam"].Value = Math.Round(miktar * (double)urun.SatisFiyat, 2);
                gridSatisListesi.Rows[satirsayisi].Cells["AlisFiyati"].Value = urun.AlisFiyat;
                gridSatisListesi.Rows[satirsayisi].Cells["KdvTutari"].Value = urun.KdvTutari;
            }

        }

        private void GenelToplam()
        {


            double toplam = 0;
            for (int i = 0; i < gridSatisListesi.Rows.Count; i++)
            {
                toplam += Convert.ToDouble(gridSatisListesi.Rows[i].Cells["Toplam"].Value);
            }
            tGenelToplam.Text = toplam.ToString("C2");
            tMiktar.Text = "1";
            tBarkod.Clear();
            tBarkod.Focus();


        }

        private void gridSatisListesi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 9)//SUTUNLARDAN 6 INCIYA TIKLANINCA
            {
                gridSatisListesi.Rows.Remove(gridSatisListesi.CurrentRow);
                gridSatisListesi.ClearSelection();
                GenelToplam();
                tBarkod.Focus();
            }
        }

        private void bh_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button b = (Button)sender;
                if (!b.Text.StartsWith("-"))
                {
                    int butonid = Convert.ToInt16(b.Name.ToString().Substring(2, b.Name.Length - 2));
                    ContextMenuStrip s = new ContextMenuStrip();
                    ToolStripMenuItem sil = new ToolStripMenuItem();
                    sil.Text = "Temizle - Buton No:" + butonid.ToString();
                    sil.Click += Sil_Click;
                    s.Items.Add(sil);
                    this.ContextMenuStrip = s;
                }
            }

        }

        private void Sil_Click(object sender, EventArgs e)
        {
            int butonid = Convert.ToInt16(sender.ToString().Substring(19, sender.ToString().Length - 19));
            var guncelle = db.HizliUrun.Find(butonid);
            guncelle.Barkod = "-";
            guncelle.Urunad = "-";
            guncelle.Fiyat = 0;
            db.SaveChanges();
            double fiyat = 0;
            Button b = this.Controls.Find("bH" + butonid, true).FirstOrDefault() as Button;
            b.Text = "-" + "\n" + fiyat.ToString("C2");
        }

        private void bNx_Click(object sender, EventArgs e) //sağ tuş menüsü bağlantı
        {
            Button b = (Button)sender;
            if (b.Text == ",")
            {
                int virgul = tNumarator.Text.Count(x => x == ',');
                if (virgul < 1)
                {
                    tNumarator.Text += b.Text;
                }
            }
            else if (b.Text == "<")
            {
                if (tNumarator.Text.Length > 0)
                {
                    tNumarator.Text = tNumarator.Text.Substring(0, tNumarator.Text.Length - 1);
                }
            }
            else
            {
                tNumarator.Text += b.Text;
            }
        }

        private void bAdet_Click(object sender, EventArgs e)
        {
            if (tNumarator.Text != "")
            {
                tMiktar.Text = tNumarator.Text;
                tNumarator.Clear();
                tBarkod.Clear();
                tBarkod.Focus();
            }
        }

        private void bOdenen_Click(object sender, EventArgs e)
        {
            if (tNumarator.Text != "")
            {
                double sonuc = Islemler.DoubleYap(tNumarator.Text) - Islemler.DoubleYap(tGenelToplam.Text);
                tParaUstu.Text = sonuc.ToString("C2");
                tNumarator.Clear();
                tBarkod.Focus();
            }
        }

        private void bBarkod_Click(object sender, EventArgs e)
        {
            if (tNumarator.Text != "")
            {
                if (db.Urun.Any(a => a.Barkod == tNumarator.Text))
                {
                    var urun = db.Urun.Where(a => a.Barkod == tNumarator.Text).FirstOrDefault();
                    UrunGetirListeye(urun, tNumarator.Text, Convert.ToDouble(tMiktar.Text));
                    tNumarator.Clear();
                    GenelToplam();
                    tBarkod.Focus();
                }
                else
                {
                    MessageBox.Show("Ürün ekleme sayfasını aç");
                }
            }
        }

        private void ParaUstuHesapla_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            double sonuc = Islemler.DoubleYap(b.Text) - Islemler.DoubleYap(tGenelToplam.Text);
            tParaUstu.Text = sonuc.ToString("C2");
        }

        private void bDigerUrun_Click(object sender, EventArgs e)
        {
            if (tNumarator.Text!="")
            {
                int satirsayisi = gridSatisListesi.Rows.Count;
                gridSatisListesi.Rows.Add();
                gridSatisListesi.Rows[satirsayisi].Cells["Barkod"].Value = "1111111111116";
                gridSatisListesi.Rows[satirsayisi].Cells["UrunAdi"].Value = "Barkodsuz Ürün";
                gridSatisListesi.Rows[satirsayisi].Cells["UrunGrup"].Value = "Barkodsuz Ürün";
                gridSatisListesi.Rows[satirsayisi].Cells["Birim"].Value = "Adet";
                gridSatisListesi.Rows[satirsayisi].Cells["Miktar"].Value = 1;
                gridSatisListesi.Rows[satirsayisi].Cells["Fiyat"].Value = Convert.ToDouble(tNumarator.Text);
                gridSatisListesi.Rows[satirsayisi].Cells["KdvTutari"].Value = 0;
                gridSatisListesi.Rows[satirsayisi].Cells["Toplam"].Value = Convert.ToDouble(tNumarator.Text);
                tNumarator.Text = "";
                GenelToplam();
                tBarkod.Focus();



            }
        }

        private void bIade_Click(object sender, EventArgs e)
        {
            if (chSatisiadeIslemi.Checked)
            {
                chSatisiadeIslemi.Checked = false;
                chSatisiadeIslemi.Text = "Satış Yapılıyor";
            }
            else
            {
                chSatisiadeIslemi.Checked = true;
                chSatisiadeIslemi.Text = "İade İşlemi";

            }
        }

        private void bTemizle_Click(object sender, EventArgs e)
        {

            Temizle();
        }
        private void Temizle()
        {
            tMiktar.Text="1";
            tBarkod.Clear();
            tOdenen.Clear();
            tParaUstu.Clear();
            lNakit.Text = "0";
            lKart.Text = "0";
            tGenelToplam.Text = 0.ToString("C2");
            chSatisiadeIslemi.Checked = false;
            tNumarator.Clear();
            gridSatisListesi.Rows.Clear();
            tBarkod.Clear();
            tBarkod.Focus();
        }
        public void SatisYap(string odemesekli)
        {
            int satirsayisi = gridSatisListesi.Rows.Count;
            bool satisiade = chSatisiadeIslemi.Checked;
            double alisfiyattoplam = 0;
            if (satirsayisi>0)
            {
                int? islemno = db.Islem.First().Islem_No;
                Satis satis = new Satis();
                for (int i = 0; i < satirsayisi; i++)
                {
                    satis.IslemNo = islemno;
                    satis.UrunAd = gridSatisListesi.Rows[i].Cells["UrunAdi"].Value.ToString();
                    satis.UrunGrup = gridSatisListesi.Rows[i].Cells["UrunGrup"].Value.ToString();
                    satis.Barkod = gridSatisListesi.Rows[i].Cells["Barkod"].Value.ToString();
                    satis.Birim = gridSatisListesi.Rows[i].Cells["Birim"].Value.ToString();
                    satis.AlisFiyat =Islemler.DoubleYap( gridSatisListesi.Rows[i].Cells["AlisFiyati"].Value.ToString());
                    satis.SatisFiyat = Islemler.DoubleYap (gridSatisListesi.Rows[i].Cells["Fiyat"].Value.ToString());
                    satis.Miktar = Islemler.DoubleYap (gridSatisListesi.Rows[i].Cells["Miktar"].Value.ToString());
                    satis.Toplam = Islemler.DoubleYap (gridSatisListesi.Rows[i].Cells["Toplam"].Value.ToString());
                    satis.KdvTutari = Islemler.DoubleYap(gridSatisListesi.Rows[i].Cells["KdvTutari"].Value.ToString()) * Islemler.DoubleYap(gridSatisListesi.Rows[i].Cells["Miktar"].Value.ToString());
                    satis.OdemeSekli = odemesekli;
                    satis.Iade = satisiade;
                    satis.Tarih = DateTime.Now;
                    satis.Kullanici = lKullanici.Text;
                    db.Satis.Add(satis);
                    db.SaveChanges();

                    if (!satisiade)
                    {
                        Islemler.StokAzalt(gridSatisListesi.Rows[i].Cells["Barkod"].Value.ToString(), Islemler.DoubleYap(gridSatisListesi.Rows[i].Cells["Miktar"].Value.ToString()));
                    }
                    else
                    {
                        Islemler.StokArtir(gridSatisListesi.Rows[i].Cells["Barkod"].Value.ToString(), Islemler.DoubleYap(gridSatisListesi.Rows[i].Cells["Miktar"].Value.ToString()));
                    }
                    alisfiyattoplam += Islemler.DoubleYap(gridSatisListesi.Rows[i].Cells["AlisFiyati"].Value.ToString());
                }
                IslemOzet io = new IslemOzet();
                io.IslemNo = islemno;
                io.Iade = satisiade;
                io.AlisFiyatToplam = alisfiyattoplam;
                io.Gelir = false;
                io.Gider = false;
                if (!satisiade)
                {
                    io.Aciklama = odemesekli + "Satış";
                    
                }
                else
                {
                    io.Aciklama = "İade İşlemi(" + odemesekli + ")";
                }
                io.OdemeSekli = odemesekli;
                io.Kullanici = lKullanici.Text;
                io.Tarih = DateTime.Now;
                switch (odemesekli)
                {
                    case "Nakit":
                        io.Nakit = Islemler.DoubleYap(tGenelToplam.Text);
                        io.Kart = 0;break;
                        case "Kart":
                        io.Nakit = 0;
                        io.Kart = Islemler.DoubleYap(tGenelToplam.Text);break;
                    case "Kart-Nakit":
                        io.Nakit = Islemler.DoubleYap(lNakit.Text);
                        io.Kart = Islemler.DoubleYap(lKart.Text);break;
                }
                db.IslemOzet.Add(io);
                db.SaveChanges();

                var islemnoartir = db.Islem.First();
                islemnoartir.Islem_No += 1;
                db.SaveChanges();
                MessageBox.Show("Yazdırma İşlemi");
            }
        }

        private void bNakit_Click(object sender, EventArgs e)
        {
            SatisYap("Nakit");
        }

        private void bKart_Click(object sender, EventArgs e)
        {
            SatisYap("Kart");
        }

        private void bKartNakit_Click(object sender, EventArgs e)
        {
            fNakitKart f = new fNakitKart();
            f.ShowDialog();
        }

        private void tOdenen_TextChanged(object sender, EventArgs e)
        {

        }

        private void tBarkod_TextChanged(object sender, EventArgs e)
        {

        }

        private void tGenelToplam_TextChanged(object sender, EventArgs e)
        {

        }

        private void lNakit_Click(object sender, EventArgs e)
        {

        }

        private void button49_Click(object sender, EventArgs e)
        {

        }

        private void button53_Click(object sender, EventArgs e)
        {

        }
    }
}
