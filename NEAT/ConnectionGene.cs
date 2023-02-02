using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class ConnectionGene : Gene
    {
        public NodeGene from { get; set; }
        public NodeGene to { get; set; }
        public double weight { get; set; }
        public bool enabled { get; set; }

        public int replaceIndex;

        public ConnectionGene(NodeGene from, NodeGene to)
        {
            this.from = from;
            this.to = to;
        }

        //override equals and hashcode
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(ConnectionGene))
            {
                return false;
            }

            ConnectionGene other = (ConnectionGene)obj;
            return from.Equals(other.from) && to.Equals(other.to);
        }

        public override int GetHashCode()
        {
            return from.innovationNumber * Population.maxNodes + to.innovationNumber;
        }
    }
}
