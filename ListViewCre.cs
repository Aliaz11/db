namespace db
{
    public class ListViewCre: IListViewCre
    {
        /// <summary>
        /// Configures the user list view. Columns are rebuilt rather than appended, and the control
        /// is only parented when it is not already on the form, so calling this twice (Form2 and
        /// Form3 both call it on load) no longer duplicates the nine columns.
        /// </summary>
        public void ListViewCre1(ListView listView1, Form form)
        {
            if (listView1 == null || form == null)
                return;

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;

            listView1.Columns.Clear();
            listView1.Columns.Add("ID", 100);
            listView1.Columns.Add("First Name", 100);
            listView1.Columns.Add("Last Name", 100);
            listView1.Columns.Add("Phone Number", 120);
            listView1.Columns.Add("Birth Date", 100);
            listView1.Columns.Add("Email", 100);
            listView1.Columns.Add("Gender", 80);
            listView1.Columns.Add("password", 120);
            listView1.Columns.Add("UserName", 100);

            if (listView1.Parent == null)
                form.Controls.Add(listView1);
        }


    }
}
