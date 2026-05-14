using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace NFakesTestAssembly
{
    public class Parent
    {
        public class Nested
        {
            
        }
    }

    public interface ITestInterface10
    {
        Parent.Nested Foo();
    }

    public interface ITestInterface9
    {
        void Foo(string @event);
    }

    public interface ITestInterface8<T> where T : class
    {
        T Foo1();
    }

    public interface ITestInterface2
    {
        void Foo1();
        void Foo2();
    }

    public interface ITestInterface3
    {
        void Foo2();
        void Foo3();
    }

    public interface ITestInterface4 : ITestInterface2, ITestInterface3
    {
    }

    public interface ITestInterface<T1>
    {
        T1 Foo();
    }

    public interface ITestInterface6<T1, T2>  where T1 : ICloneable, ITestInterface<T1>, new()  where T2 : new()
    {
        void Bar();
    }

    public interface ITestInterface7<T1>  where T1 : struct 
    {
    }

    public class TestClass
    {
    }

    public class TestStruct
    {
    }

    public interface ITestInterface1<T1, T2, T3, ZZZ>
    {
        T1 Method27();
        void Method26(out List<IEnumerable<T1>> t1);
        List<IEnumerable<T1>> Method25();
        void Method24(IEnumerable<KeyValuePair<T1, T1>> t1);
        void Method23(IEnumerable<T1> t1);
        void Method22(DateTime[] t1);
        void Method18(int? t1);
        void Method17(DateTime? t1);
        void Method16(out int[] t1);
        void Method15(int[] t1);
        int[] Method14();
        void Method13(ref T2 t);
        void Method12(out T1 t);
        T1 Method11(T2 t2, int p1, T3 t3, ZZZ zzz, T2 xxx1, int p2, T3 xxx2);
        P1 Method10<P1, P2>(P2 p2);
        void Method9(TestStruct p1, out TestStruct p2, ref TestStruct p3);
        void Method8(TestClass p1, out TestClass p2, ref TestClass p3);
        void Method7(string p1, out string p2, ref string p3);
        void Method6(int p1, int p2, ref int p3);
        int Method5(int p1, out int p2, ref int p3);
        void Method4(int p1, out int p2);
        int Method3(int p1);
        int Method2();
        void Method1();
        T1 Method0(out T3 xxx3, T2 t2, out T3 xxx4, int p1, T3 t3, ZZZ zzz, T2 xxx1, int p2, T3 xxx2);
        int P1 { get; set; }
        int P2 { get; }
        int P3 { set; }
        T1 P4 { get; set; }
        XmlAttribute P5 { set; }
        StringBuilder P6 { get; }
        string P7 { get; set; }
    }

    public interface ITestInterfaceXxx
    {
        
    }
}
