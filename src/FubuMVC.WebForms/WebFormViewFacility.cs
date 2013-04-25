using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;

namespace FubuMVC.WebForms
{
    public class WebFormViewFacility : IViewFacility
    {
        public IEnumerable<IViewToken> FindViews(TypePool types)
        {
            return types
                    .TypesMatching(IsWebFormView)
                    .Select(x => new WebFormViewToken(x) as IViewToken);
        }

        public static bool IsWebFormView(Type type)
        {
            return type.CanBeCastTo<Page>() && type.CanBeCastTo<IFubuPage>();
        }

        public static bool IsWebFormControl(Type type)
        {
            return type.CanBeCastTo<UserControl>();
        }

        public IEnumerable<IViewToken> FindViews(BehaviorGraph graph)
        {
            var search = new FileSet() { DeepSearch = true };

            search.AppendInclude("*aspx");
            search.AppendInclude("*Master");
            search.AppendInclude("*ascx");
            search.AppendExclude("bin/*.*");
            search.AppendInclude("obj/*.*");

            throw new NotImplementedException();
        }
    }
}