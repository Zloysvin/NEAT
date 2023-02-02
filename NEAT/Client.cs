using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAT.Calculations;

namespace NEAT
{
    public class Client
    {
        private Calculator calculator;

        public Genome genome;
        public double fitness;
        public Species species;

        public void GenerateCalculator()
        {
            this.calculator = new Calculator(genome);
        }
        public double[] Calculate(double[] inputs)
        {
            return calculator.Calculate(inputs);
        }

        public double[] CalculateSoftMax(double[] inputs)
        {
            return Softmax(calculator.Calculate(inputs));
        }

        public double Distance(Client other)
        {
            return genome.Distance(other.genome);
        }

        public void Mutate()
        {
            genome.Mutate();
        }

        private static double[] Softmax(double[] array)
        {
            double[] result = new double[array.Length];
            double sum = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sum += Math.Exp(array[i]);
            }

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = Math.Exp(array[i]) / sum;
            }

            return result;
        }
    }
}
