using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.IntegrationTests.Framework.Data
{
    public class ArrayController<TController, TModel> : IArrayController<TController, TModel> where TController : IAssert<TModel>
    {
        public TController[] Controllers { get; private set; }

        public ArrayController(IEnumerable<TController> controllers)
        {
            Controllers = controllers.ToArray();
            Assert = new ManyAsserter<TModel>(Controllers.Select(x => x.Assert));
        }

        public IAsserter<TModel> Assert { get; private set; }

        public IEnumerator<TController> GetEnumerator()
        {
            return ((IEnumerable<TController>)Controllers).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Controllers.GetEnumerator();
        }
    }
}