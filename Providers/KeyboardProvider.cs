using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace KeyTap.Providers
{
    public sealed class KeyboardProvider : IKeyTapProvider
    {
        #region Const Data

        public string Name { get; } = "Keyboard";

        #endregion

        #region Core Data

        private KeyboardHook _keyboardHook;
        private KeyTapManager _manager;

        #endregion

        #region Constructors

        public KeyboardProvider(KeyTapManager manager)
        {
            _manager = manager;
            _keyboardHook = new KeyboardHook(KeyFunc);
        }

        #endregion

        #region Core

        private bool KeyFunc(bool isDown, Keys key)
        {
            TapKey tapKey = new TapKey(this, id: key.ToString(), name: key.ToString());
            if (_manager.ListenState == KeyTapListenState.Off) return false;
            if (_manager.ListenState == KeyTapListenState.ListOnly &&
                !_manager.KeyList.Contains(tapKey))
                return false;
            if (isDown) KeyDown?.Invoke(this, tapKey);
            else KeyUp?.Invoke(this, tapKey);
            return true;
        }

        #endregion

        #region Key Events

        public event EventHandler<TapKey> KeyDown;

        public event EventHandler<TapKey> KeyUp;

        #endregion

        #region Dispose

        public void Dispose()
        {
            _keyboardHook.Dispose();
        }

        #endregion
    }

    #region Keyboard Hook

    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyboardHookStruct
    {
        public int vkCode;

        public int scanCode;

        public int flags;

        public int time;

        public int dwExtraInfo;
    }

    internal class KeyboardHook : IDisposable
    {
        #region Const Data

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;//KEYDOWN
        private const int WM_KEYUP = 0x101;//KEYUP
        private const int WM_SYSKEYDOWN = 0x104;//SYSKEYDOWN
        private const int WM_SYSKEYUP = 0x105;//SYSKEYUP

        #endregion

        #region Core Data

        private static int _hKeyboardHook = 0;
        private delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        private HookProc _keyboardHookProcedure;

        #endregion

        #region Delegate

        private Func<bool, Keys, bool> _keyFunc;

        #endregion

        #region Constructors

        public KeyboardHook(Func<bool, Keys, bool> keyFunc)
        {
            _keyFunc = keyFunc;
        }

        public void Start()
        {
            if (_hKeyboardHook == 0)
            {
                _keyboardHookProcedure = new HookProc(KeyboardHookProc);
                _hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardHookProcedure,
                    GetModuleHandle(Process.GetCurrentProcess().MainModule?.ModuleName), 0);
                if (_hKeyboardHook == 0)
                {
                    Stop();
                    throw new Exception("Failed to install keyboard hook.");
                }
            }
        }

        public void Stop()
        {
            bool retKeyboard = true;


            if (_hKeyboardHook != 0)
            {
                retKeyboard = UnhookWindowsHookEx(_hKeyboardHook);
                _hKeyboardHook = 0;
            }

            if (!(retKeyboard)) throw new Exception("Failed to remove keyboard hook.");
        }

        #endregion

        #region Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode >= 0 && !(_keyFunc is null))
            {
                KeyboardHookStruct hook = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                    return _keyFunc(true, (Keys) hook.vkCode) ? 1 : 0;
                if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
                    return _keyFunc(true, (Keys)hook.vkCode) ? 1 : 0;
            }

            return 0;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Stop();
        }

        #endregion

    }

    #endregion
}
