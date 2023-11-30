using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tooted
{
    public partial class Form1 : Form
    {
        int Id = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            open = new OpenFileDialog();
            open.InitialDirectory = @"C:\Users\egor.fedorenko\Pictures";
            open.Multiselect = true;
            open.Filter = "Images Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.png; *.jpg";

            FileInfo open_info = new FileInfo(@"C:\Users\egor.fedorenko\Pictures\"+open_info.FileName);
            if (open_info.ShowDialog() == DialogResult.OK && Toode_txt.Text!=null)
            {
                save = new SaveFileDialog();
                save.InitialDirectory = Path.GetFullPath(@"..\..\Images");
                save.FileNameToode_txt.Text + Path.GetExtension(open.FileName);
                save.Filter = "Images" + Path.GetExtension(open.FileName) + "|" + Path.GetExtension(open.FileName);
                if (save.ShowDialog() == DialogResult.OK && Toode_txt.Text != null)
                {
                    File.Copy(open.FileName, save.FileName);
                    Toode_pb.Image = Image.FromFile(save.FileName);
                }
            }
            else
            {
                MessageBox.Show("Puudub toode");
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Id = (int)dataGridView1.Rows[e.RowIndex].Cells["id"].Value;
            Toode_txt.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value;
            Kogus_txt.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value;
            Hind_txt.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value;
            try
            {
                Toode_pb.Image = Image.FromFile(@"..\..\Images"+dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString());
            }
            catch (Exception)
            {
                MessageBox.Show("Pilt puudub");
            }
            Kat_Box.SelectedIndex = (int)dataGridView1.Rows[e.RowIndex].Cells[5].Value;
        }

    }
}
