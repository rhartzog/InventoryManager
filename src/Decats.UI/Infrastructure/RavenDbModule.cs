using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;

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
            var documentStore = new EmbeddableDocumentStore
                                    {DataDirectory = @"~\Data"};
            documentStore.Initialize();

            var server = new HttpServer(documentStore.Configuration, documentStore.DocumentDatabase);
            server.StartListening();

            return documentStore;
        }
    }
}