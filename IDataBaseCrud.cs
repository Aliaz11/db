using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public interface IDataBaseCrud
    {
        byte[] getphoto(PictureBox picturebox);
        void update(DataGridView dataGridView1, int index, TextBox textBox1, TextBox textBox2, TextBox textBox3, NumericUpDown numeric, DateTimePicker datatime);
        void updateBase(DataGridView dataGridView1, int index, TextBox textBox1, TextBox textBox2, TextBox textBox3, NumericUpDown numeric, DateTimePicker datatime);
        void delete(DataGridView dataGridView1, int index);
        void selector(DataGridView dataGridView1, Form form);
        void selector(ListView listView1);
        void update(ListView listView1, string firstname, string lastname, string phonenumber1, string gender, string Birthdate, string password, string email, string username);
        void delete(ListView listView1);
        void insert(string firstname, string lastname, string phonenumber, string birthdate, string email, string gender, string password, string username, byte[] photoer);
    }
}
