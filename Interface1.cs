using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Forms;

namespace WinFormsApp3
{
    public interface Interface1
    {
        void insert();
        void refresh();
        string firstname { get; set; }
        string lastname { get; set; }
        string phonenumber { get; set; }
        string birthdate { get; set; }
        string email { get; set; }
        string password { get; set; }

    }
}
