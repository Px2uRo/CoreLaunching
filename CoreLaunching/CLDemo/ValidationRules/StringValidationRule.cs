using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using CLDemo.Data;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;

namespace CLDemo.ValidationRules
{
    internal class StringValidationRule : ValidationRule
    {
        // 返回 ValidationResult 结果，表示传入的值是否合法。
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var isValid = true;
            var errorText = "这代码不合法啊，但是错误有点复杂，我没工夫找。";

            if (value is string)
            {
                if ((value as string).Length == 0)
                {
                    isValid = false;
                    errorText = "请在此输入内容。";
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
