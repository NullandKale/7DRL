using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DRL
{
    public class Tile
    {
        public char Visual;
        public bool collideable;

        public override string ToString()
        {
            return Visual.ToString();
        }
    }
}
