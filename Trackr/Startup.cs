using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Trackr.Startup))]
namespace Trackr
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            Telerik.OpenAccess.ServiceHost.ServiceHostManager.StartProfilerService(15555);
            ConfigureAuth(app);
        }
    }
}
