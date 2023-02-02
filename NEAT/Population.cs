using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using NEAT.Hash;

namespace NEAT
{
    public class Population
    {
        public static int maxNodes = 1000000;

        public double c1 = 1.0;
        public double c2 = 1.0;
        public double c3 = 1.0;
        public double DeltaThreshold = 3.0;

        public double WeightShiftStrength = 0.5;
        public double WeightRandomStrength = 0.5;

        private double survivors = 0.8;

        public double ProbabilityMutateLink = 0.3;
        public double ProbabilityMutateNode = 0.1;
        public double ProbabilityMutateWeightShift = 0.2;
        public double ProbabilityMutateWeightRandom = 0.2;
        public double ProbabilityMutateToggle = 0.01;

        private Dictionary<ConnectionGene, ConnectionGene> allConnections =
            new Dictionary<ConnectionGene, ConnectionGene>();
        private RandomHashSet<NodeGene> allNodes = new RandomHashSet<NodeGene>();

        public RandomHashSet<Client> clients = new RandomHashSet<Client>();
        private RandomHashSet<Species> species = new RandomHashSet<Species>();

        private int InputSize;
        private int OutputSize;
        private int MaxClients;

        public void UpdateMutationProbabilities(double probabilityMutateLink, double probabilityMutateNode, double probabilityMutateWeightShift, double probabilityMutateWeightRandom, double probabilityMutateToggle)
        {
            ProbabilityMutateLink = probabilityMutateLink;
            ProbabilityMutateNode = probabilityMutateNode;
            ProbabilityMutateWeightShift = probabilityMutateWeightShift;
            ProbabilityMutateWeightRandom = probabilityMutateWeightRandom;
            ProbabilityMutateToggle = probabilityMutateToggle;
        }

        public Population(int inputSize, int outputSize, int clients)
        {
            Reset(inputSize, outputSize, clients);
        }

        public void Reset(int inputSize, int outputSize, int clients)
        {
            InputSize = inputSize;
            OutputSize = outputSize;
            MaxClients = clients;

            allConnections.Clear();
            allNodes.Clear();
            this.clients.Clear();

            for (int i = 0; i < inputSize; i++)
            {
                NodeGene node = GetNode();
                node.x = 0.1;
                node.y = (i + 1.0) / (inputSize + 1);
            }

            for (int i = 0; i < outputSize; i++)
            {
                NodeGene node = GetNode();
                node.x = 0.9;
                node.y = (i + 1.0) / (inputSize + 1);
            }

            for (int i = 0; i < clients; i++)
            {
                Client client = new Client();
                client.genome = CreateEmptyGenome();
                client.GenerateCalculator();
                this.clients.Add(client);
            }
        }

        public Client GetClient(int index)
        {
            return clients[index];
        }

        public Genome CreateEmptyGenome()
        {
            Genome genome = new Genome(this);
            for (int i = 0; i < InputSize + OutputSize; i++)
            {
                genome.nodes.Add(GetNode(i + 1));
            }
            return genome;
        }

        public NodeGene GetNode()
        {
            NodeGene node = new NodeGene(allNodes.Count + 1);
            allNodes.Add(node);
            return node;
        }

        public NodeGene GetNode(int id)
        {
            if (id <= allNodes.Count)
            {
                return allNodes[id - 1];
            }
            return GetNode();
        }

        public ConnectionGene GetConnection(ConnectionGene con)
        {
            ConnectionGene connection = new ConnectionGene(con.from, con.to);
            connection.innovationNumber = con.innovationNumber;
            connection.weight = con.weight;
            connection.enabled = con.enabled;
            return connection;
        }

        public ConnectionGene GetConnection(NodeGene from, NodeGene to)
        {
            ConnectionGene connection = new ConnectionGene(from, to);
            if (allConnections.ContainsKey(connection))
            {
                connection.innovationNumber = allConnections[connection].innovationNumber;
            }
            else
            {
                connection.innovationNumber = allConnections.Count + 1;
                allConnections[connection] = connection;
            }
            return connection;
        }

        public void Evolve()
        {
            GenerateSpecies();
            Kill();
            RemoveEtinctSpecies();
            Breed();
            Mutate();
            foreach (Client client in clients)
            {
                client.GenerateCalculator();
            }
        }

        private void Mutate()
        {
            foreach (Client client in clients)
            {
                client.Mutate();
            }
        }

        private void Breed()
        {
            RandomSelector<Species> selector = new RandomSelector<Species>();
            foreach (Species specie in species)
            {
                selector.Add(specie, specie.fitness);
            }

            foreach (var client in clients)
            {
                if (client.species == null)
                {
                    Species species = selector.Random();
                    client.genome = species.Breed();
                    species.ForcePut(client);
                }
            }
        }

        private void RemoveEtinctSpecies()
        {
            for (int i = species.Count - 1; i >= 0; i--)
            {
                if (species[i].Size() <= 1)
                {
                    species[i].GoExtinct();
                    species.RemoveAt(i);
                }
            }
        }

        private void Kill()
        {
            foreach (Species species in species)
            {
                species.Kill(1 - survivors);
            }
        }

        private void GenerateSpecies()
        {
            foreach (Species specie in species)
            {
                specie.Reset();
            }

            foreach (Client client in clients)
            {
                if (client.species != null)
                {
                    continue;
                }

                bool foundSpecies = false;
                foreach (Species specie in species)
                {
                    if (specie.Put(client))
                    {
                        foundSpecies = true;
                        break;
                    }
                }

                if (!foundSpecies)
                {
                    species.Add(new Species(client));
                }
            }

            foreach (Species species in species)
            {
                species.CalculateFitness();
            }
        }

        public void SetReplaceIndex(NodeGene node1, NodeGene node2, int index)
        {
            allConnections[new ConnectionGene(node1, node2)].replaceIndex = index;
        }
        public int GetReplaceIndex(NodeGene node1, NodeGene node2)
        {
            ConnectionGene con = new ConnectionGene(node1, node2);
            ConnectionGene data = allConnections[con];
            if(data == null)
                return 0;
            return data.replaceIndex;
        }

        public void PrintSpecies()
        {
            Console.WriteLine();

            foreach (Species species in species)
            {
                Console.WriteLine(species.fitness + " " + species.Size());
            }
        }
    }
}
