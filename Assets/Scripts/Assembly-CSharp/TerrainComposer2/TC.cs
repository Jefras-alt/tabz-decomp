using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TerrainComposer2
{
	public static class TC
	{
		public class MessageCommand
		{
			public string message;

			public float delay;

			public float duration;

			public float startTime;

			public MessageCommand(string message, float delay, float duration)
			{
				this.message = message;
				this.delay = delay;
				this.duration = duration;
				startTime = Time.realtimeSinceStartup;
			}
		}

		private static int refreshOutputReferences;

		public static bool refreshPreviewImages;

		public static bool repaintNodeWindow;

		public static List<MessageCommand> messages = new List<MessageCommand>();

		public static float autoGenerateCallTimeStart;

		public const int heightOutput = 0;

		public const int splatOutput = 1;

		public const int colorOutput = 2;

		public const int treeOutput = 3;

		public const int grassOutput = 4;

		public const int objectOutput = 5;

		public const int allOutput = 6;

		public const int nodeLabelLength = 19;

		public static readonly string[] outputNames = new string[6] { "Height", "Splat", "Color", "Tree", "Grass", "Object" };

		public static readonly string[] colChannelNames = new string[4] { "Red", "Green", "Blue", "Alpha" };

		public static readonly string[] colChannelNamesLowerCase = new string[4] { "red", "green", "blue", "alpha" };

		public static readonly Color[] colChannel = new Color[4]
		{
			new Color(1f, 0.6f, 0.6f, 1f),
			new Color(0.6f, 1f, 0.6f, 1f),
			new Color(0.6f, 0.6f, 1f, 1f),
			new Color(1f, 1f, 1f, 1f)
		};

		public static string installPath;

		public static string fullInstallPath;

		public static Type FindRTP()
		{
			Type type = Type.GetType("ReliefTerrain");
			TC_Settings.instance.isRTPDetected = ((type != null) ? true : false);
			return type;
		}

		public static float GetVersionNumber()
		{
			return 2.32f;
		}

		public static int OutputNameToOutputID(string outputName)
		{
			for (int i = 0; i < outputNames.Length; i++)
			{
				if (outputName == outputNames[i])
				{
					return i;
				}
			}
			return -1;
		}

		public static void AutoGenerate(bool waitForNextFrame = true)
		{
			AutoGenerate(new Rect(0f, 0f, 1f, 1f), waitForNextFrame);
		}

		public static void AutoGenerate(Rect generateRect, bool waitForNextFrame = true)
		{
			if (TC_Generate.instance != null)
			{
				TC_Generate.instance.cmdGenerate = true;
				TC_Generate.instance.autoGenerateRect = Mathw.ClampRect(generateRect, new Rect(0f, 0f, 1f, 1f));
			}
		}

		public static void RefreshOutputReferences(int outputId)
		{
			refreshOutputReferences = outputId;
		}

		public static int GetRefreshOutputReferences()
		{
			return refreshOutputReferences;
		}

		public static void RefreshOutputReferences(int outputId, bool autoGenerate)
		{
			refreshOutputReferences = outputId;
			if (autoGenerate)
			{
				AutoGenerate();
			}
		}

		public static void Swap<T>(ref T source, ref T dest)
		{
			T val = source;
			source = dest;
			dest = val;
		}

		public static void Swap<T>(List<T> source, int indexS, List<T> dest, int indexD)
		{
			if (indexD >= 0 && indexD < dest.Count)
			{
				T value = source[indexS];
				source[indexS] = dest[indexD];
				dest[indexD] = value;
			}
		}

		public static void Swap<T>(ref T[] source, ref T[] dest)
		{
			for (int i = 0; i < source.Length; i++)
			{
				Swap(ref source[i], ref dest[i]);
			}
		}

		public static void InitArray<T>(ref T[] array, int resolution)
		{
			if (array == null)
			{
				array = new T[resolution];
			}
			else if (array.Length != resolution)
			{
				array = new T[resolution];
			}
		}

		public static void InitArray<T>(ref T[,] array, int resolutionX, int resolutionY)
		{
			if (array == null)
			{
				array = new T[resolutionX, resolutionY];
			}
			else if (array.Length != resolutionX * resolutionY)
			{
				array = new T[resolutionX, resolutionY];
			}
		}

		public static void DestroyChildrenTransform(Transform t)
		{
			int childCount = t.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.Destroy(t.GetChild(childCount - i - 1).gameObject);
			}
		}

		public static void MoveToDustbinChildren(Transform t, int index)
		{
			int childCount = t.childCount;
			if (childCount >= index)
			{
				int num = childCount - index;
				for (int i = 0; i < num; i++)
				{
					MoveToDustbin(t.GetChild(t.childCount - 1));
				}
			}
		}

		public static void SetTextureReadWrite(Texture2D tex)
		{
			if (!(tex == null))
			{
			}
		}

		public static string GetFileName(string path)
		{
			int num = path.LastIndexOf("/");
			if (num != -1)
			{
				string text = path.Substring(num + 1);
				num = text.LastIndexOf(".");
				if (num != -1)
				{
					return text.Substring(0, num);
				}
				return string.Empty;
			}
			return string.Empty;
		}

		public static string GetPath(string path)
		{
			int num = path.LastIndexOf("/");
			if (num != -1)
			{
				return path.Substring(0, num);
			}
			return string.Empty;
		}

		public static string GetAssetDatabasePath(string path)
		{
			return path.Replace(Application.dataPath, "Assets");
		}

		public static bool FileExists(string fullPath)
		{
			if (fullPath == null)
			{
				Debug.Log("Path doesn't exists.");
				return false;
			}
			FileInfo fileInfo = new FileInfo(fullPath);
			if (fileInfo.Exists)
			{
				return true;
			}
			return false;
		}

		public static bool FileExistsPath(string path)
		{
			if (path == null)
			{
				Debug.Log("Path doesn't exists.");
				return false;
			}
			path = Application.dataPath.Replace("Assets", string.Empty) + path;
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				return true;
			}
			return false;
		}

		public static long GetFileLength(string fullPath)
		{
			FileInfo fileInfo = new FileInfo(fullPath);
			return fileInfo.Length;
		}

		public static void GetInstallPath()
		{
		}

		public static bool LoadGlobalSettings()
		{
			return true;
		}

		public static void MoveToDustbin(Transform t)
		{
			TC_Settings instance = TC_Settings.instance;
			if (instance.dustbinT == null)
			{
				instance.CreateDustbin();
			}
			t.parent = instance.dustbinT;
			AddMessage(t.name + " is not compatible with the hierarchy of TerrainComposer\n It is moved to the 'Dustbin' GameObject.");
			AddMessage("If you pressed the delete key you can undo it with control-z", 3f);
		}

		public static void AddMessage(string message, float delay = 0f, float duration = 2f)
		{
			for (int i = 0; i < messages.Count; i++)
			{
				if (messages[i].message.Contains(message))
				{
					return;
				}
			}
			messages.Add(new MessageCommand(message, delay, duration));
		}
	}
}
