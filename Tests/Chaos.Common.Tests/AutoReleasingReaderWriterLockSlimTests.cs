#region
using Chaos.Common.Synchronization;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class AutoReleasingReaderWriterLockSlimTests
{
    private readonly AutoReleasingReaderWriterLockSlim _autoLock;
    private readonly ReaderWriterLockSlim _rootLock = new();

    public AutoReleasingReaderWriterLockSlimTests() => _autoLock = new AutoReleasingReaderWriterLockSlim(_rootLock);

    [Test]
    public void EnterReadLock_ShouldAcquireReadLockAndReleaseOnDispose()
    {
        using (_autoLock.EnterReadLock())
            _rootLock.IsReadLockHeld
                     .Should()
                     .BeTrue();

        _rootLock.IsReadLockHeld
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void EnterUpgradeableReadLock_ShouldAcquireUpgradeableReadLockAndAllowUpgrade()
    {
        using (var upgradeableLock = _autoLock.EnterUpgradeableReadLock())
        {
            _rootLock.IsUpgradeableReadLockHeld
                     .Should()
                     .BeTrue();

            upgradeableLock.UpgradeToWriteLock();

            _rootLock.IsWriteLockHeld
                     .Should()
                     .BeTrue();
        }

        _rootLock.IsUpgradeableReadLockHeld
                 .Should()
                 .BeFalse();

        _rootLock.IsWriteLockHeld
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void EnterWriteLock_ShouldAcquireWriteLockAndReleaseOnDispose()
    {
        using (_autoLock.EnterWriteLock())
            _rootLock.IsWriteLockHeld
                     .Should()
                     .BeTrue();

        _rootLock.IsWriteLockHeld
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void NoOpDisposable_ShouldDoNothingOnDispose()
    {
        var disposable = AutoReleasingReaderWriterLockSlim.NoOpDisposable;
        var disposeAction = () => disposable.Dispose();

        disposeAction.Should()
                     .NotThrow();
    }

    [Test]
    public void TryEnterReadLock_WithTimeout_ShouldSucceedWithinTimeoutAndReleaseOnDispose()
    {
        using (var readLock = _autoLock.TryEnterReadLock(1000))
        {
            readLock.Should()
                    .NotBeNull();

            _rootLock.IsReadLockHeld
                     .Should()
                     .BeTrue();
        }

        _rootLock.IsReadLockHeld
                 .Should()
                 .BeFalse();
    }
}