using System.ComponentModel.DataAnnotations;

namespace Application.ValidationAttributes
{
    public class NotZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is int i)
            {
                return i != 0;
            }
            return true;
        }
    }
}
