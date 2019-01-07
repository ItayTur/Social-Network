using BL.Managers;
using Common.Interfaces;
using Common.Interfaces.Helpers;
using Common.Loggers;
using DAL.Repositories;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace AuthenticationApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //SimpleInjector registrations.
            container.Register<IAuthRepository, DynamoDbAuthRepository>(Lifestyle.Singleton);
            container.Register<ILoginTokenRepository, DynamoDbLoginTokenRepository>(Lifestyle.Singleton);
            container.Register<IAuthManager, AuthManager>(Lifestyle.Singleton);
            container.Register<ILoginTokenManager, LoginTokenManager>(Lifestyle.Singleton);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);


            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

                       

            GlobalConfiguration.Configure(WebApiConfig.Register);

            LoggerFactory.Init(null, ConfigurationManager.AppSettings["LogFile"]);

        }
    }
}
