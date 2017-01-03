using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(vote.Startup))]
namespace vote
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
