using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Hash
{
    public class RandomSelector<T>
    {
        private List<T> objects = new List<T>();
        private List<double> scores = new List<double>();

        private double totalScore = 0;

        public void Add(T obj, double score)
        {
            objects.Add(obj);
            scores.Add(score);
            totalScore += score;
        }

        public T Random()
        {
            Random random = new Random();
            double rand = random.NextDouble() * totalScore;

            double c = 0;
            for (int i = 0; i < objects.Count; i++)
            {
                c += scores[i];
                if (c >= rand)
                {
                    return objects[i];
                }
            }
            return default;
        }

        public void Reset()
        {
            objects.Clear();
            scores.Clear();
            totalScore = 0;
        }
    }
}
