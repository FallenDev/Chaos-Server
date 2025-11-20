#region
using System.Numerics;
#endregion

namespace Chaos.Common.Abstractions;

public interface IBigFlagsValue
{
    Type Type { get; }
    BigInteger Value { get; }
}