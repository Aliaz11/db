namespace WinFormsApp3
{
    public delegate bool RequiredValidDel(string fieldVal);
    public delegate bool StringLengthValidDel(string fieldVal, int min, int max);
    public delegate bool DateValidDel(string fieldVal, out DateTime validDateTime);
    public delegate bool PatternMatchValidDel(string fieldVal, string pattern);
    public delegate bool CompareFieldsValidDel(string fieldVal, string fieldValCompare);
    public class CommonFieldValidatorFunctions
    {
        private static RequiredValidDel? _requiredValidDel;
        private static StringLengthValidDel? _stringLengthValidDel;
        private static DateValidDel? _dateValidDel;
        private static PatternMatchValidDel? _patternMatchValidDel;
        private static CompareFieldsValidDel? _compareFieldsValidDel;

        public static RequiredValidDel RequiredFieldValidDel
        {
            get
            {
                return _requiredValidDel ??= new RequiredValidDel(RequiredFieldValid);
            }
        }

        public static StringLengthValidDel StringLengthFieldValidDel
        {
            get
            {
                return _stringLengthValidDel ??= new StringLengthValidDel(StringFieldLengthValid);
            }
        }
        public static DateValidDel DateFieldValidDel
        {
            get
            {
                return _dateValidDel ??= new DateValidDel(DateFieldValid);
            }
        }
        public static PatternMatchValidDel PatternMatchValidDel
        {
            get
            {
                return _patternMatchValidDel ??= new PatternMatchValidDel(FieldPatternValid);
            }
        }

        public static CompareFieldsValidDel FieldsCompareValidDel
        {
            get
            {
                return _compareFieldsValidDel ??= new CompareFieldsValidDel(FieldComparisonValid);
            }
        }


        public static bool RequiredFieldValid(string fieldVal)
        {
            return !string.IsNullOrEmpty(fieldVal);
        }

        public static bool StringFieldLengthValid(string fieldVal, int min, int max)
        {
            return fieldVal is not null && fieldVal.Length >= min && fieldVal.Length <= max;
        }

        public static bool DateFieldValid(string dateTime, out DateTime validDateTime)
        {
            return DateTime.TryParse(dateTime, out validDateTime);
        }

        public static bool FieldPatternValid(string fieldVal, string regularExpressionPattern)
        {
            if (fieldVal is null || string.IsNullOrEmpty(regularExpressionPattern))
                return false;

            // Fully qualified: db.Regex (the pattern holder) shadows this type inside namespace db.
            return System.Text.RegularExpressions.Regex.IsMatch(fieldVal, regularExpressionPattern);
        }

        public static bool FieldComparisonValid(string field1, string field2)
        {
            return string.Equals(field1, field2, StringComparison.Ordinal);
        }
    }
}
