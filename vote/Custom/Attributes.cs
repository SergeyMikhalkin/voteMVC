using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace vote.Attributes
{
    public class CustomValidateDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt;
            bool parseSuccess =  DateTime.TryParseExact(value.ToString(), "dd.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);


            if (parseSuccess)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Дата введена неверно!");
            }
        }
    }
}