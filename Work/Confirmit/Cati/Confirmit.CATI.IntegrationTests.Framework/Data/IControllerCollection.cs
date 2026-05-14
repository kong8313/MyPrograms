using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Data
{
    public interface IAssert<TModel>
    {
        IAsserter<TModel> Assert { get; }
    }

    public interface IAsserter<TModel>
    {
        IAsserter<TModel> IsTrue(Func<TModel, bool> checker, string message = null);
        IAsserter<TModel> AreEqual<T>(T expected, Func<TModel, T> getter, string message = null);
        void IsNull();
    }


    public interface IArrayController<TController, TModel> : IAssert<TModel>, IEnumerable<TController>
    {
    }

    public class SingleAsserter<TModel> : IAsserter<TModel>
    {
        private Func<TModel> _getter; 
        
        public SingleAsserter(TModel model)
        {
            _getter = () => model;
        }

        public SingleAsserter(Func<TModel> getter)
        {
            _getter = getter;
        }

        public SingleAsserter(IModelProvider<TModel> provider)
        {
            _getter = () => provider.Model;
        }


        public IAsserter<TModel> IsTrue(Func<TModel, bool> checker, string message = null)
        {
            if(message != null)
                Assert.IsTrue(checker(_getter()), message);
            else
                Assert.IsTrue(checker(_getter()));

            return this;
        }

        public IAsserter<TModel> AreEqual<T>(T expected, Func<TModel, T> getter, string message = null)
        {
            if (message != null)
                Assert.AreEqual(expected, getter(_getter()), message);
            else
                Assert.AreEqual(expected, getter(_getter()));

            return this;
        }

        public void IsNull()
        {
            Assert.IsNull(_getter());
        }
    }

    public class ManyAsserter<TModel> : IAsserter<TModel>
    {
        private readonly List<IAsserter<TModel>> _asserters;

        public ManyAsserter(IEnumerable<IAsserter<TModel>> asserters)
        {
            _asserters = asserters.ToList();
        }

        public IAsserter<TModel> IsTrue(Func<TModel, bool> checker, string message = null)
        {
            _asserters.ForEach( x => x.IsTrue(checker, message));
            return this;
        }

        public IAsserter<TModel> AreEqual<T>(T expected, Func<TModel, T> getter, string message = null)
        {
            _asserters.ForEach(x => x.AreEqual(expected, getter, message));
            return this;
        }

        public void IsNull()
        {
            _asserters.ForEach(x => x.IsNull());
        }
    }

    public class CallAsserter : SingleAsserter<BvCallEntity>
    {
        public CallAsserter(BvCallEntity model) : base(model) { }

        public CallAsserter(Func<BvCallEntity> getter) : base(getter) { }

        public CallAsserter(IModelProvider<BvCallEntity> provider) : base(provider) { }

        public void CallState(CallState expectedState)
        {
            AreEqual(expectedState, x => (CallState) x.CallState);
        }
    }
}
