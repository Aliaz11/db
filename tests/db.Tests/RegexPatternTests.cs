using Xunit;
using SysRegex = System.Text.RegularExpressions.Regex;

namespace db.Tests
{
    public class RegexPatternTests
    {
        private static bool IsStrongPassword(string value) =>
            SysRegex.IsMatch(value, db.Regex.Strong_Password_RegEx_Pattern);

        private static bool IsEmail(string value) =>
            SysRegex.IsMatch(value, db.Regex.Email_Address_RegEx_Pattern);

        [Theory]
        [InlineData("Passw0rd!")]
        [InlineData("Abcdef1!")]
        [InlineData("Tr0ub4dor&3")]
        public void StrongPassword_AcceptsAValidPassword(string password)
        {
            Assert.True(IsStrongPassword(password));
        }

        // Regression guard. The original pattern was pasted in HTML-escaped, so its "special
        // character" class contained the letters of &amp; &quot; &gt; &lt; -- which meant plain
        // words like these satisfied the special-character rule and the requirement was never
        // actually enforced.
        [Theory]
        [InlineData("Passw0rda")]
        [InlineData("Passw0rdX")]
        [InlineData("Passw0rdt")]
        public void StrongPassword_RejectsWhatTheHtmlEscapedPatternWronglyAccepted(string password)
        {
            Assert.False(IsStrongPassword(password));
        }

        [Theory]
        [InlineData("Abc1!")]      // too short
        [InlineData("abcdef1!")]   // no uppercase
        [InlineData("ABCDEF1!")]   // no lowercase
        [InlineData("Abcdefg!")]   // no digit
        [InlineData("Abcdef12")]   // no special character
        [InlineData("Ab1! def2")]  // contains whitespace
        [InlineData("")]
        public void StrongPassword_RejectsAnInvalidPassword(string password)
        {
            Assert.False(IsStrongPassword(password));
        }

        [Fact]
        public void StrongPassword_RejectsAnOverlongPassword()
        {
            Assert.False(IsStrongPassword("Ab1!" + new string('c', 61)));
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("user@sub.example.museum")] // TLD longer than the old {2,4} bound
        [InlineData("user+tag@example.travel")] // + is legal in a local part
        public void Email_AcceptsAValidAddress(string address)
        {
            Assert.True(IsEmail(address));
        }

        [Theory]
        [InlineData("no-at-sign.com")]
        [InlineData("bad@@x.com")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        public void Email_RejectsAnInvalidAddress(string address)
        {
            Assert.False(IsEmail(address));
        }
    }
}
