#region
using Chaos.Common.CustomTypes;
#endregion

namespace Chaos.Definitions;

[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible")]
public class BigFlagsTest : BigFlags<BigFlagsTest>
{
    public static readonly BigFlagsValue<BigFlagsTest> Test1;
    public static readonly BigFlagsValue<BigFlagsTest> Test2;
    public static readonly BigFlagsValue<BigFlagsTest> Test3;
    public static readonly BigFlagsValue<BigFlagsTest> Test4;
    public static readonly BigFlagsValue<BigFlagsTest> Test5;

    static BigFlagsTest() => Initialize();
}