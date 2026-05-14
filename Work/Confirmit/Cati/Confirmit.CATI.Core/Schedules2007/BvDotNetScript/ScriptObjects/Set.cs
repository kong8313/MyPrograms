using System.Collections.Generic;
using System.Linq;

namespace BvDotNetScript.ScriptObjects
{
    public class Set
    {
        private HashSet<int> _set;

        public Set(int[] array)
        {
            _set = new HashSet<int>(array);
        }

        public int[] ToArray()
        {
            return _set.ToArray();
        }

        public void UnionWith(Set other)
        {
            _set.UnionWith(other._set);
        }

        public void ExceptWith(Set other)
        {
            _set.ExceptWith(other._set);
        }
    }
}
