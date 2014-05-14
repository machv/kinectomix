using System;

namespace KinectomixLogic
{

    public class DummyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
