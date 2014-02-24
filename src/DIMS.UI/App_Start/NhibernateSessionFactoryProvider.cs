using NHibernate;
using Ninject.Activation;

namespace DIMS.UI
{
    public class NhibernateSessionFactoryProvider : Provider<ISessionFactory>
    {
        protected override ISessionFactory CreateInstance(IContext context)
        {
            var sessionFactory = new NhibernateSessionFactory();
            return sessionFactory.GetSessionFactory();
        } 
    }
}