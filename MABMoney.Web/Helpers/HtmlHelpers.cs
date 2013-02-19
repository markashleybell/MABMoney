using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MABMoney.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString TextControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return TextControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString PasswordControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return PasswordControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString EditorControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return EditorControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString SelectControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return SelectControlGroupFor<TModel, TProperty>(htmlHelper, expression, selectList, optionLabel, null);
        }

        public static MvcHtmlString TextControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();
            
            output.Append("<div class=\"control-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "control-label" }));
            output.Append("<div class=\"controls\">");
            output.Append(System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes));
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString PasswordControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"control-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "control-label" }));
            output.Append("<div class=\"controls\">");
            output.Append(System.Web.Mvc.Html.InputExtensions.PasswordFor(htmlHelper, expression, htmlAttributes));
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString EditorControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"control-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "control-label" }));
            output.Append("<div class=\"controls\">");
            output.Append(System.Web.Mvc.Html.EditorExtensions.EditorFor(htmlHelper, expression, htmlAttributes));
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString SelectControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"control-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "control-label" }));
            output.Append("<div class=\"controls\">");
            output.Append(System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, optionLabel, htmlAttributes));
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString CheckboxControlGroupFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            var output = new StringBuilder();

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            output.Append("<div class=\"control-group\">");
            output.Append("<div class=\"controls\">");
            output.Append("<label class=\"checkbox\" for=\"" + ExpressionHelper.GetExpressionText(expression) + "\">");
            output.Append(System.Web.Mvc.Html.InputExtensions.CheckBoxFor(htmlHelper, expression));
            output.Append(metadata.DisplayName);
            output.Append("</label>");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString SubmitControlGroup<TModel>(this HtmlHelper<TModel> htmlHelper, string buttonText)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"control-group\">");
            output.Append("<div class=\"controls\">");
            output.Append("<input type=\"submit\" value=\"" + buttonText + "\" class=\"btn btn-primary\" />");
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString ButtonControlGroup<TModel>(this HtmlHelper<TModel> htmlHelper, string buttonText)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"control-group\">");
            output.Append("<div class=\"controls\">");
            output.Append("<input type=\"submit\" value=\"" + buttonText + "\" class=\"btn\" />");
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }
    }
}