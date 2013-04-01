using System;
using System.Linq;
using Nancy.ViewEngines.Razor;

namespace JabbR
{
    public static class InputExtensions
    {
        public static IHtmlString TextBox<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName)
        {
            return TextBox(htmlHelper, propertyName, String.Empty);
        }

        public static IHtmlString TextBox<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName, string className)
        {
            return TextBox(htmlHelper, propertyName, className, null);
        }

        public static IHtmlString TextBox<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName, string className, string placeholder)
        {
            return TextBox(htmlHelper, propertyName, className, placeholder, null);
        }

        public static IHtmlString TextBox<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName, string className, string placeholder, int? maxLength)
        {
            return InputHelper(htmlHelper, "text", propertyName, htmlHelper.GetValueForProperty(propertyName), className, placeholder, maxLength);
        }

        public static IHtmlString Password<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName)
        {
            return Password(htmlHelper, propertyName, String.Empty);
        }

        public static IHtmlString Password<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName, string className)
        {
            return Password(htmlHelper, propertyName, className, null);
        }

        public static IHtmlString Password<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName, string className, string placeholder)
        {
            return Password(htmlHelper, propertyName, className, placeholder, null);
        }

        public static IHtmlString Password<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName, string className, string placeholder, int? maxLength)
        {
            return InputHelper(htmlHelper, "password", propertyName, null, className, placeholder, maxLength);
        }

        private const string InputTemplate = @"<input type=""{0}"" id=""{1}"" name=""{2}"" value=""{3}"" class=""{4}"" {5} />";
        private static IHtmlString InputHelper<TModel>(HtmlHelpers<TModel> htmlHelper, string inputType, string propertyName, string value, string className, string placeholder, int? maxLength)
        {
            bool hasError = htmlHelper.GetErrorsForProperty(propertyName).Any();

            string additionalProperties = string.Empty;

            if (!string.IsNullOrEmpty(placeholder))
            {
                additionalProperties += string.Format(@"placeholder=""{0}"" ", placeholder);
            }
            
            if (maxLength != null)
            {
                additionalProperties += string.Format(@"maxlength=""{0}"" ", maxLength);
            }

            return new NonEncodedHtmlString(String.Format(InputTemplate, inputType, propertyName, propertyName, value, hasError ? String.Format("{0} {1}", className, "error").Trim() : className, additionalProperties));
        }

        internal static string GetValueForProperty<TModel>(this HtmlHelpers<TModel> htmlHelper, string propertyName)
        {
            var propInfo =
                typeof (TModel).GetProperties()
                               .FirstOrDefault(
                                   x => x.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));

            string value = null;

            if (propInfo != null && htmlHelper.Model != null)
            {
                value = propInfo.GetValue(htmlHelper.Model) as string;
            }

            if (String.IsNullOrWhiteSpace(value))
            {
                value = htmlHelper.RenderContext.Context.Request.Form[propertyName];
            }

            if (String.IsNullOrWhiteSpace(value))
            {
                value = htmlHelper.RenderContext.Context.Request.Query[propertyName];
            }

            return value;
        }
    }
}