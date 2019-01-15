using BL.Managers;
using Common.Interfaces;
using DAL.Repositories;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace IdentityServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //SimpleInjector registrations.
            container.Register<IUsersRepository, DynamoDbUsersRepository>(Lifestyle.Singleton);
            container.Register<IUsersManager, UsersManager>(Lifestyle.Singleton);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);


            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);



            GlobalConfiguration.Configure(WebApiConfig.Register);

            //LoggerFactory.Init(null, ConfigurationManager.AppSettings["LogFile"]);
        }
    }
}
