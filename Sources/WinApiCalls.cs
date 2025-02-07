/*
 * Created by SharpDevelop.
 * User: Tebjan Halm
 * Date: 21.01.2014
 * Time: 16:55
 * 
 * Modified by PoteznaSowa
 */
using System;
using System.Runtime.InteropServices;
//using System.Security;

namespace TimerTool {
    /// <summary>
    /// Description of WinApiCalls.
    /// </summary>
    public static class WinApiCalls {

        [DllImport("NTDLL.DLL")]
        public static extern NtStatus NtQueryTimerResolution(out int MinimumResolution, out int MaximumResolution, out int ActualResolution);

        [DllImport("NTDLL.DLL")]
        public static extern NtStatus NtSetTimerResolution(int DesiredResolution, bool SetResolution, out int CurrentResolution);
    }
}
