using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorDialerDriver.Services
{
    public static class LinkedListExtensions
    {
        public static LinkedListNode<T> Find<T>(this LinkedList<T> _this, Func<T, bool> predicate)
        {
            LinkedListNode<T> linkedListNode = _this.First;

            if (linkedListNode == null)
                return null;

            while (!predicate(linkedListNode.Value))
            {
                linkedListNode = linkedListNode.Next;
                if (linkedListNode == _this.First || linkedListNode == null)
                    return null;
            }
            return linkedListNode;
        }
    }
}
