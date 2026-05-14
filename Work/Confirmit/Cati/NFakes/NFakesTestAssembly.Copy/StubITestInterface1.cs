using System;
using NFakesTestAssembly;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface1<T1, T2, T3, ZZZ> : ITestInterface1<T1, T2, T3, ZZZ> 
    {
        private ITestInterface1<T1, T2, T3, ZZZ> _inner;

        public StubITestInterface1()
        {
            _inner = null;
        }

        public ITestInterface1<T1, T2, T3, ZZZ> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate T1 Method27Delegate();
        public Method27Delegate Method27;

        T1 ITestInterface1<T1, T2, T3, ZZZ>.Method27()
        {


            if (Method27 != null)
            {
                return Method27();
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method27();
            }

            return default(T1);
        }

        public delegate void Method26ListOfIEnumerableOfT1OutDelegate(out List<IEnumerable<T1>> t1);
        public Method26ListOfIEnumerableOfT1OutDelegate Method26ListOfIEnumerableOfT1Out;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method26(out List<IEnumerable<T1>> t1)
        {
            t1 = default(List<IEnumerable<T1>>);

            if (Method26ListOfIEnumerableOfT1Out != null)
            {
                Method26ListOfIEnumerableOfT1Out(out t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method26(out t1);
            }
        }

        public delegate List<IEnumerable<T1>> Method25Delegate();
        public Method25Delegate Method25;

        List<IEnumerable<T1>> ITestInterface1<T1, T2, T3, ZZZ>.Method25()
        {


            if (Method25 != null)
            {
                return Method25();
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method25();
            }

            return default(List<IEnumerable<T1>>);
        }

        public delegate void Method24IEnumerableOfKeyValuePairOfT1T1Delegate(IEnumerable<KeyValuePair<T1, T1>> t1);
        public Method24IEnumerableOfKeyValuePairOfT1T1Delegate Method24IEnumerableOfKeyValuePairOfT1T1;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method24(IEnumerable<KeyValuePair<T1, T1>> t1)
        {

            if (Method24IEnumerableOfKeyValuePairOfT1T1 != null)
            {
                Method24IEnumerableOfKeyValuePairOfT1T1(t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method24(t1);
            }
        }

        public delegate void Method23IEnumerableOfT1Delegate(IEnumerable<T1> t1);
        public Method23IEnumerableOfT1Delegate Method23IEnumerableOfT1;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method23(IEnumerable<T1> t1)
        {

            if (Method23IEnumerableOfT1 != null)
            {
                Method23IEnumerableOfT1(t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method23(t1);
            }
        }

        public delegate void Method22DateTimeArrayDelegate(DateTime[] t1);
        public Method22DateTimeArrayDelegate Method22DateTimeArray;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method22(DateTime[] t1)
        {

            if (Method22DateTimeArray != null)
            {
                Method22DateTimeArray(t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method22(t1);
            }
        }

        public delegate void Method18NullableOfInt32Delegate(int? t1);
        public Method18NullableOfInt32Delegate Method18NullableOfInt32;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method18(int? t1)
        {

            if (Method18NullableOfInt32 != null)
            {
                Method18NullableOfInt32(t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method18(t1);
            }
        }

        public delegate void Method17NullableOfDateTimeDelegate(DateTime? t1);
        public Method17NullableOfDateTimeDelegate Method17NullableOfDateTime;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method17(DateTime? t1)
        {

            if (Method17NullableOfDateTime != null)
            {
                Method17NullableOfDateTime(t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method17(t1);
            }
        }

        public delegate void Method16Int32ArrayOutDelegate(out int[] t1);
        public Method16Int32ArrayOutDelegate Method16Int32ArrayOut;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method16(out int[] t1)
        {
            t1 = default(int[]);

            if (Method16Int32ArrayOut != null)
            {
                Method16Int32ArrayOut(out t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method16(out t1);
            }
        }

        public delegate void Method15Int32ArrayDelegate(int[] t1);
        public Method15Int32ArrayDelegate Method15Int32Array;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method15(int[] t1)
        {

            if (Method15Int32Array != null)
            {
                Method15Int32Array(t1);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method15(t1);
            }
        }

        public delegate int[] Method14Delegate();
        public Method14Delegate Method14;

        int[] ITestInterface1<T1, T2, T3, ZZZ>.Method14()
        {


            if (Method14 != null)
            {
                return Method14();
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method14();
            }

            return default(int[]);
        }

        public delegate void Method13T2RefDelegate(ref T2 t);
        public Method13T2RefDelegate Method13T2Ref;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method13(ref T2 t)
        {

            if (Method13T2Ref != null)
            {
                Method13T2Ref(ref t);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method13(ref t);
            }
        }

        public delegate void Method12T1OutDelegate(out T1 t);
        public Method12T1OutDelegate Method12T1Out;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method12(out T1 t)
        {
            t = default(T1);

            if (Method12T1Out != null)
            {
                Method12T1Out(out t);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method12(out t);
            }
        }

        public delegate T1 Method11T2Int32T3ZZZT2Int32T3Delegate(T2 t2, int p1, T3 t3, ZZZ zzz, T2 xxx1, int p2, T3 xxx2);
        public Method11T2Int32T3ZZZT2Int32T3Delegate Method11T2Int32T3ZZZT2Int32T3;

        T1 ITestInterface1<T1, T2, T3, ZZZ>.Method11(T2 t2, int p1, T3 t3, ZZZ zzz, T2 xxx1, int p2, T3 xxx2)
        {


            if (Method11T2Int32T3ZZZT2Int32T3 != null)
            {
                return Method11T2Int32T3ZZZT2Int32T3(t2, p1, t3, zzz, xxx1, p2, xxx2);
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method11(t2, p1, t3, zzz, xxx1, p2, xxx2);
            }

            return default(T1);
        }

        P1 ITestInterface1<T1, T2, T3, ZZZ>.Method10<P1, P2>(P2 p2)
        {


            return default(P1);
        }

        public delegate void Method9TestStructTestStructOutTestStructRefDelegate(TestStruct p1, out TestStruct p2, ref TestStruct p3);
        public Method9TestStructTestStructOutTestStructRefDelegate Method9TestStructTestStructOutTestStructRef;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method9(TestStruct p1, out TestStruct p2, ref TestStruct p3)
        {
            p2 = default(TestStruct);

            if (Method9TestStructTestStructOutTestStructRef != null)
            {
                Method9TestStructTestStructOutTestStructRef(p1, out p2, ref p3);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method9(p1, out p2, ref p3);
            }
        }

        public delegate void Method8TestClassTestClassOutTestClassRefDelegate(TestClass p1, out TestClass p2, ref TestClass p3);
        public Method8TestClassTestClassOutTestClassRefDelegate Method8TestClassTestClassOutTestClassRef;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method8(TestClass p1, out TestClass p2, ref TestClass p3)
        {
            p2 = default(TestClass);

            if (Method8TestClassTestClassOutTestClassRef != null)
            {
                Method8TestClassTestClassOutTestClassRef(p1, out p2, ref p3);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method8(p1, out p2, ref p3);
            }
        }

        public delegate void Method7StringStringOutStringRefDelegate(string p1, out string p2, ref string p3);
        public Method7StringStringOutStringRefDelegate Method7StringStringOutStringRef;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method7(string p1, out string p2, ref string p3)
        {
            p2 = default(string);

            if (Method7StringStringOutStringRef != null)
            {
                Method7StringStringOutStringRef(p1, out p2, ref p3);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method7(p1, out p2, ref p3);
            }
        }

        public delegate void Method6Int32Int32Int32RefDelegate(int p1, int p2, ref int p3);
        public Method6Int32Int32Int32RefDelegate Method6Int32Int32Int32Ref;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method6(int p1, int p2, ref int p3)
        {

            if (Method6Int32Int32Int32Ref != null)
            {
                Method6Int32Int32Int32Ref(p1, p2, ref p3);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method6(p1, p2, ref p3);
            }
        }

        public delegate int Method5Int32Int32OutInt32RefDelegate(int p1, out int p2, ref int p3);
        public Method5Int32Int32OutInt32RefDelegate Method5Int32Int32OutInt32Ref;

        int ITestInterface1<T1, T2, T3, ZZZ>.Method5(int p1, out int p2, ref int p3)
        {
            p2 = default(int);


            if (Method5Int32Int32OutInt32Ref != null)
            {
                return Method5Int32Int32OutInt32Ref(p1, out p2, ref p3);
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method5(p1, out p2, ref p3);
            }

            return default(int);
        }

        public delegate void Method4Int32Int32OutDelegate(int p1, out int p2);
        public Method4Int32Int32OutDelegate Method4Int32Int32Out;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method4(int p1, out int p2)
        {
            p2 = default(int);

            if (Method4Int32Int32Out != null)
            {
                Method4Int32Int32Out(p1, out p2);
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method4(p1, out p2);
            }
        }

        public delegate int Method3Int32Delegate(int p1);
        public Method3Int32Delegate Method3Int32;

        int ITestInterface1<T1, T2, T3, ZZZ>.Method3(int p1)
        {


            if (Method3Int32 != null)
            {
                return Method3Int32(p1);
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method3(p1);
            }

            return default(int);
        }

        public delegate int Method2Delegate();
        public Method2Delegate Method2;

        int ITestInterface1<T1, T2, T3, ZZZ>.Method2()
        {


            if (Method2 != null)
            {
                return Method2();
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method2();
            }

            return default(int);
        }

        public delegate void Method1Delegate();
        public Method1Delegate Method1;

        void ITestInterface1<T1, T2, T3, ZZZ>.Method1()
        {

            if (Method1 != null)
            {
                Method1();
            } else if (_inner != null)
            {
                ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method1();
            }
        }

        public delegate T1 Method0T3OutT2T3OutInt32T3ZZZT2Int32T3Delegate(out T3 xxx3, T2 t2, out T3 xxx4, int p1, T3 t3, ZZZ zzz, T2 xxx1, int p2, T3 xxx2);
        public Method0T3OutT2T3OutInt32T3ZZZT2Int32T3Delegate Method0T3OutT2T3OutInt32T3ZZZT2Int32T3;

        T1 ITestInterface1<T1, T2, T3, ZZZ>.Method0(out T3 xxx3, T2 t2, out T3 xxx4, int p1, T3 t3, ZZZ zzz, T2 xxx1, int p2, T3 xxx2)
        {
            xxx3 = default(T3);
            xxx4 = default(T3);


            if (Method0T3OutT2T3OutInt32T3ZZZT2Int32T3 != null)
            {
                return Method0T3OutT2T3OutInt32T3ZZZT2Int32T3(out xxx3, t2, out xxx4, p1, t3, zzz, xxx1, p2, xxx2);
            } else if (_inner != null)
            {
                return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).Method0(out xxx3, t2, out xxx4, p1, t3, zzz, xxx1, p2, xxx2);
            }

            return default(T1);
        }

        private int _P1;
        public Func<int> P1Get;
        public Action<int> P1SetInt32;

        int ITestInterface1<T1, T2, T3, ZZZ>.P1
        {
            get
            {
                if (P1Get != null)
                {
                    return P1Get();
                } else if (_inner != null)
                {
                    return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P1;
                }

                if (P1SetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _P1;
                }

                return default(int);
            }

            set
            {
                if (P1SetInt32 != null)
                {
                    P1SetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P1 = value;
                    return;
                }

                if (P1Get == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _P1 = value;
                }

            }
        }

        private int _P2;
        public Func<int> P2Get;
        public Action<int> P2SetInt32;

        int ITestInterface1<T1, T2, T3, ZZZ>.P2
        {
            get
            {
                if (P2Get != null)
                {
                    return P2Get();
                } else if (_inner != null)
                {
                    return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P2;
                }

                if (P2SetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _P2;
                }

                return default(int);
            }

        }

        private int _P3;
        public Func<int> P3Get;
        public Action<int> P3SetInt32;

        int ITestInterface1<T1, T2, T3, ZZZ>.P3
        {
            set
            {
                if (P3SetInt32 != null)
                {
                    P3SetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P3 = value;
                    return;
                }

                if (P3Get == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _P3 = value;
                }

            }
        }

        private T1 _P4;
        public Func<T1> P4Get;
        public Action<T1> P4SetT1;

        T1 ITestInterface1<T1, T2, T3, ZZZ>.P4
        {
            get
            {
                if (P4Get != null)
                {
                    return P4Get();
                } else if (_inner != null)
                {
                    return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P4;
                }

                if (P4SetT1 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _P4;
                }

                return default(T1);
            }

            set
            {
                if (P4SetT1 != null)
                {
                    P4SetT1(value);
                    return;
                } else if (_inner != null)
                {
                    ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P4 = value;
                    return;
                }

                if (P4Get == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _P4 = value;
                }

            }
        }

        private XmlAttribute _P5;
        public Func<XmlAttribute> P5Get;
        public Action<XmlAttribute> P5SetXmlAttribute;

        XmlAttribute ITestInterface1<T1, T2, T3, ZZZ>.P5
        {
            set
            {
                if (P5SetXmlAttribute != null)
                {
                    P5SetXmlAttribute(value);
                    return;
                } else if (_inner != null)
                {
                    ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P5 = value;
                    return;
                }

                if (P5Get == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _P5 = value;
                }

            }
        }

        private StringBuilder _P6;
        public Func<StringBuilder> P6Get;
        public Action<StringBuilder> P6SetStringBuilder;

        StringBuilder ITestInterface1<T1, T2, T3, ZZZ>.P6
        {
            get
            {
                if (P6Get != null)
                {
                    return P6Get();
                } else if (_inner != null)
                {
                    return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P6;
                }

                if (P6SetStringBuilder == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _P6;
                }

                return default(StringBuilder);
            }

        }

        private string _P7;
        public Func<string> P7Get;
        public Action<string> P7SetString;

        string ITestInterface1<T1, T2, T3, ZZZ>.P7
        {
            get
            {
                if (P7Get != null)
                {
                    return P7Get();
                } else if (_inner != null)
                {
                    return ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P7;
                }

                if (P7SetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _P7;
                }

                return default(string);
            }

            set
            {
                if (P7SetString != null)
                {
                    P7SetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ITestInterface1<T1, T2, T3, ZZZ>)_inner).P7 = value;
                    return;
                }

                if (P7Get == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _P7 = value;
                }

            }
        }

    }
}