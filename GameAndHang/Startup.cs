using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GameAndHang.Startup))]
namespace GameAndHang
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
