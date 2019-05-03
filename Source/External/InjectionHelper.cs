using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PrisonLabor.External
{
    /// <summary>
    /// Author of this class: Jerry.Wang from CodeProject
    /// Link to source: https://www.codeproject.com/Articles/463508/NET-CLR-Injection-Modify-IL-Code-during-Run-time
    /// </summary>
    public static class InjectionHelper
    {
        #region P/Invoke
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibraryW(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        #endregion


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool UpdateILCodesDelegate(IntPtr pMethodTable, IntPtr pMethodHandle, int md, IntPtr pBuffer, int dwSize, int nMaxStack);


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool IsInitializedDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool IsInjectionSucceededDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool WaitForIntializationCompletionDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void GetErrorMessageDelegate([MarshalAs(UnmanagedType.LPStr)] StringBuilder szError
            , [MarshalAs(UnmanagedType.U4)] int nCapcity
            );


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void CollectVersionDelegate([MarshalAs(UnmanagedType.LPStr)] string lpszDllFile);

        private static IntPtr _moduleHandle;
        private static GetErrorMessageDelegate _getErrorMessage;
        private static UpdateILCodesDelegate _updateILCodesMethod;
        private static IsInitializedDelegate _isInitializedDelegate;
        private static IsInjectionSucceededDelegate _isInjectionSucceededDelegate;
        private static WaitForIntializationCompletionDelegate _waitForIntializationCompletionDelegate;

        private static CollectVersionDelegate _collectVersionDelegate;

        public static void Initialize()
        {
            string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            currentDir = Regex.Replace(currentDir, @"^(file\:\\)", string.Empty);

            // Environment.Is64BitProcess
            string path = Path.Combine(currentDir, (IntPtr.Size == 8) ? "Injection64.dll" : "Injection32.dll");

            _moduleHandle = LoadLibraryW(path);
            if (_moduleHandle == IntPtr.Zero)
                throw new FileNotFoundException(string.Format("Failed to load [{0}]", path));


            IntPtr ptr = GetProcAddress(_moduleHandle, "UpdateILCodes");
            if (ptr == IntPtr.Zero)
                throw new MethodAccessException("Failed to locate UpdateILCodes function!");
            _updateILCodesMethod = (UpdateILCodesDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(UpdateILCodesDelegate));

            ptr = GetProcAddress(_moduleHandle, "GetErrorMessage");
            if (ptr == IntPtr.Zero)
                throw new MethodAccessException("Failed to locate GetErrorMessage function!");
            _getErrorMessage = (GetErrorMessageDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(GetErrorMessageDelegate));


            ptr = GetProcAddress(_moduleHandle, "IsInitialized");
            if (ptr == IntPtr.Zero)
                throw new MethodAccessException("Failed to locate IsInitialized function!");
            _isInitializedDelegate = (IsInitializedDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(IsInitializedDelegate));

            ptr = GetProcAddress(_moduleHandle, "IsInjectionSucceeded");
            if (ptr == IntPtr.Zero)
                throw new MethodAccessException("Failed to locate IsInjectionSucceeded function!");
            _isInjectionSucceededDelegate = (IsInjectionSucceededDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(IsInjectionSucceededDelegate));

            ptr = GetProcAddress(_moduleHandle, "WaitForIntializationCompletion");
            if (ptr == IntPtr.Zero)
                throw new MethodAccessException("Failed to locate WaitForIntializationCompletion function!");
            _waitForIntializationCompletionDelegate = (WaitForIntializationCompletionDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(WaitForIntializationCompletionDelegate));

            ptr = GetProcAddress(_moduleHandle, "__CollectAddress");
            if (ptr == IntPtr.Zero)
                throw new MethodAccessException("Failed to locate __CollectAddress function!");
            _collectVersionDelegate = (CollectVersionDelegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CollectVersionDelegate));

        }

        public static void Uninitialize()
        {
            if (_moduleHandle != IntPtr.Zero)
            {
                FreeLibrary(_moduleHandle);
                _moduleHandle = IntPtr.Zero;
            }
        }

        public static void UpdateILCodes(MethodInfo method, byte[] ilCodes, int nMaxStack = -1)
        {
            if (_updateILCodesMethod == null)
                throw new Exception("Please Initialize() first.");

            IntPtr pMethodTable = IntPtr.Zero;
            if (method.DeclaringType != null)
                pMethodTable = method.DeclaringType.TypeHandle.Value;

            IntPtr pMethodHandle = IntPtr.Zero;
            if (method is DynamicMethod)
            {
                pMethodHandle = GetDynamicMethodHandle(method);
            }
            else
            {
                pMethodHandle = method.MethodHandle.Value;
            }

            IntPtr pBuffer = Marshal.AllocHGlobal(ilCodes.Length);
            if (pBuffer == IntPtr.Zero)
                throw new OutOfMemoryException();

            Marshal.Copy(ilCodes, 0, pBuffer, ilCodes.Length);

            int token = 0;
            try
            {
                token = method.MetadataToken;
            }
            catch
            {
            }


            if (!_updateILCodesMethod(pMethodTable, pMethodHandle, token, pBuffer, ilCodes.Length, nMaxStack))
                throw new Exception("UpdateILCodes() failed, please check the initialization is failed or uncompleted.");
        }

        /// <summary>
        /// This method will block current calling thread until the initialization is completed.
        /// If there is something wrong during the initialization
        /// An exception will be throw out.
        /// </summary>
        public static void WaitForIntializationCompletion()
        {
            bool success = _waitForIntializationCompletionDelegate();
            if (!success)
                throw new Exception(GetErrorMessage());
        }

        /// <summary>
        /// Return a boolean value indicates if the initialization is completed
        /// </summary>
        /// <returns></returns>
        public static bool IsInitialized()
        {
            return _isInitializedDelegate();
        }

        /// <summary>
        /// Return a boolean value indicates if the injection is done successfully
        /// </summary>
        /// <returns></returns>
        public static bool IsInjectionSucceeded()
        {
            return _isInjectionSucceededDelegate();
        }

        /// <summary>
        ///  this method is only intended to be used by sample application to collect installed .NET versions on the machine to improve the cache data.
        /// </summary>
        /// <param name="dllFile"></param>
        public static void CollectVersion(string dllFile)
        {
            _collectVersionDelegate(dllFile);
        }

        public static string GetErrorMessage()
        {
            StringBuilder sb = new StringBuilder(4096);
            _getErrorMessage(sb, sb.Capacity);
            return sb.ToString();
        }

        private static IntPtr GetDynamicMethodHandle(MethodBase method)
        {
            // .Net 4.0
            {
                FieldInfo fieldInfo = typeof(DynamicMethod).GetField("m_methodHandle", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    object runtimeMethodInfoStub = fieldInfo.GetValue(method);
                    if (runtimeMethodInfoStub != null)
                    {
                        fieldInfo = runtimeMethodInfoStub.GetType().GetField("m_value", BindingFlags.Instance | BindingFlags.Public);
                        if (fieldInfo != null)
                        {
                            object internalRuntimeMethodHandle = fieldInfo.GetValue(runtimeMethodInfoStub);
                            if (internalRuntimeMethodHandle != null)
                            {
                                fieldInfo = internalRuntimeMethodHandle.GetType().GetField("m_handle", BindingFlags.NonPublic | BindingFlags.Instance);
                                if (fieldInfo != null)
                                {
                                    return (IntPtr)fieldInfo.GetValue(internalRuntimeMethodHandle);
                                }
                            }
                        }
                    }
                }
            }

            // .Net 2.0
            {
                FieldInfo fieldInfo = typeof(DynamicMethod).GetField("m_method", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    return ((RuntimeMethodHandle)fieldInfo.GetValue(method)).Value;
                }
            }
            return IntPtr.Zero;
        }
    }
}

