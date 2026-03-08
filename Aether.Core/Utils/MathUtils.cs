using System.Numerics;

namespace Aether.Core.Utils;
public static class MathUtils
{
    public static T PercentageChange<T>(T oldValue, T newValue) where T : INumber<T>
    {
        if (oldValue == T.CreateChecked(0))
            return T.CreateChecked(0);

        var result = (newValue - oldValue) / oldValue * T.CreateChecked(100);

        return result;
    }
}
