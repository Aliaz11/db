using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    public interface ILabelValidator
    {
          void selectoring(string email, string username, TextBox text_email, TextBox text_user);
        IEnumerable<ValidationError> Validate(IUser user);
         void RemoveValidationLabel(string name, Form form);
         void ShowValidationLabel(string name, string message, Point pos, Form form);
    }
}
