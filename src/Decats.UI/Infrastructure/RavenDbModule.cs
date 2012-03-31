using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Modules;
using Raven.Client;
using Raven.Client.Embedded;

namespace Decats.UI.Infrastructure
{
    public class RavenDbModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDocumentStore>().ToConstant(new EmbeddableDocumentStore { DataDirectory = "~/App_Data/RavenDB" }.Initialize());
            Kernel.Bind<IDocumentSession>().ToMethod(ctx => ctx.Kernel.Get<IDocumentStore>().OpenSession());
        }
    }
}