using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Core.Misc
{
    public static class ArrayTools
    {

        public static int InvalidIndex<TKey, TValue>(this SortedList<TKey, TValue> _this)
        {
            return -1;
        }

        public static List<List<T>> SplitIntoBatches<T>(this List<T> _this, int batchSize)
        {
            var batches = new List<List<T>>();

            for (int i = 0; i < _this.Count; i += batchSize)
            {
                int len = Math.Min(batchSize, _this.Count - i);

                batches.Add(new List<T>(_this.GetRange(i, len)));
            }

            return batches;
        }

        public static int LowerBound<TKey, TValue>(this SortedList<TKey, TValue> _this, TKey key)
        {
            int left = 0;
            int right = _this.Count - 1;
            int result = _this.InvalidIndex();

            while (left <= right)
            {
                int index = (left + right) / 2;

                int compareResult = _this.Comparer.Compare(_this.Keys[index], key);

                if (compareResult > 0)
                {
                    right = index - 1;
                    result = index;
                }
                else if (compareResult < 0)
                {
                    left = index + 1;
                }
                else //if (compareResult == 0)
                {
                    return index;
                }
            }

            return result;
        }

        public static int UpperBound<TKey, TValue>(this SortedList<TKey, TValue> _this, TKey key)
        {
            int left = 0;
            int right = _this.Count - 1;
            int result = _this.InvalidIndex();

            while (left <= right)
            {
                int index = (left + right) / 2;

                int compareResult = _this.Comparer.Compare(_this.Keys[index], key);

                if (compareResult > 0)
                {
                    right = index - 1;
                    result = index;
                }
                else //if (compareResult <= 0)
                {
                    left = index + 1;
                }
            }

            return result;
        }

        /*        public static TKey? BinarySearch<TKey, TValue>(this SortedList<TKey, TValue> _this, TKey key)
                    where TKey : struct
                {
                    int left = 0;
                    int right = _this.Count - 1;

                    while (left <= right)
                    {
                        int index = (left + right) / 2;

                        if (_this.Comparer.Compare(_this.Keys[index], key) > 0)
                        {
                            right = index - 1;
                        }
                        if (_this.Comparer.Compare(_this.Keys[index], key) < 0)
                        {
                            left = index + 1;
                        }
                        if (_this.Comparer.Compare(_this.Keys[index], key) == 0)
                            return _this.Keys[index];
                    }

                    return null;
                }*/
    }
}
