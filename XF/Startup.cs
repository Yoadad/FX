using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XF.Startup))]
namespace XF
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
