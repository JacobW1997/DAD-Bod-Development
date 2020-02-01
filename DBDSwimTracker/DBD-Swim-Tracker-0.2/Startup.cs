using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DBD_Swim_Tracker_0._2.Startup))]
namespace DBD_Swim_Tracker_0._2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
