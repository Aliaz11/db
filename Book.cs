using System.Globalization;

namespace db
{
    /// <summary>
    /// One row of the <c>Books</c> table.
    /// The lowercase member names mirror the database columns and are relied on by several forms,
    /// so they are kept as-is. See <see cref="PriceValue"/> for numeric use of <see cref="price"/>.
    /// </summary>
    public class Book
    {
        /// <summary>Book title (<c>Books.name</c>).</summary>
        public string Name { get; set; } = "";

        /// <summary>Author's full name (<c>Books.author</c>).</summary>
        public string author { get; set; } = "";

        /// <summary>
        /// Unit price, stored as text because the <c>Books.price</c> column is textual.
        /// It is not a formatted currency string — use <see cref="PriceValue"/> for any arithmetic.
        /// </summary>
        public string price { get; set; } = "";

        /// <summary>Publication or acquisition date as displayed text (<c>Books.Date</c>).</summary>
        public string Date { get; set; } = "";

        /// <summary>Cover image as raw image bytes (<c>Books.image</c>). Empty when there is no cover.</summary>
        public byte[] image { get; set; } = Array.Empty<byte>();

        /// <summary>Number of copies (<c>Books.quantity</c>). Whole units, despite the decimal type.</summary>
        public decimal quantity { get; set; }

        /// <summary>
        /// <see cref="price"/> parsed as a number. Returns 0 when the text is empty or not numeric,
        /// so summing a list of books can never throw. Invariant culture is tried first, then the
        /// current culture, so both "12.50" and "12,50" are accepted.
        /// </summary>
        public decimal PriceValue
        {
            get
            {
                string raw = (price ?? "").Trim();
                if (raw.Length == 0)
                    return 0m;

                const NumberStyles styles = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;

                if (decimal.TryParse(raw, styles, CultureInfo.InvariantCulture, out decimal invariant))
                    return invariant;

                if (decimal.TryParse(raw, styles, CultureInfo.CurrentCulture, out decimal current))
                    return current;

                return 0m;
            }
        }

        /// <summary>Total value of the stock held for this title.</summary>
        public decimal TotalValue => PriceValue * quantity;
    }
}
