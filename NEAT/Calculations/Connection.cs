using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Calculations
{
    public class Connection
    {
        public Node from { get; set; }
        public Node to { get; set; }
        public double weight { get; set; }
        public bool enabled { get; set; }

        public Connection(Node from, Node to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
