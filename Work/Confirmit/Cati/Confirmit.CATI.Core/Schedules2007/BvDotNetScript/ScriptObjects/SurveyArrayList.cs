using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace BvDotNetScript.ScriptObjects
{
    public class SurveyArrayList : ArrayList
    {
        public SurveyArrayList()
            : base()
        {
        }

        public SurveyArrayList(int size)
            : base(size)
        {
        }

        public SurveyArrayList(Array a)
            : base(a)
        {
        }
        public int length
        {
            get
            {
                return this.Count;
            }
        }

        public override string ToString()
        {
            return this.join();
        }

        public string toString()
        {
            return this.ToString();
        }

        public string valueOf()
        {
            return this.ToString();
        }

        /// <summary>
        /// Returns a string value consisting of all the elements of an array concatenated together and separated by the specified separator character.
        /// </summary>
        /// <param name="separator">A string that is used to separate one element of an array from the next in the resulting String object. If omitted, the array elements are separated with a comma.</param>
        /// <returns></returns>
        public string join(string separator)
        {
            return ArrayListSupport.Join(this, (separator == null ? "," : separator));
        }

        /// <summary>
        /// Returns a string value consisting of all the elements of an array concatenated together and separated by comma.
        /// </summary>
        /// <returns></returns>
        public string join()
        {
            return this.join(null);
        }

        /// <summary>
        /// The reverse method reverses the elements of an Array object in place. It does not create a new Array object during execution. 
        /// </summary>
        /// <returns>Returns an Array object with the elements reversed</returns>
        public SurveyArrayList reverse()
        {
            this.Reverse();
            return this;
        }

        /// <summary>
        /// The concat method returns an Array object containing the concatenation of the current array and any other supplied items. 
        /// The items to be added (item1 ... itemN) to the array are added, in order, from left to right. If one of the items is an array, its contents are added to the end of the current array. If the item is anything other than an array, it is added to the end of the array as a single array element.
        /// Elements of source arrays are copied to the resulting array as follows: 
        /// For an object reference copied from any of the arrays being concatenated to the new array, the object reference continues to point to the same object. A change in either the new array or the original array will result in a change to the other. 
        /// For a numeric or string value being concatenated to the new array, only the value is copied. Changes in a value in one array do not affect the value in the other. 
        /// </summary>
        /// <param name="items">Additional items to add to the end of the current array.</param>
        /// <returns>Returns a new array consisting of a combination of the current array and any additional items.</returns>
        public SurveyArrayList concat(params object[] items)
        {
            SurveyArrayList ret = new SurveyArrayList();
            ret.AddRange(this);
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is ICollection)
                {
                    ret.AddRange((ICollection)items[i]);
                }
                else
                {
                    ret.Add(items[i]);
                }
            }
            return ret;
        }

        /// <summary>
        /// Removes the last element from an array and returns it.
        /// If the array is empty, undefined is returned.
        /// </summary>
        /// <returns></returns>
        public object pop()
        {
            if (this.Count > 0)
            {
                object o = this[this.Count - 1];
                this.RemoveAt(this.Count - 1);
                return o;
            }
            return null;
        }

        /// <summary>
        /// Appends new elements to an array, and returns the new length of the array.
        /// </summary>
        /// <param name="items">New elements of the Array</param>
        /// <returns>new length of the array</returns>
        public int push(params object[] items)
        {
            if (items != null)
                this.AddRange(items);
            return this.Count;
        }

        /// <summary>
        /// The shift method removes the first element from an array and returns it.
        /// </summary>
        /// <returns>the first element</returns>
        public object shift()
        {
            if (this.Count > 0)
            {
                object o = this[0];
                this.RemoveAt(0);
                return o;
            }
            return null;
        }

        /// <summary>
        /// The unshift method inserts elements into the start of an array, 
        /// so they appear in the same order in which they appear in the argument list.
        /// </summary>
        /// <param name="items">Elements to insert at the start of the Array</param>
        /// <returns></returns>
        public SurveyArrayList unshift(params object[] items)
        {
            this.InsertRange(0, items);
            return this;
        }

        /// <summary>
        /// The slice method returns an Array object containing the specified portion of the array. 
        ///	The slice method copies up to, but not including, the element indicated by end. 
        ///	If start is negative, it is treated as length + start where length is the length of the array. 
        ///	If end is negative, it is treated as length + end where length is the length of the array. 
        ///	If end is omitted, extraction continues to the end of the array. 
        ///	If end occurs before start, no elements are copied to the new array.
        /// </summary>
        /// <param name="start">The index to the beginning of the specified portion of the array.</param>
        /// <returns>Returns a section of an array.</returns>
        public SurveyArrayList slice(int start)
        {
            return this.slice(start, this.Count);
        }

        /// <summary>
        /// The slice method returns an Array object containing the specified portion of the array. 
        ///	The slice method copies up to, but not including, the element indicated by end. 
        ///	If start is negative, it is treated as length + start where length is the length of the array. 
        ///	If end is negative, it is treated as length + end where length is the length of the array. 
        ///	If end is omitted, extraction continues to the end of the array. 
        ///	If end occurs before start, no elements are copied to the new array.
        /// </summary>
        /// <param name="start">The index to the beginning of the specified portion of the array. </param>
        /// <param name="end">The index to the end of the specified portion of the array. </param>
        /// <returns>Returns a section of an array.</returns>
        public SurveyArrayList slice(int start, int end)
        {
            int index = (start < 0 ? this.Count + start : start);
            int count = (end < 0 ? this.Count + end : end) - index;
            if (count > 0)
            {
                SurveyArrayList ret = new SurveyArrayList(count);
                ret.AddRange(this.GetRange(index, count));
                return ret;
            }
            return new SurveyArrayList();
        }

        /// <summary>
        /// The sort method sorts the Array object in place; no new Array object is created during execution. 
        /// </summary>
        /// <returns>Returns a SurveyArrayList object with the elements sorted. </returns>
        public SurveyArrayList sort()
        {
            this.Sort();
            return this;
        }



        /// <summary>
        /// The sort method sorts the Array object in place; no new Array object is created during execution. 
        /// </summary>
        /// <param name="functionWrapper">The jscript sort function</param>
        /// <returns>Returns a SurveyArrayList object with the elements sorted. </returns>
        /*public SurveyArrayList sort(FunctionWrapper functionWrapper)
        {
            Array array = JScriptArraySupport.SortArray(this.ToArray(), functionWrapper);
            return new SurveyArrayList(array);
        }*/
        /// <summary>
        /// Removes elements from an array and, if necessary, inserts new elements in their place, returning the deleted elements. Returns the elements removed from the array.
        /// The splice method modifies the array by removing the specified number of elements from position start and inserting new elements. The deleted elements are returned as a new array object.
        /// </summary>
        /// <param name="start">The zero-based location in the array from which to start removing elements</param>
        /// <param name="deleteCount">The number of elements to remove</param>
        /// <param name="items">Elements to insert into the array in place of the deleted elements</param>
        /// <returns>Returns the elements removed from the array.</returns>
        public SurveyArrayList splice(int start, int deleteCount, params object[] items)
        {
            SurveyArrayList ret = new SurveyArrayList(deleteCount);
            ret.AddRange(this.GetRange(start, deleteCount));
            this.RemoveRange(start, deleteCount);
            if (items.Length > 0)
                this.InsertRange(start, items);
            return ret;
        }

    }
}
