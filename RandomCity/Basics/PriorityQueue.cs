namespace System.Collections.Generic
{
    public delegate int Comparator<T>(T obj1, T obj2);
    public class PriorityQueue<T>:IEnumerable<T>
    {
        private readonly List<T> array = new List<T>();
        private readonly Comparator<T> compare;

        private T back;

        public PriorityQueue():this(delegate (T obj1, T obj2)
        {
            return (obj1 as IComparable).CompareTo(obj2);
        })
        {
            T temp = default(T);
            if(!(temp is IComparable))
            {
                throw new Exception("Elements should be IComparable.");
            }
        }
        public PriorityQueue(Comparator<T> comparator)
        {
            compare = comparator;
        }

        public T Back
        {
            get
            {
                if(IsEmpty)
                {
                    throw new IndexOutOfRangeException();
                }
                return back;
            }
        }
        public T Front
        {
           get
            {
                return array[0];
            }
        }

        public T Dequeue()
        {
            var result = Front;
            array[0] = array[array.Count - 1];
            array.RemoveAt(array.Count - 1);
            int currentIndex = 0;
            if(array.Count > 1)
            {
                while(currentIndex * 2 < array.Count)
                {
                    int left = currentIndex * 2;
                    int right = currentIndex * 2 + 1;
                    var currentNode = array[currentIndex];
                    var leftNode = array[left];
                    if(right >= array.Count)
                    {
                        if(compare(leftNode, currentNode) > 0)
                        {
                            var temp = leftNode;
                            array[left] = currentNode;
                            array[currentIndex] = temp;
                        }
                        break;
                    }
                    var rightNode = array[right];
                    if(compare(currentNode, leftNode) >= 0 &&
                        compare(currentNode, rightNode) >= 0)
                    {
                        break;
                    }
                    if(compare(leftNode, rightNode) > 0)
                    {
                        var temp = leftNode;
                        array[left] = currentNode;
                        array[currentIndex] = temp;
                        currentIndex *= 2;
                    }
                    else
                    {
                        var temp = rightNode;
                        array[right] = currentNode;
                        array[currentIndex] = temp;
                        currentIndex *= 2;
                        currentIndex++;
                    }
                }
            }
            return result;
        }
        public void Enqueue(T newElement)
        {
            if(IsEmpty || compare(newElement, back) < 0)
            {
                back = newElement;
            }
            int currentIndex = array.Count;
            array.Add(newElement);
            while(currentIndex > 0)
            {
                var parentNode = array[currentIndex / 2];
                var currentNode = array[currentIndex];
                if(compare(parentNode, currentNode) < 0)
                {
                    var temp = parentNode;
                    array[currentIndex / 2] = currentNode;
                    array[currentIndex] = temp;
                }
                else
                {
                    break;
                }
                currentIndex /= 2;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return array.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return array.Count;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return array.Count == 0;
            }
        }
        public T this[int key]
        {
            get
            {
                return array[key];
            }
        }
    }
}
