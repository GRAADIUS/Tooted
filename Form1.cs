using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Aspose.Pdf;
using Microsoft.VisualBasic;
using Image = System.Drawing.Image;

namespace Tooted
{
    public partial class Form1 : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=HP-CZC2349HSZ;Initial Catalog=Tooded_DB;Integrated Security=True");

        SqlDataAdapter adapter_toode, adapter_kategooria;
        SqlCommand command;
        public Form1()
        {
            InitializeComponent();
            NaitaAndmed();
            NaitaKategooriad();
        }

        string kat;
        SaveFileDialog save;
        OpenFileDialog open;
        string extension = null;
        int Id = 0;

        public void NaitaKategooriad()
        {
            connect.Open();
            adapter_kategooria = new SqlDataAdapter("SELECT Id,Kategooria_nimetus FROM Kategooriatabel", connect);
            DataTable dt_kat = new DataTable();
            adapter_kategooria.Fill(dt_kat);
            foreach (DataRow item in dt_kat.Rows)
            {
                if (!comboBox1.Items.Contains(item["Kategooria_nimetus"]))
                {
                    comboBox1.Items.Add(item["Kategooria_nimetus"]);
                }
                else
                {
                    command = new SqlCommand("DELETE FROM Kategooriatabel WHERE Id=@id", connect);
                    command.Parameters.AddWithValue("@id", item["Id"]);
                    command.ExecuteNonQuery();
                }

            }
            connect.Close();
        }

        private void button1_Click(object sender, EventArgs e) //lisa kat
        {
            bool on = false;
            foreach (var item in comboBox1.Items)
            {
                if (item.ToString() == comboBox1.Text)
                {
                    on = true;
                }
            }
            if (on == false)
            {
                command = new SqlCommand("INSERT INTO Kategooriatabel (Kategooria_nimetus) VALUES (@kat)", connect);
                connect.Open();
                command.Parameters.AddWithValue("@kat", comboBox1.Text);
                command.ExecuteNonQuery();
                connect.Close();
                comboBox1.Items.Clear();
                NaitaKategooriad();
            }
            else
            {
                MessageBox.Show("Selline kategooriat on juba olemas!");
            }
        }

        private void button2_Click(object sender, EventArgs e) //kasuta kat
        {
            if (comboBox1.SelectedItem != null)
            {
                connect.Open();
                string kat_val = comboBox1.SelectedItem.ToString();
                command = new SqlCommand("DELETE FROM Kategooriatabel WHERE Kategooria_nimetus=@kat ", connect);
                command.Parameters.AddWithValue("@kat", kat_val);
                command.ExecuteNonQuery();
                connect.Close();
                comboBox1.Items.Clear();
                NaitaKategooriad();
            }
        }

        private void button3_Click(object sender, EventArgs e) //otsi
        {
            open = new OpenFileDialog();
            open.InitialDirectory = @"C:\Users\marina.oleinik\Pictures";
            open.Multiselect = true;
            open.Filter = "Images Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";

            FileInfo open_info = new FileInfo(@"C:\Users\marina.oleinik\Pictures\" + open.FileName);
            if (open.ShowDialog() == DialogResult.OK && textBox1.Text != null)
            {
                save = new SaveFileDialog();
                save.InitialDirectory = Path.GetFullPath(@"..\..\Images");
                extension = Path.GetExtension(open.FileName);
                save.FileName = textBox1.Text + Path.GetExtension(open.FileName);//extension
                save.Filter = "Images" + Path.GetExtension(open.FileName) + "|" + Path.GetExtension(open.FileName);
                if (save.ShowDialog() == DialogResult.OK && textBox1.Text != null)
                {

                    File.Copy(open.FileName, save.FileName);//??
                    pictureBox1.Image = Image.FromFile(save.FileName);
                }
            }
            else
            {
                MessageBox.Show("Puudub toode nimetus või oli vajutatud Cancel");
            }

        }

        private void button4_Click(object sender, EventArgs e) //lisa
        {
            if (textBox1.Text.Trim() != string.Empty && textBox3.Text.Trim() != string.Empty && textBox2.Text.Trim() != string.Empty && comboBox1.SelectedItem != null)
            {
                try
                {
                    connect.Open();

                    command = new SqlCommand("SELECT Id FROM Kategooriatabel WHERE Kategooria_nimetus=@kat", connect);
                    command.Parameters.AddWithValue("@kat", comboBox1.Text);
                    command.ExecuteNonQuery();
                    Id = Convert.ToInt32(command.ExecuteScalar());

                    command = new SqlCommand("INSERT INTO Toodetabel (Toodenimetus,Kogus,Hind,Pilt,KategooriadID) VALUES (@toode,@kogus,@hind,@pilt,@kat)", connect);
                    command.Parameters.AddWithValue("@toode", textBox1.Text);
                    command.Parameters.AddWithValue("@kogus", textBox3.Text);
                    command.Parameters.AddWithValue("@hind", textBox2.Text);
                    command.Parameters.AddWithValue("@pilt", textBox1.Text + extension);//jpg-png
                    command.Parameters.AddWithValue("@kat", Id);//Id?
                    command.ExecuteNonQuery();

                    connect.Close();

                    NaitaAndmed();
                }
                catch (Exception)
                {

                    MessageBox.Show("Andmebaasiga viga!");
                }
            }
            else
            {
                MessageBox.Show("Sisesta andmeid!");
            }

        }

        private void button5_Click(object sender, EventArgs e) //uuenda
        {

            if (textBox1.Text != "" && textBox3.Text != "" && textBox2.Text != "" && pictureBox1.Image != null)
            {
                command = new SqlCommand("UPDATE Toodetabel  SET Toodenimetus=@toode,Kogus=@kogus,Hind=@hind, Pilt=@pilt WHERE Id=@id", connect);
                connect.Open();
                command.Parameters.AddWithValue("@id", Id);
                command.Parameters.AddWithValue("@toode", textBox1.Text);
                command.Parameters.AddWithValue("@kogus", textBox3.Text);
                command.Parameters.AddWithValue("@hind", textBox2.Text.Replace(",", "."));

                string pilt = dataGridView1.SelectedRows[0].Cells["Pilt"].Value.ToString();
                string file_pilt = textBox1.Text + extension;//kontroll


                command.Parameters.AddWithValue("@pilt", file_pilt);
                command.ExecuteNonQuery();
                connect.Close();
                NaitaAndmed();

                MessageBox.Show("Andmed uuendatud");
            }
            else
            {
                MessageBox.Show("Viga");
            }
        }

        private void button6_Click(object sender, EventArgs e) //kustuta
        {
            Id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
            MessageBox.Show(Id.ToString());
            if (Id != 0)
            {
                command = new SqlCommand("DELETE Toodetabel WHERE Id=@id", connect);
                connect.Open();
                command.Parameters.AddWithValue("@id", Id);
                command.ExecuteNonQuery();
                connect.Close();

                NaitaAndmed();

                MessageBox.Show("Andmed tabelist Tooded on kustutatud");
            }
            else
            {
                MessageBox.Show("Viga Tooded tabelist andmete kustutamisega");
            }
        }

        public void NaitaAndmed()
        {
            connect.Open();
            DataTable dt_toode = new DataTable();
            adapter_toode = new SqlDataAdapter("SELECT Toodetabel.Id,Toodetabel.Toodenimetus,Toodetabel.Kogus,Toodetabel.Hind,Toodetabel.Pilt, Kategooriatabel.Kategooria_nimetus as Kategooria_nimetus  FROM Toodetabel INNER JOIN Kategooriatabel on Toodetabel.KategooriadID=Kategooriatabel.Id ", connect);
            adapter_toode.Fill(dt_toode);
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = dt_toode;
            DataGridViewComboBoxColumn combo_kat = new DataGridViewComboBoxColumn();
            combo_kat.DataPropertyName = "Kategooria_nimetus";
            HashSet<string> keys = new HashSet<string>();
            foreach (DataRow item in dt_toode.Rows)
            {
                string kat_n = item["Kategooria_nimetus"].ToString();
                if (!keys.Contains(kat_n))
                {
                    keys.Add(kat_n);
                    combo_kat.Items.Add(kat_n);
                }
            }
            dataGridView1.Columns.Add(combo_kat);
            pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Images"), "epood.png"));
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            connect.Close();
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Id = (int)dataGridView1.Rows[e.RowIndex].Cells["Id"].Value;
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Toodenimetus"].Value.ToString();
            textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["Kogus"].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["Hind"].Value.ToString();
            try
            {
                pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Images"), dataGridView1.Rows[e.RowIndex].Cells["Pilt"].Value.ToString()));
            }
            catch (Exception)
            {

                pictureBox1.Image = Image.FromFile(Path.Combine(Path.GetFullPath(@"..\..\Images"), "epood.png"));
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            comboBox1.SelectedItem = dataGridView1.Rows[e.RowIndex].Cells[5].Value;//?
        }
        private void Kust_btn_Click(object sender, DataGridViewCellMouseEventArgs e) //Kustuta toode
        {
            Id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
            MessageBox.Show(Id.ToString());
            if (Id != 0)
            {
                command = new SqlCommand("DELETE Toodetabel WHERE Id=@id", connect);
                connect.Open();
                command.Parameters.AddWithValue("@id", Id);
                command.ExecuteNonQuery();
                connect.Close();

                NaitaAndmed();

                MessageBox.Show("Andmed tabelist Tooded on kustutatud");
            }
            else
            {
                MessageBox.Show("Viga Tooded tabelist andmete kustutamisega");
            }
        }
        private void Kust_btn_Click(object sender, EventArgs e)
        {
            Id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
            MessageBox.Show(Id.ToString());
            if (Id != 0)
            {
                command = new SqlCommand("DELETE Toodetabel WHERE Id=@id", connect);
                connect.Open();
                command.Parameters.AddWithValue("@id", Id);
                command.ExecuteNonQuery();
                connect.Close();

                NaitaAndmed();

                MessageBox.Show("Andmed tabelist Tooded on kustutatud");
            }
            else
            {
                MessageBox.Show("Viga Tooded tabelist andmete kustutamisega");
            }
        }
        private void eemalda_btn_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox3.Text = "";
            textBox2.Text = "";
            pictureBox1.Image = null;
        }
        Document document;
        private void Ostan_btn_Click(object sender, EventArgs e)//arve koostamine
        {
            document = new Document();//using Aspose.Pdf
            var page = document.Pages.Add();
            page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment("Toode  Hind  Kogus Summa"));
            foreach (var toode in Tooded_list)
            {
                page.Paragraphs.Add(new Aspose.Pdf.Text.TextFragment(toode));//
            }
            document.Save(@"..\..\Arved\Arve_.pdf");
            document.Dispose();
        }
        List<string> Tooded_list = new List<string>();//tooded listisse
        private void Valik_btn_Click(object sender, EventArgs e)
        {
            Tooded_list.Add("-----------------------");
            Tooded_list.Add((textBox1.Text + "  " + textBox2.Text + "  " + textBox3.Text + "  " + (Convert.ToInt32(textBox3.Text.ToString()) * Convert.ToInt32(textBox2.Text.ToString()))).ToString());
        }

        private void SaadaArve_btn_Click(object sender, EventArgs e)
        {
            string adress = Interaction.InputBox("Sisesta e-mail", "Kuhu saada", "egorfedorenko@gmail.com.ee");
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    //string password = Interaction.InputBox("Sisesta salasõna");
                    Credentials = new System.Net.NetworkCredential("mvc.programmeerimine@gmail.com", "3.Kuursus"), //kellelt email,password
                    EnableSsl = true
                };
                mail.From = new MailAddress("mvc.programmeerimine@gmail.com");
                mail.To.Add(adress);//kellele
                mail.Subject = "Arve";
                mail.Body = "Arve on ostetud ja ta on maanuses";
                mail.Attachments.Add(new Attachment(@"..\..\Arved\Arve_.pdf"));
                smtpClient.Send(mail);
                MessageBox.Show("Arve oli saadetud mailile: " + adress);
            }
            catch (Exception)
            {
                MessageBox.Show("Viga");
            }
        }
    }
}
