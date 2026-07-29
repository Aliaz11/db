namespace db
{
    public interface IDbCrudBook
    {
        void insert(Book book);
        void inserter(DataGridView dataGridView1);
        void delete(DataGridView dataGridView1, int index);
    }
}
