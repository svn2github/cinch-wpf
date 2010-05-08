using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CinchCodeGen
{
    #region TupleHelper

    public static class TupleHelper
    {
        //Allows Tuple.New(1, "2") instead of new Tuple<int, string>(1, "2")
        public static Tuple<T1> New<T1>(T1 t1)
        {
            return new Tuple<T1>(t1);
        }

        //Allows Tuple.New(1, "2") instead of new Tuple<int, string>(1, "2")
        public static Tuple<T1, T2> New<T1, T2>(T1 t1, T2 t2)
        {
            return new Tuple<T1, T2>(t1, t2);
        }

        //Allows Tuple.New(1, "2") instead of new Tuple<int, string>(1, "2")
        public static Tuple<T1, T2, T3> New<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new Tuple<T1, T2, T3>(t1, t2, t3);
        }

        //Allows Tuple.New(1, "2") instead of new Tuple<int, string>(1, "2")
        public static Tuple<T1, T2, T3, T4> New<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4);
        }
    }

    #endregion

    #region Tuples

    #region Tuple<T>
    public class Tuple<T>
    {
        public Tuple(T first)
        {
            First = first;
        }

        public T First { get; set; }
    }


    #endregion

    #region Tuple<T, T2>
    public class Tuple<T, T2> : Tuple<T>
    {
        public Tuple(T first, T2 second)
            : base(first)
        {
            Second = second;
        }

        public T2 Second { get; set; }
    }


    #endregion

    #region Tuple<T, T2, T3>
    public class Tuple<T, T2, T3> : Tuple<T, T2>
    {
        public Tuple(T first, T2 second, T3 third)
            : base(first, second)
        {
            Third = third;
        }

        public T3 Third { get; set; }


    }
    #endregion

    #region Tuple<T, T2, T3, T4>
    public class Tuple<T, T2, T3, T4> : Tuple<T, T2, T3>
    {
        public Tuple(T first, T2 second, T3 third, T4 fourth)
            : base(first, second, third)
        {
            Fourth = fourth;
        }

        public T4 Fourth { get; set; }
    }


    #endregion

    #endregion

}
