using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
/// <summary>
/// Debugs.
///0-->Color.red 
///1-->Color.green
///2-->Color.blue
///3-->Color.black
///4-->Color.white
///5-->Color.yellow
///6-->Color.cyan
/// </summary>
public class Debugs {

	//public static SDebug instance {
	//	get {
	//		if (ins == null) {
	//			ins = new SDebug();
	//		}
	//		return ins;
	//	}
	//}
	//static SDebug ins;
	//public SDebug() { }
	//private void OnDestroy() {
	//	ins = null;
	//}

	static Color[] ColorDebug = { Color.red, Color.green, Color.blue, Color.black, Color.white, Color.yellow, Color.cyan };

	public static void Log(UnityEngine.Object message) {
		Color color = ColorDebug[3];
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void Log(string message) {
		Color color = ColorDebug[3];
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void Log(string message, int colors) {
		Color color = ColorDebug[colors];
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));
#endif

	}
	public static void LogRed(string message) {
		Color color = Color.red;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void LogGreen(string message) {
		Color color = Color.green;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogBlue(string message) {
		Color color = Color.blue;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void LogBlack(string message) {
		Color color = Color.black;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogWhite(string message) {
		Color color = Color.white;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogYellow(string message) {
		Color color = Color.yellow;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void LogCyan(string message) {
		Color color = Color.cyan;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}



	public static void Log(int message) {
		Color color = ColorDebug[3];
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void Log(int message, int colors) {
		Color color = ColorDebug[colors];
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void LogRed(int message) {
		Color color = Color.red;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogGreen(int message) {
		Color color = Color.green;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogBlue(int message) {
		Color color = Color.blue;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogBlack(int message) {
		Color color = Color.black;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogWhite(int message) {
		Color color = Color.white;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogYellow(int message) {
		Color color = Color.yellow;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogCyan(int message) {
		Color color = Color.cyan;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}

	public static void Log(float message, int colors) {
		Color color = ColorDebug[colors];
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}


	public static void Log(float message) {
		Color color = ColorDebug[3];
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
	public static void LogRed(float message) {
		Color color = Color.red;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogGreen(float message) {
		Color color = Color.green;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogBlue(float message) {
		Color color = Color.blue;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogBlack(float message) {
		Color color = Color.black;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogWhite(float message) {
		Color color = Color.white;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogYellow(float message) {
		Color color = Color.yellow;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif

	}
	public static void LogCyan(float message) {
		Color color = Color.cyan;
#if TEST
		Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));

#endif
	}
    
    
    public static string ColorString(string message,Color color){
		return string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message);
    }
}
