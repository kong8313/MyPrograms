using System;
using Confirmit.CATI.Core.ScheduleDom.Script;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ScheduleDom.Script.Fakes
{
    public class StubIActionCollection : IActionCollection 
    {
        private IActionCollection _inner;

        public StubIActionCollection()
        {
            _inner = null;
        }

        public IActionCollection Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddActionDelegate(Action item);
        public AddActionDelegate AddAction;

        void IActionCollection.Add(Action item)
        {

            if (AddAction != null)
            {
                AddAction(item);
            } else if (_inner != null)
            {
                ((IActionCollection)_inner).Add(item);
            }
        }

        public delegate void ClearDelegate();
        public ClearDelegate Clear;

        void IActionCollection.Clear()
        {

            if (Clear != null)
            {
                Clear();
            } else if (_inner != null)
            {
                ((IActionCollection)_inner).Clear();
            }
        }

        public delegate bool ContainsActionDelegate(Action item);
        public ContainsActionDelegate ContainsAction;

        bool IActionCollection.Contains(Action item)
        {


            if (ContainsAction != null)
            {
                return ContainsAction(item);
            } else if (_inner != null)
            {
                return ((IActionCollection)_inner).Contains(item);
            }

            return default(bool);
        }

        public delegate void CopyToArrayOfActionInt32Delegate(Action[] array, int arrayIndex);
        public CopyToArrayOfActionInt32Delegate CopyToArrayOfActionInt32;

        void IActionCollection.CopyTo(Action[] array, int arrayIndex)
        {

            if (CopyToArrayOfActionInt32 != null)
            {
                CopyToArrayOfActionInt32(array, arrayIndex);
            } else if (_inner != null)
            {
                ((IActionCollection)_inner).CopyTo(array, arrayIndex);
            }
        }

        public delegate bool RemoveActionDelegate(Action item);
        public RemoveActionDelegate RemoveAction;

        bool IActionCollection.Remove(Action item)
        {


            if (RemoveAction != null)
            {
                return RemoveAction(item);
            } else if (_inner != null)
            {
                return ((IActionCollection)_inner).Remove(item);
            }

            return default(bool);
        }

        public delegate IEnumerator<Action> GetEnumeratorDelegate();
        public GetEnumeratorDelegate GetEnumerator;

        IEnumerator<Action> IActionCollection.GetEnumerator()
        {


            if (GetEnumerator != null)
            {
                return GetEnumerator();
            } else if (_inner != null)
            {
                return ((IActionCollection)_inner).GetEnumerator();
            }

            return default(IEnumerator<Action>);
        }

        public delegate Action GetActionByIdInt32Delegate(int actionId);
        public GetActionByIdInt32Delegate GetActionByIdInt32;

        Action IActionCollection.GetActionById(int actionId)
        {


            if (GetActionByIdInt32 != null)
            {
                return GetActionByIdInt32(actionId);
            } else if (_inner != null)
            {
                return ((IActionCollection)_inner).GetActionById(actionId);
            }

            return default(Action);
        }

        private int _Count;
        public Func<int> CountGet;
        public Action<int> CountSetInt32;

        int IActionCollection.Count
        {
            get
            {
                if (CountGet != null)
                {
                    return CountGet();
                } else if (_inner != null)
                {
                    return ((IActionCollection)_inner).Count;
                }

                if (CountSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Count;
                }

                return default(int);
            }

        }

        private bool _IsReadOnly;
        public Func<bool> IsReadOnlyGet;
        public Action<bool> IsReadOnlySetBoolean;

        bool IActionCollection.IsReadOnly
        {
            get
            {
                if (IsReadOnlyGet != null)
                {
                    return IsReadOnlyGet();
                } else if (_inner != null)
                {
                    return ((IActionCollection)_inner).IsReadOnly;
                }

                if (IsReadOnlySetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsReadOnly;
                }

                return default(bool);
            }

        }

        private Action[] _Actions;
        public Func<Action[]> ActionsGet;
        public Action<Action[]> ActionsSetArrayOfAction;

        Action[] IActionCollection.Actions
        {
            get
            {
                if (ActionsGet != null)
                {
                    return ActionsGet();
                } else if (_inner != null)
                {
                    return ((IActionCollection)_inner).Actions;
                }

                if (ActionsSetArrayOfAction == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Actions;
                }

                return default(Action[]);
            }

            set
            {
                if (ActionsSetArrayOfAction != null)
                {
                    ActionsSetArrayOfAction(value);
                    return;
                } else if (_inner != null)
                {
                    ((IActionCollection)_inner).Actions = value;
                    return;
                }

                if (ActionsGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Actions = value;
                }

            }
        }

    }
}