using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Labs.UI.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "img-action, img-controller")]
    public class ImageTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ImageTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// Действие контроллера
        /// </summary>
        [HtmlAttributeName("img-action")]
        public string ImgAction { get; set; } = string.Empty;

        /// <summary>
        /// Контроллер
        /// </summary>
        [HtmlAttributeName("img-controller")]
        public string ImgController { get; set; } = string.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            var url = urlHelper.Action(ImgAction, ImgController);

            output.Attributes.SetAttribute("src", url);
        }
    }
}
