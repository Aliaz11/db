using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using WinFormsApp3;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace db
{

    public partial class Form6 : Form
    {
        int index;
        string id;

        string connection = Locator.GetConnectionString();
        public Form6(List<Book> books)
        {
            InitializeComponent();
            BackPhoto.BackSet(this);


        }


        private void Form6_Load(object sender, EventArgs e)
        {
          

            DataBaseCrud db = new DataBaseCrud(connection);
            db.selector(dataGridView1,this);
     


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Form3 form1 = new Form3();
            form1.Location = this.Location;
            form1.Size = this.Size;
            form1.StartPosition = FormStartPosition.Manual;
            form1.Show();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {


         /*   if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please select a book to delete by clicking on its row in the table.", "No Book Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }*/

            DialogResult confirmResult = MessageBox.Show(
                "Are you sure you want to delete this book? This action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
              
                DataBaseCrud db= new DataBaseCrud(connection);   
                db.delete(dataGridView1,index,textBox1,textBox2,textBox3);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                index = e.RowIndex;

            
                DataBaseCrud db = new DataBaseCrud(connection);
                
            }
            
            
            
            else
            {

                id = string.Empty;
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
        }





        private void button2_Click_1(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Please select a book to edit by clicking on its row in the table.", "No Book Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            string name = textBox1.Text;
            string author = textBox2.Text;
            string price = textBox3.Text;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Please fill in all book details (Name, Author, Price).", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

         

           
        }

  
    }
}
    

