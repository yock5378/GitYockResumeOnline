using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(YockResume.Startup))]
namespace YockResume
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
