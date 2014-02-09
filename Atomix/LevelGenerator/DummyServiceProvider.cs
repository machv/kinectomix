using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinectomix.LevelGenerator
{

    public class DummyServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
