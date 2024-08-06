namespace ChampionOpenAPI_CSharp
{
// Struct "ValueTuple" is not supported in the version under .NET Framework 4.7.
// To support .NET Framework 4.7, I built a custom struct, whose name is "ValueTuple2".
// So please use "ValueTuple2" instead of "ValueTuple", in this project, if possible.
    public struct ValueTuple2<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public ValueTuple2(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }
    }
    public struct ValueTuple2<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public ValueTuple2(T1 t1, T2 t2, T3 t3)
        {
            this.Item1 = t1;
            this.Item2 = t2;
            this.Item3 = t3;
        }
    }
}
