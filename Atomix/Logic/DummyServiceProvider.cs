using System;

namespace Mach.Kinectomix.Logic
{

    public class DummyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
