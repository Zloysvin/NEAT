using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAT.Calculations;
using NEAT.Hash;

namespace NEAT
{
    public class Genome
    {
        public RandomHashSet<ConnectionGene> connections { get; set; }
        public RandomHashSet<NodeGene> nodes { get; set; }

        public Population population { get; set; }

        private Calculator calculator { get; set; }

        public Genome(Population population)
        {
            this.population = population;
            connections = new RandomHashSet<ConnectionGene>();
            nodes = new RandomHashSet<NodeGene>();
        }

        public double Distance(Genome g2)
        {
            Genome g1 = this;

            int highestInnovation1 = 0;
            if (g1.connections.Count != 0)
            {
                highestInnovation1 = g1.connections[g1.connections.Count - 1].innovationNumber;
            }

            int highestInnovation2 = 0;
            if (g2.connections.Count != 0)
            {
                highestInnovation2 = g2.connections[g2.connections.Count - 1].innovationNumber;
            }

            if (highestInnovation1 < highestInnovation2)
            {
                (g1, g2) = (g2, g1);
            }

            int index1 = 0;
            int index2 = 0;

            int disjoint = 0;
            int excess = 0;
            double weightDiff = 0;
            int similar = 0;

            while (index1 < g1.connections.Count && index2 < g2.connections.Count)
            {
                ConnectionGene gene1 = g1.connections.ElementAt(index1);
                ConnectionGene gene2 = g2.connections.ElementAt(index2);

                int in1 = gene1.innovationNumber;
                int in2 = gene2.innovationNumber;

                if (in1 == in2)
                {
                    similar++;
                    weightDiff += Math.Abs(gene1.weight - gene2.weight);
                    index1++;
                    index2++;
                }
                else if (in1 > in2)
                {
                    disjoint++;
                    index2++;
                }
                else
                {
                    disjoint++;
                    index1++;
                }
            }

            weightDiff /= similar;
            excess = g1.connections.Count - index1;

            double n = Math.Max(g1.connections.Count, g2.connections.Count);
            if (n < 20)
            {
                n = 1;
            }

            return disjoint / n * population.c1 + excess / n * population.c2 + weightDiff * population.c3;
        }

        public Genome Crossover(Genome g2)
        {
            Genome g1 = this;
            
            Population p = g1.population;

            Genome genome = p.CreateEmptyGenome();

            int index1 = 0;
            int index2 = 0;
            Random rnd = new Random();

            while (index1 < g1.connections.Count && index2 < g2.connections.Count)
            {
                ConnectionGene gene1 = g1.connections.ElementAt(index1);
                ConnectionGene gene2 = g2.connections.ElementAt(index2);

                int in1 = gene1.innovationNumber;
                int in2 = gene2.innovationNumber;

                if (in1 == in2)
                {
                    if (rnd.NextDouble() > 0.5)
                    {
                        genome.connections.Add(population.GetConnection(gene1));
                    }
                    else
                    {
                        genome.connections.Add(population.GetConnection(gene2));
                    }

                    index1++;
                    index2++;
                }
                else if (in1 > in2)
                {
                    index2++;
                }
                else
                {
                    index1++;
                }
            }

            while (index1 < g1.connections.Count)
            {
                ConnectionGene gene1 = g1.connections.ElementAt(index1);
                genome.connections.Add(population.GetConnection(gene1));
                index1++;
            }

            foreach (ConnectionGene gene in genome.connections)
            {
                genome.nodes.Add(gene.from);
                genome.nodes.Add(gene.to);
            }

            return genome;
        }

        public void Mutate()
        {
            Random rnd = new Random();
            if (rnd.NextDouble() < population.ProbabilityMutateLink)
            {
                MutateLink();
            }
            if (connections.Count > 0)
            {
                if (rnd.NextDouble() < population.ProbabilityMutateNode)
                {
                    MutateNode();
                }

                if (rnd.NextDouble() < population.ProbabilityMutateWeightShift)
                {
                    MutateWeightShift();
                }

                if (rnd.NextDouble() < population.ProbabilityMutateWeightRandom)
                {
                    MutateWeightRandom();
                }

                if (rnd.NextDouble() < population.ProbabilityMutateToggle)
                {
                    MutateLinkToggle();
                }
            }
        }

        public void MutateLink()
        {
            for (int i = 0; i < 100; i++)
            {
                NodeGene a = nodes.GetRandom();
                NodeGene b = nodes.GetRandom();

                if (a.x == b.x)
                {
                    continue;
                }

                ConnectionGene gene;
                if (a.x < b.x)
                {
                    gene = new ConnectionGene(a, b);
                }
                else
                {
                    gene = new ConnectionGene(b, a);
                }

                if (connections.Contains(gene))
                {
                    continue;
                }

                gene = population.GetConnection(gene.from, gene.to);
                gene.enabled = true;
                Random rnd = new Random();
                gene.weight = (rnd.NextDouble() * 2 - 1) * population.WeightRandomStrength;

                connections.AddSorted(gene);
                
                return;
            }
        }

        public void MutateNode()
        {
            Random rnd = new Random();
            ConnectionGene gene = connections.GetRandom();
            
            if(gene == null)
                return;

            NodeGene from = gene.from;
            NodeGene to = gene.to;

            NodeGene middle;
            int replaceindex = population.GetReplaceIndex(from, to);
            if (replaceindex == 0)
            {
                middle = population.GetNode();
                middle.x = (from.x + to.x) / 2;
                middle.y = (from.y + to.y) / 2 + rnd.NextDouble() * 0.1 - 0.05;
                population.SetReplaceIndex(from, to, middle.innovationNumber);
            }
            else
            {
                middle = population.GetNode(replaceindex);
            }

            ConnectionGene gene1 = population.GetConnection(from, middle);
            ConnectionGene gene2 = population.GetConnection(middle, to);

            gene1.weight = 1;
            gene2.weight = gene.weight;
            gene2.enabled = gene.enabled;

            connections.Remove(gene);
            connections.Add(gene1);
            connections.Add(gene2);

            nodes.Add(middle);
        }

        public void MutateWeightShift()
        {
            ConnectionGene gene = connections.GetRandom();
            if (gene != null)
            {
                Random rnd = new Random();
                gene.weight += (rnd.NextDouble() * 2 - 1) * population.WeightShiftStrength;
            }
        }

        public void MutateWeightRandom()
        {
            ConnectionGene gene = connections.GetRandom();
            if (gene != null)
            {
                Random rnd = new Random();
                gene.weight = (rnd.NextDouble() * 2 - 1) * population.WeightRandomStrength;
            }
        }

        public void MutateLinkToggle()
        {
            ConnectionGene gene = connections.GetRandom();
            if (gene != null)
            {
                gene.enabled = !gene.enabled;
            }
        }

        public void GenerateCalculator()
        {
            calculator = new Calculator(this);
        }

        public double[] Calculate(double[] inputs)
        {
            return calculator.Calculate(inputs);
        }
    }
}
