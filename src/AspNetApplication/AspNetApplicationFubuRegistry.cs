using AspNetApplication.WebForms;
using FubuMVC.Core;
using FubuMVC.WebForms;

namespace AspNetApplication
{
    public class AspNetApplicationFubuRegistry : FubuRegistry
    {
        public AspNetApplicationFubuRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();

            Routes.HomeIs<ViewController>(x => x.get_webforms_simple_Name(null));

            Import<WebFormsEngine>();

        }
    }
}