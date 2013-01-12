using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Raven.Client.Document;

namespace Decats.UI.Infrastructure
{
    public class RavenDbModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDocumentStore>().ToConstant(Initialize());
            Kernel.Bind<IDocumentSession>().ToMethod(ctx => ctx.Kernel.Get<IDocumentStore>().OpenSession()).InRequestScope();
        }

        public static IDocumentStore Initialize()
        {
            var documentStore = new DocumentStore {ConnectionStringName = "RavenDB"};
            documentStore.Initialize();

            return documentStore;
        }
    }
}