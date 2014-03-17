using System.Web.Security;
using DIMS.Core.Entities;
using DIMS.Core.Enumerations;
using DIMS.Data.DataAccess;
using DIMS.UI.Security;
using FluentNHibernate.Utils;
using NHibernate;

[assembly: WebActivator.PreApplicationStartMethod(typeof(DIMS.UI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(DIMS.UI.App_Start.NinjectWebCommon), "Stop")]

namespace DIMS.UI.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //Nhibernate
            kernel.Bind<ISessionFactory>().ToProvider<NhibernateSessionFactoryProvider>().InSingletonScope();
            kernel.Bind<ISession>()
                .ToMethod(context => context.Kernel.Get<ISessionFactory>().OpenSession())
                .InRequestScope();

            //Data Access
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IRepository<Box>>().To<Repository<Box>>();
            kernel.Bind<IRepository<User>>().To<Repository<User>>();
            kernel.Bind<IRepository<Role>>().To<Repository<Role>>();
            kernel.Bind<IRepository<Campus>>().To<Repository<Campus>>();

            //Inject to property of Membership and Role Providers
            kernel.Bind<MembershipProvider>().ToMethod(ctx => Membership.Provider);
            kernel.Bind<RoleProvider>().ToMethod(ctx => Roles.Provider);
            kernel.Bind<IHttpModule>().To<ProviderInitializationHttpModule>();
        }

        public class ProviderInitializationHttpModule : IHttpModule
        {
            public ProviderInitializationHttpModule(MembershipProvider membershipProvider, RoleProvider roleProvider)
            {
            }

            public void Init(HttpApplication context)
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
