using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public interface Idels
    {
        bool RequiredFieldValid(string fieldVal);
        bool StringFieldLengthValid(string fieldVal, int min, int max);
        bool DateFieldValid(string dateTime, out DateTime validDateTime);
        bool FieldPatternValid(string fieldVal, string regularExpressionPattern);
        bool FieldComparisonValid(string field1, string field2);
    }
}
