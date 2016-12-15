using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityShutdown
{
    class Program
    {
        static void Main(string[] args)
        {
            WindowsServiceWithTopshelf.ConfigureService.Configure();
        }
    }
}
