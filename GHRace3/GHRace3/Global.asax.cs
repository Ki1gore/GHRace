using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GHRace3.DBService;
using GHRace3.Models;
using System.Data.Entity;
using Autofac;
using Utilities;
using Interfaces;
using RaceDataService;
using System.Web.Security;
using Autofac.Integration.Mvc;
using System.Reflection;

namespace GHRace3
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static IContainer Container { get; private set; }

        protected void Application_Start()
        {
            Database.SetInitializer<GHRaceContext>(new GHRaceInitializer());
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            RegisterTypes();
        }

        private void RegisterTypes()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CommonUtilities>().As<ICommonUtilities>();
            builder.RegisterType<RetrieveRaceData>().As<IRetrieveRaceData>();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }

    }
}