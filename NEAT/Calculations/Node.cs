using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Calculations
{
    public class Node : IComparable<Node>
    {
        public double x;
        public double output;
        public List<Connection> connections = new List<Connection>();

        public Node(double x)
        {
            this.x = x;
        }

        public int CompareTo(Node other)
        {
            if (x < other.x)
            {
                return -1;
            }
            else if (x > other.x)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void Calculate()
        {
            double sum = 0;
            foreach (var connection in connections)
            {
                if (connection.enabled)
                {
                    sum += connection.weight * connection.from.output;
                }
            }
            output = Sigmoid(sum);
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
    }
}
