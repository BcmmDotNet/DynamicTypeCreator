using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTypeCreator
{
    public class DispatchProxyBase
    {
        public void Invoke()
        {
            Console.WriteLine("This is DispatchProxyBase.");
        }
    }
}
