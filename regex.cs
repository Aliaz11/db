namespace db
{
    /// <summary>
    /// Validation patterns used by the field validators.
    /// NOTE: this type is deliberately named <c>Regex</c> and therefore shadows
    /// <see cref="System.Text.RegularExpressions.Regex"/> inside the <c>db</c> namespace.
    /// Code in <c>db</c> that needs the real engine must spell out
    /// <c>System.Text.RegularExpressions.Regex</c>.
    /// </summary>
    public class Regex
    {
        /// <summary>
        /// Local part, then one or more dot-separated labels, then a 2..63 character alphabetic TLD.
        /// The previous {2,4} upper bound rejected valid modern TLDs such as .museum or .travel.
        /// </summary>
        public const string Email_Address_RegEx_Pattern = @"^[\w\-\.\+]+@([\w\-]+\.)+[A-Za-z]{2,63}$";

        public const string Uk_PhoneNumber_RegEx_Pattern = @"^\(?(?:(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?\(?(?:0\)?[\s-]?\(?)?|0)(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}|\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4}|\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3})|\d{5}\)?[\s-]?\d{4,5}|8(?:00[\s-]?11[\s-]?11|45[\s-]?46[\s-]?4\d))(?:(?:[\s-]?(?:x|ext\.?\s?|\#)\d+)?)$";

        public const string Uk_Post_Code_RegEx_Pattern = @"^(([Gg][Ii][Rr] ?0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2}))$";

        /// <summary>
        /// 8 to 64 characters, at least one lowercase letter, one uppercase letter, one digit and one
        /// special character, and no whitespace anywhere.
        /// The previous pattern had HTML entities (&amp;amp; &amp;quot; &amp;gt; &amp;lt;) pasted into the
        /// character class, which made plain letters such as 'a', 'm', 'p', 'q', 'u', 'o', 't', 'g' and 'l'
        /// count as "special characters".
        /// </summary>
        public const string Strong_Password_RegEx_Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9\s])(?!.*\s).{8,64}$";
    }
}
