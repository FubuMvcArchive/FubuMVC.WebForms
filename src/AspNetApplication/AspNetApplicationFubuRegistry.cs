using FubuMVC.Core;
using FubuMVC.WebForms;

namespace AspNetApplication
{
    public class AspNetApplicationFubuRegistry : FubuRegistry
    {
        public AspNetApplicationFubuRegistry()
        {
            Actions.IncludeClassesSuffixedWithController();

            Import<WebFormsEngine>();

        }
    }
}