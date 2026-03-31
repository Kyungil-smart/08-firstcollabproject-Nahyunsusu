using System.Diagnostics;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace _Scripts.LYC.Utils
{
	public static class ExtensionLogger
	{
		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		public static void Log(this object _, object message,
			[CallerFilePath] string filePath = "")
		{
			string fileName = System.IO.Path.GetFileName(filePath);
			Debug.Log($"[{fileName}] {message}");
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		public static void LogWarning(this object _, object message,
			[CallerFilePath] string filePath = "")
		{
			string fileName = System.IO.Path.GetFileName(filePath);
			Debug.LogWarning($"[{fileName}] {message}");
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		public static void LogError(this object _, object message,
			[CallerFilePath] string filePath = "")
		{
			string fileName = System.IO.Path.GetFileName(filePath);
			Debug.LogError($"[{fileName}] {message}");
		}
	}
}