using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public static class Locator
    {
        public static string GetConnectionString()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            return $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}Stu2.mdf;Integrated Security=True;";
        }
    }
}
