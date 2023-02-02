using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAT.Hash;

namespace NEAT
{
    public class Species
    {
        private RandomHashSet<Client> clients = new RandomHashSet<Client>();
        private Client representative;
        public double fitness;

        public Species(Client representative)
        {
            this.representative = representative;
            representative.species = this;
            clients.Add(representative);
        }

        public bool Put(Client client)
        {
            if (client.Distance(representative) < representative.genome.population.DeltaThreshold)
            {
                client.species = this;
                clients.Add(client);
                return true;
            }

            return false;
        }

        public void ForcePut(Client client)
        {
            client.species = this;
            clients.Add(client);
        }

        public void GoExtinct()
        {
            foreach (var client in clients)
            {
                client.species = null;
                
            }
        }

        public void CalculateFitness()
        {
            fitness = 0;
            foreach (var client in clients)
            {
                fitness += client.fitness;
            }
            fitness /= clients.Count;
        }

        public void Reset()
        {
            representative = clients.GetRandom();
            foreach (var client in clients)
            {
                client.species = null;
            }
            clients.Clear();

            clients.Add(representative);
            representative.species = this;
            fitness = 0;
        }

        public void Kill(double percentage)
        {
            //sort by fitness
            clients.Sort((a, b) => a.fitness.CompareTo(b.fitness));
            double amount = percentage * clients.Count;
            for (int i = 0; i < amount; i++)
            {
                clients[0].species = null;
                clients.Remove(clients[0]);
            }
        }

        public Genome Breed()
        {
            Client parent1 = clients.GetRandom();
            Client parent2 = clients.GetRandom();

            if (parent1.fitness > parent2.fitness)
            {
                return parent1.genome.Crossover(parent2.genome);
            }
            return parent2.genome.Crossover(parent1.genome);
        }

        public int Size()
        {
            return clients.Count;
        }
    }
}
