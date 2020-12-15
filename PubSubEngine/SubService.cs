using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public class SubService : ISubscribe
    {

        public void Subscribe(int from, int to)
        {
            Console.WriteLine($"Subccriber XYZ subcribed to [{from}-{to}]");
        }
    }
}
