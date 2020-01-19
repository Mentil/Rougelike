using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rougelike.Core
{

    public class World : IWorld
    {
        public double Sum { get; set; }

        public double A { get; set; }
        public double B { get; set; }

        public World()
        {
            A = 5;
            B = 5;
        }

        public double GetSum()
        {
            Sum = A + B;
            return Sum;
        }
    }
}
