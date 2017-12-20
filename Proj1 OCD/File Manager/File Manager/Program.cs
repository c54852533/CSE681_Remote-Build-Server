using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======Loading DLL files========");
            string DLLPath = "../../../Tests";
            string[] files = System.IO.Directory.GetFiles(DLLPath, "*.dll");
            foreach (string file in files)
            {
                Console.Write("\n  loading: \"{0}\"", file);
            }
            Console.WriteLine("\n======Finished file manager prototype========\n");
            Console.ReadLine();
        }
    }
}
