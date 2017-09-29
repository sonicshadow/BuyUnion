using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BuyUnion.Startup))]
namespace BuyUnion
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
