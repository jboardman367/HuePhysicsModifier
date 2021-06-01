using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityModManagerNet;
using InControl;
using HarmonyLib;
using System.Reflection;
using Fabric;
using System.Runtime.CompilerServices;

namespace HuePhysicsModifier
{
	static class Main
	{
		public static Settings settings;

		public static bool dbljRelease = false;
		public static bool forceDblj = false;
		public static int dbljReset = 0;
		public static Vector2 wind = new Vector2(0, 0);
		public static System.Random seedRandomiser = new System.Random();
		public static System.Random windRandomiser = null;
		public static int lastWindSeed = 0;

		public static bool enabled;
		public static UnityModManager.ModEntry mod;

		static bool Load(UnityModManager.ModEntry modEntry)
		{
			settings = Settings.Load<Settings>(modEntry);

			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			mod = modEntry;
			modEntry.OnToggle = OnToggle;
			modEntry.OnGUI = OnGUI;
			modEntry.OnSaveGUI = OnSaveGUI;

			return true;
		}
		static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			enabled = value;

			return true;
		}

		static void OnGUI(UnityModManager.ModEntry modEntry)
        {
			settings.Draw(modEntry);
        }

		static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
			settings.Save(modEntry);
        }
	}



	
}
