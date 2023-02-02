using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAT.Hash;

namespace NEAT.Calculations
{
    public class Calculator
    {
        public List<Node> InputNodes = new List<Node>();
        public List<Node> HiddenNodes = new List<Node>();
        public List<Node> OutputNodes = new List<Node>();

        public Calculator(Genome genome)
        {
            RandomHashSet<NodeGene> nodes = genome.nodes;
            RandomHashSet<ConnectionGene> connections = genome.connections;

            Dictionary<int, Node> nodeDict = new Dictionary<int, Node>();

            foreach (NodeGene n in nodes)
            {
                Node node = new Node(n.x);
                nodeDict.Add(n.innovationNumber, node);

                if (n.x <= 0.1)
                {
                    InputNodes.Add(node);
                }
                else if (n.x >= 0.9)
                {
                    OutputNodes.Add(node);
                }
                else
                {
                    HiddenNodes.Add(node);
                }
            }
            HiddenNodes.Sort();

            foreach (var connection in connections)
            {
                NodeGene from = connection.from;
                NodeGene to = connection.to;

                Node fromNode = nodeDict[from.innovationNumber];
                Node toNode = nodeDict[to.innovationNumber];

                Connection c = new Connection(fromNode, toNode);
                c.weight = connection.weight;
                c.enabled = connection.enabled;

                toNode.connections.Add(c);
            }
        }

        public double[] Calculate(double[] inputs)
        {
            for (int i = 0; i < InputNodes.Count; i++)
            {
                InputNodes[i].output = inputs[i];
            }

            foreach (var node in HiddenNodes)
            {
                node.Calculate();
            }

            double[] outputs = new double[OutputNodes.Count];
            for (int i = 0; i < OutputNodes.Count; i++)
            {
                OutputNodes[i].Calculate();
                outputs[i] = OutputNodes[i].output;
            }

            return outputs;
        }
    }
}
