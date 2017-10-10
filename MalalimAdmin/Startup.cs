using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MalalimAdmin.Startup))]
namespace MalalimAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
