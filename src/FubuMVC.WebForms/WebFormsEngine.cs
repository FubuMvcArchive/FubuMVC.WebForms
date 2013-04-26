using System;
using FubuMVC.Core;
using FubuMVC.Core.View;

namespace FubuMVC.WebForms
{
    public class WebFormsEngine : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.ViewFacility(new WebFormViewFacility());

            registry.Services(x =>
            {
                x.SetServiceIfNone<IWebFormRenderer, WebFormRenderer>();
                x.SetServiceIfNone<IWebFormsControlBuilder, WebFormsControlBuilder>();
                x.SetServiceIfNone<IPartialRenderer, PartialRenderer>();
                x.SetServiceIfNone<IPartialViewTypeRegistry>(new PartialViewTypeRegistry());
            });
        }


    }
}