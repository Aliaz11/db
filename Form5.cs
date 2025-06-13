using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using WinFormsApp3;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace db
{
  
    public partial class Form5 : Form
    {

        string ids;
        string connection1=Locator.GetConnectionString();   

        public Form5()
        {
            InitializeComponent();
            BackPhoto.BackSet(this);
        }
        public Form5(string ids)
        {
            InitializeComponent();
            BackPhoto.BackSet(this);
            this.ids = ids;
        }


        private void Form5_Load(object sender, EventArgs e)
        {

            DataBaseCrud db1 = new DataBaseCrud(connection1);
            db1.selector(dataGridView1, this);

        }

  

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            this.Close();
            form4.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DbCrudBook tester = new DbCrudBook(ids);
            tester.inserter(dataGridView1);


            try
            {


            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            Form9 form9 = new Form9();
            form9.ShowDialog();
        }
    }
}

