using System;
using System.Linq;
using System.Runtime.InteropServices;

public class BoltDebugStartSettings
{
#if UNITY_EDITOR
	public static bool DebugStartIsSinglePlayer
	{
		get { return BoltRuntimeSettings.instance.debugEditorMode == BoltEditorStartMode.None; }
	}

	public static bool DebugStartIsServer
	{
		get { return BoltRuntimeSettings.instance.debugEditorMode == BoltEditorStartMode.Server; }
	}

	public static bool DebugStartIsClient
	{
		get { return BoltRuntimeSettings.instance.debugEditorMode == BoltEditorStartMode.Client; }
	}

	public static int WindowIndex
	{
		get { return -1; }
	}
#elif UNITY_STANDALONE
	public static bool DebugStartIsSinglePlayer
	{
		get { return false; }
	}

	public static bool DebugStartIsServer
	{
		get { return Environment.GetCommandLineArgs().Contains("--bolt-debugstart-server"); }
	}

	public static bool DebugStartIsClient
	{
		get { return Environment.GetCommandLineArgs().Contains("--bolt-debugstart-client"); }
	}

	public static int WindowIndex
	{
		get
		{
			foreach (string arg in Environment.GetCommandLineArgs())
			{
				if (arg.StartsWith("--bolt-window-index-"))
				{
					return int.Parse(arg.Replace("--bolt-window-index-", ""));
				}
			}

			return 0;
		}
	}
#else
	public static bool DebugStartIsSinglePlayer
	{
		get { return false; }
	}

	public static bool DebugStartIsServer
	{
		get { return false; }
	}

	public static bool DebugStartIsClient
	{
		get { return false; }
	}

	public static int WindowIndex
	{
		get { return -1; }
	}
#endif

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
	static readonly object handle = new object();
	static HandleRef unityHandle = new HandleRef();

	static class HWND
	{
		public static IntPtr
		  NoTopMost = new IntPtr(-2),
		  TopMost = new IntPtr(-1),
		  Top = new IntPtr(0),
		  Bottom = new IntPtr(1);
	}

	static class SWP
	{
		public static readonly int
		  NOSIZE = 0x0001,
		  NOMOVE = 0x0002,
		  NOZORDER = 0x0004,
		  NOREDRAW = 0x0008,
		  NOACTIVATE = 0x0010,
		  DRAWFRAME = 0x0020,
		  FRAMECHANGED = 0x0020,
		  SHOWWINDOW = 0x0040,
		  HIDEWINDOW = 0x0080,
		  NOCOPYBITS = 0x0100,
		  NOOWNERZORDER = 0x0200,
		  NOREPOSITION = 0x0200,
		  NOSENDCHANGING = 0x0400,
		  DEFERERASE = 0x2000,
		  ASYNCWINDOWPOS = 0x4000;
	}

	delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetSystemMetrics(int index);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

	static bool Window(IntPtr hWnd, IntPtr lParam)
	{
		int pid = -1;
		int unity_pid = System.Diagnostics.Process.GetCurrentProcess().Id;

		GetWindowThreadProcessId(new HandleRef(handle, hWnd), out pid);

		if (pid == unity_pid)
		{
			unityHandle = new HandleRef(handle, hWnd);
			return false;
		}

		return true;
	}

	public static void PositionWindow()
	{
		if (DebugStartIsClient || DebugStartIsServer)
		{
			EnumWindows(Window, IntPtr.Zero);

			if (unityHandle.Wrapper != null)
			{
				int ww = UnityEngine.Screen.width;
				int wh = UnityEngine.Screen.height;

				int x = 0;
				int y = 0;
				int w = GetSystemMetrics(0);
				int h = GetSystemMetrics(1);

				if (DebugStartIsServer)
				{
					x = w / 2 - (ww / 2);
					y = h / 2 - (wh / 2);

				}
				else
				{
					switch (WindowIndex % 4)
					{
						case 1: x = w - ww; break;
						case 2: y = h - wh; break;
						case 3:
							x = w - ww;
							y = h - wh;
							break;
					}
				}

				SetWindowPos(unityHandle.Handle, HWND.Top, x, y, ww, wh, SWP.NOSIZE);
			}
		}
	}
#else
	public static void PositionWindow()
	{

	}
#endif
}
