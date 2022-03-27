using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CLDemo.ValidationRules
{
    internal class SensitiveWordsValidRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var isValid = true;
            var errorText = "请文明上网。";

            if (value is string)
            {
                var str = value as string;
                if (str.Contains("SB"))
                {
                    isValid = false;
                }
            }
            else
            {
                isValid = false;
            }
            return new ValidationResult(isValid, errorText);
        }
    }
}
