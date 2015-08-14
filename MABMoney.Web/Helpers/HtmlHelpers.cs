using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MABMoney.Web.Extensions;

namespace MABMoney.Web.Helpers
{
    public static class HtmlHelpers
    {
        private static object _baseClasses = new { @class = "form-control" };

        public static MvcHtmlString TextControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return TextControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString TextAreaControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return TextAreaControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString PasswordControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return PasswordControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString FileControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return FileControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString EditorControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return EditorControlGroupFor<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString SelectControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            return SelectControlGroupFor<TModel, TProperty>(htmlHelper, expression, selectList, optionLabel, null);
        }

        public static MvcHtmlString SubmitControlGroup<TModel>(this HtmlHelper<TModel> htmlHelper, string buttonText)
        {
            return SubmitControlGroup(htmlHelper, buttonText, null);
        }

        public static MvcHtmlString ButtonControlGroup<TModel>(this HtmlHelper<TModel> htmlHelper, string buttonText)
        {
            return ButtonControlGroup(htmlHelper, buttonText, null);
        }

        public static MvcHtmlString TextControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();
            
            output.Append("<div class=\"form-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "col-md-2 control-label" }));
            output.Append("<div class=\"col-md-4\">");
            output.Append(System.Web.Mvc.Html.InputExtensions.TextBoxFor(htmlHelper, expression, _baseClasses.CombineWith(htmlAttributes)));
            output.Append("</div>");
            output.Append("<div class=\"col-md-6\">");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString TextAreaControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"form-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "col-md-2 control-label" }));
            output.Append("<div class=\"col-md-4\">");
            output.Append(System.Web.Mvc.Html.TextAreaExtensions.TextAreaFor(htmlHelper, expression, _baseClasses.CombineWith(htmlAttributes)));
            output.Append("</div>");
            output.Append("<div class=\"col-md-6\">");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString PasswordControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"form-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "col-md-2 control-label" }));
            output.Append("<div class=\"col-md-4\">");
            output.Append(System.Web.Mvc.Html.InputExtensions.PasswordFor(htmlHelper, expression, _baseClasses.CombineWith(htmlAttributes)));
            output.Append("</div>");
            output.Append("<div class=\"col-md-6\">");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString FileControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"form-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "col-md-2 control-label" }));
            output.Append("<div class=\"col-md-4\">");
            output.Append("<input type=\"file\" name=\"" + ExpressionHelper.GetExpressionText(expression) + "\" id=\"" + ExpressionHelper.GetExpressionText(expression) + "\" value=\"\" />");
            output.Append("</div>");
            output.Append("<div class=\"col-md-6\">");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString EditorControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"form-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "col-md-2 control-label" }));
            output.Append("<div class=\"col-md-4\">");
            output.Append(System.Web.Mvc.Html.EditorExtensions.EditorFor(htmlHelper, expression, _baseClasses.CombineWith(htmlAttributes)));
            output.Append("</div>");
            output.Append("<div class=\"col-md-6\">");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString SelectControlGroupFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"form-group\">");
            output.Append(System.Web.Mvc.Html.LabelExtensions.LabelFor(htmlHelper, expression, new { @class = "col-md-2 control-label" }));
            output.Append("<div class=\"col-md-4\">");
            output.Append(System.Web.Mvc.Html.SelectExtensions.DropDownListFor(htmlHelper, expression, selectList, optionLabel, _baseClasses.CombineWith(htmlAttributes)));
            output.Append("</div>");
            output.Append("<div class=\"col-md-6\">");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString CheckboxControlGroupFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            return CheckboxControlGroupFor<TModel>(htmlHelper, expression, null);
        }

        public static MvcHtmlString CheckboxControlGroupFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            var output = new StringBuilder();

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            output.Append("<div class=\"form-group\">");
            output.Append("<div class=\"col-md-4 col-md-offset-2\">");
            output.Append("<div class=\"checkbox\">");
            output.Append("<label for=\"" + ExpressionHelper.GetExpressionText(expression) + "\">");
            output.Append(System.Web.Mvc.Html.InputExtensions.CheckBoxFor(htmlHelper, expression, htmlAttributes));
            output.Append(metadata.DisplayName);
            output.Append("</label>");
            output.Append("</div>");
            output.Append("</div>");
            output.Append("<div class=\"col-md-6\">");
            output.Append(System.Web.Mvc.Html.ValidationExtensions.ValidationMessageFor(htmlHelper, expression));
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString SubmitControlGroup<TModel>(this HtmlHelper<TModel> htmlHelper, string buttonText, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"form-group\">");
            output.Append("<div class=\"col-md-2 col-md-offset-2\">");

            var tag = new TagBuilder("input");
            tag.Attributes.Add("type", "submit");

            var defaultAttributes = new { @class = "form-control btn btn-block" };

            var attributes = defaultAttributes.CombineWith(htmlAttributes);

            if (htmlAttributes == null)
                attributes["class"] = attributes["class"].ToString() + " btn-primary";

            foreach (KeyValuePair<string, object> a in attributes)
                tag.Attributes.Add(a.Key, a.Value.ToString());

            tag.Attributes.Add("value", buttonText);

            output.Append(tag.ToString());

            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }

        public static MvcHtmlString ButtonControlGroup<TModel>(this HtmlHelper<TModel> htmlHelper, string buttonText, object htmlAttributes)
        {
            var output = new StringBuilder();

            output.Append("<div class=\"form-group\">");
            output.Append("<div class=\"col-md-2 col-md-offset-2\">");

            var tag = new TagBuilder("button");
            tag.Attributes.Add("type", "button");

            var defaultAttributes = new { @class = "form-control btn btn-default btn-block" };

            var attributes = defaultAttributes.CombineWith(htmlAttributes);

            foreach (KeyValuePair<string, object> a in attributes)
                tag.Attributes.Add(a.Key, a.Value.ToString());

            tag.InnerHtml = buttonText;

            output.Append(tag.ToString());
            output.Append("</div>");
            output.Append("</div>");

            return new MvcHtmlString(output.ToString());
        }
    }
}