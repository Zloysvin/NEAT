using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class Gene
    {
        public int innovationNumber { get; set; }
        public Gene()
        {

        }

        public Gene(int innovationNumber)
        {
            this.innovationNumber = innovationNumber;
        }
    }
}
