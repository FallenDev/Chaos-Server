namespace Chaos.Common.Abstractions;

/// <summary>
///     An object that is bound to a base directory
/// </summary>
public interface IDirectoryBound
{
    /// <summary>
    ///     Instructs the object to use the specified directory as its base directory
    /// </summary>
    void UseBaseDirectory(string baseDirectory);
}