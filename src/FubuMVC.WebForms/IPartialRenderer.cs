using System;
using System.Globalization;
using System.IO;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;

namespace FubuMVC.WebForms
{
    public interface IPartialRenderer
    {
        IFubuPage CreateControl<TView, TViewModel>(IServiceLocator locator, TViewModel model)
            where TView : IFubuPage
            where TViewModel : class;

        IFubuPage CreateControl<TViewModel>(IServiceLocator locator, Type controlType, TViewModel model) where TViewModel : class;

        string Render<T>(IFubuPage view, T viewModel, string prefix, int? index = null) where T : class;
        void Render<T>(IFubuPage view, T viewModel, string prefix, TextWriter writer, int? index = null) where T : class;

        string Render<T>(IFubuPage parentPage, IFubuPage partialControl, T viewModel, string prefix) where T : class;
        string Render<T>(IFubuPage view, Type controlType, T viewModel, string prefix) where T : class;
        void Render<T>(IFubuPage view, Type controlType, T viewModel, string prefix, TextWriter writer) where T : class;
    }

    public class PartialRenderer : IPartialRenderer
    {
        private readonly IWebFormsControlBuilder _builder;
        private readonly IFubuRequest _request;

        public PartialRenderer(IWebFormsControlBuilder builder, IFubuRequest request)
        {
            _builder = builder;
            _request = request;
        }

        public IFubuPage CreateControl<TView, TViewModel>(IServiceLocator locator, TViewModel model)
            where TView : IFubuPage
            where TViewModel : class
        {
            return CreateControl(locator, typeof(TView), model);
        }

        public IFubuPage CreateControl<TViewModel>(IServiceLocator locator, Type controlType, TViewModel model) where TViewModel : class
        {
            //TODO: I'm not sure this IF is required any more, I think that's what the
            // second arg to LoadControlFromVirtualPath does. This needs some investigation before
            // deleting.
            if (!typeof(IFubuPage).IsAssignableFrom(controlType) || !typeof(Control).IsAssignableFrom(controlType))
            {
                throw new InvalidOperationException(String.Format(
                                                        "PartialRenderer cannot render type '{0}'. It can only render System.Web.UI.Control objects which implement the IFubuPage interface.",
                                                        (controlType == null) ? "(null)" : controlType.Name));
            }

            var virtualPath = controlType.ToVirtualPath();
            var control = _builder.LoadControlFromVirtualPath(virtualPath, controlType);
            var controlAsPage = getControlPage(control, model, locator);
            return controlAsPage;
        }

        private IFubuPage getControlPage<TViewModel>(Control control, TViewModel model, IServiceLocator locator) where TViewModel : class
        {
            // Attaching the ServiceLocator to the page for the changes made on WebForms
            var controlAsPage = control as IFubuPage<TViewModel>;
            if (controlAsPage != null)
            {
                controlAsPage.Model = model;
                controlAsPage.ServiceLocator = locator;
                return controlAsPage;
            }

            var fubuPage = control as IFubuPage;
            if (fubuPage != null) fubuPage.ServiceLocator = locator;
            return fubuPage;
        }

        public string Render<T>(IFubuPage view, T viewModel, string prefix, int? index = null) where T : class
        {
            var writer = new StringWriter(CultureInfo.CurrentCulture);
            Render(view, viewModel, prefix, writer, index);
            return writer.GetStringBuilder().ToString();
        }

        public void Render<TViewModel>(IFubuPage view, TViewModel viewModel, string prefix, TextWriter writer, int? index = null) where TViewModel : class
        {
            var page = new Page();
            page.Controls.Add(view as Control);

            var shouldClearModel = false;
            if (viewModel != null)
            {
                shouldClearModel = !_request.Has(viewModel.GetType());
                _request.Set(viewModel.GetType(), viewModel);
                (view as IFubuPage<TViewModel>).Model = viewModel;
            }

            setParentPageIfNotAlreadySet(view, page);

            if (index.HasValue)
            {
                prefix = "{0}[{1}]".ToFormat(prefix, index);
            }

            view.ElementPrefix = prefix;

            _builder.ExecuteControl(page, writer);

            writer.Flush();

            if (shouldClearModel)
            {
                _request.Clear(viewModel.GetType());
            }
        }

        public string Render<TViewModel>(IFubuPage parentView, Type controlType, TViewModel viewModel, string prefix) where TViewModel : class
        {
            var view = CreateControl(parentView.ServiceLocator, controlType, viewModel);

            setParentPageIfNotAlreadySet(view, (Control)parentView);

            return Render(view, viewModel, prefix);
        }

        public string Render<TViewModel>(IFubuPage parentPage, IFubuPage partialControl, TViewModel viewModel, string prefix) where TViewModel : class
        {
            setParentPageIfNotAlreadySet(partialControl, (Control)parentPage);

            return Render(partialControl, viewModel, prefix);
        }

        public void Render<TViewModel>(IFubuPage parentView, Type controlType, TViewModel viewModel, string prefix, TextWriter writer) where TViewModel : class
        {
            var view = CreateControl(parentView.ServiceLocator, controlType, viewModel);

            setParentPageIfNotAlreadySet(view, (Control)parentView);

            Render(view, viewModel, prefix, writer);
        }

        private static void setParentPageIfNotAlreadySet(IFubuPage view, Control parent)
        {
            var controlThatNeedsParent = view as INeedToKnowAboutParentPage;
            if (controlThatNeedsParent == null) return;
            if (controlThatNeedsParent.ParentPage != null) return;

            controlThatNeedsParent.ParentPage = parent as Page ?? parent.Page;
        }
    }
}