using FubuCore.Descriptions;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.WebForms
{
    public class WebFormsViewFactory<TView> : IViewFactory where TView : IFubuPage
    {
        private readonly IWebFormsControlBuilder _builder;
        private readonly IWebFormRenderer _renderer;
        private readonly string _viewName;

        public WebFormsViewFactory(IWebFormsControlBuilder builder, IWebFormRenderer renderer)
        {
            _builder = builder;
            _renderer = renderer;
            _viewName = typeof(TView).ToVirtualPath();
        }

        public IRenderableView GetView()
        {
            var control = _builder.LoadControlFromVirtualPath(_viewName, typeof(IFubuPage));

            return new WebFormsRenderableView(_renderer, control);
        }

        public IRenderableView GetPartialView()
        {
            return GetView();
        }

        public void Describe(Description description)
        {
            description.Title = "FubuMVC.WebForms";
            description.ShortDescription = "Adds webform support to fubumvc";
        }
    }
}