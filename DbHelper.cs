using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public static class DbHelper
    {
        public static string GetConnectionString()
        {
            string dbPath = System.AppDomain.CurrentDomain.BaseDirectory;
            return $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath}Stu2.mdf;Integrated Security=True;";
        }
    }
}
