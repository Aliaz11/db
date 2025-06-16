using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public interface IDbCrudBook
    {
        void insert(Book book);
        void inserter(DataGridView dataGridView1);
        void delete(DataGridView dataGridView1, int index);
    }
}
