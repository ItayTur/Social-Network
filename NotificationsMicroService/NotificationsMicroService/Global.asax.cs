using BL.Helpers.XMPP;
using BL.Managers;
using Common.Interfaces;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web.Http;

namespace NotificationsMicroService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //SimpleInjector registrations.
            container.Register<INotificationsManager, NotificationsManager>(Lifestyle.Singleton);
            container.Register<INotificationsHelper, XMPPNotificationsHelper>(Lifestyle.Singleton);
            container.Register<ICommonOperationsManager, CommonOperationsManager>(Lifestyle.Singleton);
            
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);


            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
