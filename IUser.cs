namespace db
{
    public class IUser
    {
        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime BirthDate { get; set; } 
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string PasswordR { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Gender { get; set; } = "";
        public byte[] Photo { get; set; }   =Array.Empty<byte>();

    }
}
