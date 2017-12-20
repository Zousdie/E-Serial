using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = Environment.NewLine;
            foreach (var item in Encoding.UTF8.GetBytes(s))
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }
}
