namespace Picofon.Utils
{
    public delegate void ActionIn<T>(in T arg)
        where T : struct;
}
