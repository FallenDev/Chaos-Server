// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;

namespace ChaosLauncher
{
    [Flags]
    internal enum ProcessAccess
    {
        None = 0,
        Terminate = 1,
        CreateThread = 2,
        VmOperation = 8,
        VmRead = 16,
        VmWrite = 32,
        DuplicateHandle = 64,
        CreateProcess = 128,
        SetQuota = 256,
        SetInformation = 512,
        QueryInformation = 1024,
        SuspendResume = 2048,
        QueryLimitedInformation = 4096,
        All = 0x1F0FFF
    }
    [Flags]
    internal enum ProcessCreationFlags
    {
        DebugProcess = 1,
        DebugOnlyThisProcess = 2,
        Suspended = 4,
        DetachedProcess = 8,
        NewConsole = 16,
        NewProcessGroup = 512,
        UnicodeEnvironment = 1024,
        SeparateWowVdm = 2048,
        SharedWowVdm = 4096,
        InheritParentAffinity = SharedWowVdm,
        ProtectedProcess = 262144,
        ExtendedStartupInfoPresent = 524288,
        BreakawayFromJob = 16777216,
        PreserveCodeAuthZLevel = 33554432,
        DefaultErrorMode = 67108864,
        NoWindow = 134217728,
    }
}
