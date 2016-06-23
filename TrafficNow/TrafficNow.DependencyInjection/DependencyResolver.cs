using Ninject;

namespace TrafficNow.DependencyInjection
{
    public class DependencyResolver
    {
        public static IKernel Kernel { get; set; }
        public void Resolve()
        {
            Kernel = new StandardKernel();
            var serviceRegister = new DependencyServiceRegister();
            serviceRegister.Register(Kernel);
        }
    }
}
