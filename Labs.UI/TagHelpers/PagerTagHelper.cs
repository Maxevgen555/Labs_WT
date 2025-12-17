using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Labs.UI.TagHelpers
{
    [HtmlTargetElement("pager", Attributes = "current-page, total-pages")]
    public class PagerTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PagerTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        [HtmlAttributeName("current-page")]
        public int CurrentPage { get; set; }

        [HtmlAttributeName("total-pages")]
        public int TotalPages { get; set; }

        public string? Category { get; set; }

        public bool Admin { get; set; } = false;

        public string Controller { get; set; } = "Product";

        public string Action { get; set; } = "Index";

        private int PrevPage => CurrentPage == 1 ? 1 : CurrentPage - 1;

        private int NextPage => CurrentPage == TotalPages ? TotalPages : CurrentPage + 1;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (TotalPages <= 1)
                return;

            output.TagName = "nav";
            output.Attributes.Add("aria-label", "Page navigation");
            output.AddClass("mt-4", HtmlEncoder.Default);

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            ul.AddCssClass("justify-content-center");

            // Кнопка "Предыдущая"
            ul.InnerHtml.AppendHtml(CreatePageItem(Category, PrevPage, "&laquo;",
                CurrentPage == 1 ? "disabled" : ""));

            // Нумерация страниц
            for (int i = 1; i <= TotalPages; i++)
            {
                var active = i == CurrentPage ? "active" : "";
                ul.InnerHtml.AppendHtml(CreatePageItem(Category, i, i.ToString(), active));
            }

            // Кнопка "Следующая"
            ul.InnerHtml.AppendHtml(CreatePageItem(Category, NextPage, "&raquo;",
                CurrentPage == TotalPages ? "disabled" : ""));

            output.Content.AppendHtml(ul);
        }

        private TagBuilder CreatePageItem(string? category, int pageNo, string innerText, string additionalClass = "")
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            if (!string.IsNullOrEmpty(additionalClass))
                li.AddCssClass(additionalClass);

            if (pageNo == CurrentPage && string.IsNullOrEmpty(additionalClass))
                li.AddCssClass("active");

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");

            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            if (Admin)
            {
                // Для административной панели используем другой маршрут
                var routeValues = new { area = "Admin", pageNo = pageNo };
                var url = urlHelper.Page("/Dishes/Index", routeValues);
                a.Attributes.Add("href", url);
            }
            else
            {
                var routeValues = new { category = category, pageNo = pageNo };
                var url = urlHelper.Action(Action, Controller, routeValues);
                a.Attributes.Add("href", url);
            }

            a.InnerHtml.AppendHtml(innerText);
            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}