using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SXF.Utils.MVC.CustomValidation
{
    public class UrlAttribute : ValidationAttribute, IClientValidatable
    {
        public override string FormatErrorMessage(string name)
        {
            return string.Format("{0}格式有误", name);
        }

        public UrlAttribute()
        {
        }

        //服务器端验证
        public override bool IsValid(object value)
        {
            var text = value as string;
            Uri uri;

            return (!string.IsNullOrWhiteSpace(text) && Uri.TryCreate(text, UriKind.Absolute, out uri));
        }

        //客户端验证
        //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        //{
        //    var validationRule = new ModelClientValidationRule
        //    {
        //        ErrorMessage = FormatErrorMessage(metadata.DisplayName),
        //        ValidationType = "url",
        //    };

        //    yield return validationRule;
        //}
    }
}
