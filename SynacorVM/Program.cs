using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynacorVM
{
    class Program
    {
        static void Main(string[] args)
        {
            VM Synacor = new VM();
            Synacor.Read("challenge.bin");
            Synacor.Run();
            Console.ReadKey();
        }
    }
}
