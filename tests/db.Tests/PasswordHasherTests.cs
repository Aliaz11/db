using db.Security;
using Xunit;

namespace db.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void Hash_ProducesTheDocumentedFormat()
        {
            string hash = PasswordHasher.Hash("Correct-Horse-1!");

            string[] parts = hash.Split('$');
            Assert.Equal(4, parts.Length);
            Assert.Equal("PBKDF2", parts[0]);
            Assert.Equal("100000", parts[1]);
        }

        [Fact]
        public void Hash_DoesNotLeakThePlaintext()
        {
            const string password = "Correct-Horse-1!";

            Assert.DoesNotContain(password, PasswordHasher.Hash(password), StringComparison.Ordinal);
        }

        [Fact]
        public void Hash_UsesARandomSaltPerCall()
        {
            Assert.NotEqual(PasswordHasher.Hash("same"), PasswordHasher.Hash("same"));
        }

        [Fact]
        public void Verify_AcceptsTheCorrectPassword()
        {
            string hash = PasswordHasher.Hash("Correct-Horse-1!");

            Assert.True(PasswordHasher.Verify("Correct-Horse-1!", hash));
        }

        [Theory]
        [InlineData("Correct-Horse-1")]
        [InlineData("correct-horse-1!")]
        [InlineData("")]
        public void Verify_RejectsAWrongPassword(string attempt)
        {
            string hash = PasswordHasher.Hash("Correct-Horse-1!");

            Assert.False(PasswordHasher.Verify(attempt, hash));
        }

        [Fact]
        public void Verify_StillAcceptsALegacyPlaintextRow()
        {
            // Rows written before hashing existed must keep working until the next successful login.
            Assert.True(PasswordHasher.Verify("oldpass", "oldpass"));
            Assert.False(PasswordHasher.Verify("nope", "oldpass"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Verify_RejectsAMissingStoredValue(string? stored)
        {
            Assert.False(PasswordHasher.Verify("anything", stored));
        }

        [Fact]
        public void NeedsUpgrade_IsTrueOnlyForLegacyPlaintext()
        {
            Assert.True(PasswordHasher.NeedsUpgrade("oldpass"));
            Assert.False(PasswordHasher.NeedsUpgrade(PasswordHasher.Hash("x")));
            Assert.False(PasswordHasher.NeedsUpgrade(null));
            Assert.False(PasswordHasher.NeedsUpgrade(""));
        }

        [Theory]
        [InlineData("PBKDF2$abc$x$y")]        // non-numeric iteration count
        [InlineData("PBKDF2$100000$!!!$!!!")] // not base64
        [InlineData("PBKDF2$1$$")]            // empty salt and hash
        [InlineData("$$$")]
        [InlineData("PBKDF2$100000$AAAA")]    // too few segments
        [InlineData("PBKDF2$-1$AAAA$AAAA")]   // non-positive iteration count
        public void Verify_DoesNotThrowOnAMalformedStoredValue(string stored)
        {
            // A corrupt row must fail the login, never crash the app.
            Assert.False(PasswordHasher.Verify("anything", stored));
        }
    }
}
