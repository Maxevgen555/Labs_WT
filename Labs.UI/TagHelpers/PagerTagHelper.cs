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

        /// <summary>
        /// Номер текущей страницы
        /// </summary>
        [HtmlAttributeName("current-page")]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Общее количество страниц
        /// </summary>
        [HtmlAttributeName("total-pages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Имя категории объектов
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Признак страниц администратора
        /// </summary>
        public bool Admin { get; set; } = false;

        /// <summary>
        /// Имя контроллера
        /// </summary>
        public string Controller { get; set; } = "Product";

        /// <summary>
        /// Имя действия
        /// </summary>
        public string Action { get; set; } = "Index";

        /// <summary>
        /// Номер предыдущей страницы
        /// </summary>
        private int PrevPage => CurrentPage == 1 ? 1 : CurrentPage - 1;

        /// <summary>
        /// Номер следующей страницы
        /// </summary>
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
            var routeValues = new { category = category, pageNo = pageNo };
            var url = urlHelper.Action(Action, Controller, routeValues);

            a.Attributes.Add("href", url);
            a.InnerHtml.AppendHtml(innerText);

            li.InnerHtml.AppendHtml(a);
            return li;
        }
    }
}