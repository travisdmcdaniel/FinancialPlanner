using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinancialPlanner.Startup))]
namespace FinancialPlanner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
