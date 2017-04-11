namespace Ceql.Composition
{
    public delegate bool BooleanExpression();
    public delegate bool BooleanExpression<in T>(T from);
    public delegate bool BooleanExpression<in T1, in T2>(T1 from, T2 to);
    public delegate bool BooleanExpression<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
    public delegate bool BooleanExpression<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate bool BooleanExpression<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate bool BooleanExpression<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate bool BooleanExpression<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

    public delegate TResult SelectExpression<out TResult>();
    public delegate TResult SelectExpression<in T, out TResult>(T t);
    public delegate TResult SelectExpression<in T, in T1, out TResult>(T t, T1 t1);
    public delegate TResult SelectExpression<in T1, in T2, in T3, out TResult>(T1 t1, T2 t2, T3 t3);
    public delegate TResult SelectExpression<in T1, in T2, in T3, in T4, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate TResult SelectExpression<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
 }
