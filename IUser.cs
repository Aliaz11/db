namespace db
{
    /// <summary>
    /// One row of the <c>Stu1</c> table, used as the form-to-database transfer object.
    /// Despite the <c>I</c> prefix this is a plain class, not an interface; the name is kept
    /// because many call sites depend on it.
    /// </summary>
    public class IUser
    {
        /// <summary>Given name (<c>Stu1.firstname</c>), 2–11 characters.</summary>
        public string FirstName { get; set; } = "";

        /// <summary>Family name (<c>Stu1.lastname</c>), 2–11 characters.</summary>
        public string LastName { get; set; } = "";

        /// <summary>Digits only, 10–11 of them (<c>Stu1.phonenumber</c>). Empty when not provided.</summary>
        public string PhoneNumber { get; set; } = "";

        /// <summary>Date of birth (<c>Stu1.Birthdate</c>). Time component is unused.</summary>
        public DateTime BirthDate { get; set; }

        /// <summary>Email address (<c>Stu1.Email</c>). Empty when not provided; blanks never collide.</summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// The password as typed by the user. Always plaintext in memory — it is hashed on the way
        /// into the database and is never persisted from this property directly.
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>The password confirmation field; must equal <see cref="Password"/>. Never stored.</summary>
        public string PasswordR { get; set; } = "";

        /// <summary>Login name (<c>Stu1.username</c>); must be unique.</summary>
        public string UserName { get; set; } = "";

        /// <summary>"male", "female" or empty when nothing was chosen (<c>Stu1.Gender</c>).</summary>
        public string Gender { get; set; } = "";

        /// <summary>Profile picture as raw image bytes (<c>Stu1.image</c>). Empty when none was chosen.</summary>
        public byte[] Photo { get; set; } = Array.Empty<byte>();
    }
}
