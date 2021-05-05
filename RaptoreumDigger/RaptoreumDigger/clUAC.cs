using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace RaptoreumDigger
{
    public class UAC
    {
        [StructLayout(LayoutKind.Sequential)]
        struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LookupPrivilegeValue(string lpSystemName, string lpName,
            out LUID lpLuid);

        const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;

        [StructLayout(LayoutKind.Sequential)]
        struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public UInt32 Attributes;
        }
        const Int32 ANYSIZE_ARRAY = 1;

        struct TOKEN_PRIVILEGES
        {
            public UInt32 PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ANYSIZE_ARRAY)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
           [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
           ref TOKEN_PRIVILEGES NewState,
           UInt32 Zero,
           IntPtr Null1,
           IntPtr Null2);

        [DllImport("user32.dll")]
        static extern IntPtr GetShellWindow();

        const string SE_INCREASE_QUOTA_NAME = "SeLockMemoryPrivilege";
        const int ERROR_NOT_ALL_ASSIGNED = 1300;

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [Flags()]
        enum ProcessAccessFlags : int
        {
            /// Specifies all possible access flags for the process object.
            AllAccess = CreateThread | DuplicateHandle | QueryInformation | SetInformation
                | Terminate | VMOperation | VMRead | VMWrite | Synchronize,
            /// Enables usage of the process handle in the CreateRemoteThread 
            /// function to create a thread in the process.
            CreateThread = 0x2,
            /// Enables usage of the process handle as either the source or target process 
            /// in the DuplicateHandle function to duplicate a handle.
            DuplicateHandle = 0x40,
            /// Enables usage of the process handle in the GetExitCodeProcess and 
            /// GetPriorityClass functions to read information from the process object.
            QueryInformation = 0x400,
            /// Enables usage of the process handle in the SetPriorityClass function to 
            /// set the priority class of the process.
            SetInformation = 0x200,
            /// Enables usage of the process handle in the TerminateProcess function to 
            /// terminate the process.
            Terminate = 0x1,
            /// Enables usage of the process handle in the VirtualProtectEx and 
            /// WriteProcessMemory functions to modify the virtual memory of the process.
            VMOperation = 0x8,
            /// Enables usage of the process handle in the ReadProcessMemory function to' 
            /// read from the virtual memory of the process.
            VMRead = 0x10,
            /// Enables usage of the process handle in the WriteProcessMemory function to 
            /// write to the virtual memory of the process.
            VMWrite = 0x20,
            /// Enables usage of the process handle in any of the wait functions to wait 
            /// for the process to terminate.
            Synchronize = 0x100000
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)]
            bool bInheritHandle, uint dwProcessId);

        [Flags()]
        enum TokenAccessFlags : int
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            STANDARD_RIGHTS_READ = 0x00020000,
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY),
            TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
                TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
                TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
                TOKEN_ADJUST_SESSIONID)
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle,
            TokenAccessFlags DesiredAccess, out IntPtr TokenHandle);

        enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static bool DuplicateTokenEx(
            IntPtr hExistingToken,
            TokenAccessFlags dwDesiredAccess,
            IntPtr lpThreadAttributes,
            SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
            TOKEN_TYPE TokenType,
            out IntPtr phNewToken);

        [Flags]
        enum CreationFlags
        {
            CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
            CREATE_NEW_CONSOLE = 0x00000010,
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            CREATE_NO_WINDOW = 0x08000000,
            CREATE_PROTECTED_PROCESS = 0x00040000,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
            CREATE_SEPARATE_WOW_VDM = 0x00001000,
            CREATE_SUSPENDED = 0x00000004,
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            DEBUG_ONLY_THIS_PROCESS = 0x00000002,
            DEBUG_PROCESS = 0x00000001,
            DETACHED_PROCESS = 0x00000008,
            EXTENDED_STARTUPINFO_PRESENT = 0x00080000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        extern static bool CloseHandle(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcessAsUserW(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            CreationFlags dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);


        public static void SetPrivileges()
        {
            IntPtr currentToken;
            int lastError;

            if (!OpenProcessToken(System.Diagnostics.Process.GetCurrentProcess().Handle,
                TokenAccessFlags.TOKEN_ADJUST_PRIVILEGES, out currentToken))
            {
                lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError);
            }

            LUID myLUID;

            if (!LookupPrivilegeValue(null, SE_INCREASE_QUOTA_NAME, out myLUID))
            {
                lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError);
            }

            TOKEN_PRIVILEGES myTokenPrivileges;

            myTokenPrivileges.PrivilegeCount = 1;
            myTokenPrivileges.Privileges = new LUID_AND_ATTRIBUTES[1];
            myTokenPrivileges.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
            myTokenPrivileges.Privileges[0].Luid = myLUID;

            if (!AdjustTokenPrivileges(currentToken, false, ref myTokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero))
            {
                lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError);
            }

        }
    }
}
