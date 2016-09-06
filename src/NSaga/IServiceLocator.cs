using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSaga
{
    public interface IServiceLocator
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
