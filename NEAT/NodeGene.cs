using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class NodeGene : Gene
    {
        public double x {get; set;}
        public double y { get; set; }

        public NodeGene(int innovationNumber) : base(innovationNumber)
        {
        }

        //override equals and hashcode
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(NodeGene))
            {
                return false;
            }

            return innovationNumber == ((NodeGene)obj).innovationNumber;
        }

        public override int GetHashCode()
        {
            return innovationNumber;
        }
    }
}
