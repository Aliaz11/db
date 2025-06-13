using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public class ListViewCre
    {
       public void ListViewCre1(ListView listView1,Form form)
        {



            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("ID", 100);
            listView1.Columns.Add("First Name", 100);
            listView1.Columns.Add("Last Name", 100);
            listView1.Columns.Add("Phone Number", 120);
            listView1.Columns.Add("Birth Date", 100);
            listView1.Columns.Add("Email", 100);
            listView1.Columns.Add("Gender", 80);
            listView1.Columns.Add("password", 120);
            listView1.Columns.Add("UserName", 100);
            form.Controls.Add(listView1);
        }

 
    }
}
