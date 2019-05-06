using System;
using System.Collections.Generic;
using System.Linq;

namespace AMazeCS
{
    public class RandomQueue<T>
    {
        private readonly LinkedList<T> Queue;
        private readonly Random random = new Random();
        public RandomQueue(T[] array)
        {
            Queue = new LinkedList<T>(array);
        }
        public RandomQueue()
        {
            Queue = new LinkedList<T>();
        }

        public void Enqueue(T value)
        {
            if(random.Next(0, 101) < 50)
            {
                Queue.AddFirst(value);
            }
            else
            {
                Queue.AddLast(value);
            }
        }

        public T Dequeue()
        {
            if(Queue.Count == 0)
            {
                return default(T);
            }
            if (random.Next(0, 101) < 50)
            {
                var returnValue = Queue.First<T>();
                Queue.RemoveFirst();
                return returnValue;
            }
            else
            {
                var returnValue = Queue.Last<T>();
                Queue.RemoveLast();
                return returnValue;
            }
        }

        public int Count
        {
            get
            {
                return Queue.Count;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return Queue.Count == 0;
            }
        }
    }
}
