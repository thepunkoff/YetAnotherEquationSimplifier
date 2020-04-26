using Yet_Another_Simplifier.Tokens;

namespace Yet_Another_Simplifier.Interfaces
{
    public interface IEliminatable
    {
        ValueToken Eliminate(decimal value);
    }
}
