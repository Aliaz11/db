namespace db
{
    public class Book
    {
        public string Name { get; set; } = "";
        public string author { get; set; } = "";
        public string price { get; set; } = "";
        public string Date { get; set; } = "";
        public byte[] image {  get; set; }  =Array.Empty<byte>();   
        public decimal quantity {  get; set; }




    }
}
