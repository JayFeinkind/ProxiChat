using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProxyChatService.Startup))]
namespace ProxyChatService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
        }
    }
}
