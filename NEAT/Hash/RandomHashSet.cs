using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Hash
{
    public class RandomHashSet<T> : IEnumerable<T>
    {
        private Dictionary<T, int> _dict;
        private List<T> _list;
        private Random _random;

        public RandomHashSet()
        {
            _dict = new Dictionary<T, int>();
            _list = new List<T>();
            _random = new Random();
        }

        //contains
        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        public void Add(T item)
        {
            if (!_dict.ContainsKey(item))
            {
                _dict.Add(item, _list.Count);
                _list.Add(item);
            }
        }

        public void AddSorted(T item)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                int innovationNumber = (_list[i] as Gene).innovationNumber;
                if (innovationNumber > (item as Gene).innovationNumber)
                {
                    _dict.Add(item, i);
                    _list.Insert(i, item);
                    return;
                }
            }
            _dict.Add(item, _list.Count);
            _list.Add(item);
        }

        public void Remove(T item)
        {
            if (_dict.ContainsKey(item))
            {
                int index = _dict[item];
                _dict.Remove(item);
                _list.Remove(item);
                //_list[index] = _list[_list.Count - 1];
                //_list.RemoveAt(_list.Count - 1);
            }
        }

        //remove at
        public void RemoveAt(int index)
        {
            T item = _list[index];
            _dict.Remove(item);
            _list[index] = _list[_list.Count - 1];
            _list.RemoveAt(_list.Count - 1);
        }

        //clear method
        public void Clear()
        {
            _dict.Clear();
            _list.Clear();
        }

        //get data
        public T this[int index]
        {
            get { return _list[index]; }
        }

        public T GetRandom()
        {
            return _list[_random.Next(_list.Count)];
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public int Max(Func<object, object> func)
        {
            int max = 0;
            foreach (T item in _list)
            {
                int value = (int)func(item);
                if (value > max)
                {
                    max = value;
                }
            }
            return max;
        }

        public ConnectionGene ElementAt(int index1)
        {
            return _list[index1] as ConnectionGene;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Sort(Func<T, T, int> func)
        {
            _list.Sort((x, y) => func(x, y));
        }
    }
}
