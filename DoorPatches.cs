using HarmonyLib;
using System;
using UnityEngine;

namespace HuePhysicsModifier
{
    [HarmonyPatch(typeof(DoorNode), "GetTarget")]
    public static class DoorNode_GetTarget_Patch
    {
        [HarmonyPrefix]
        static bool DetectRoomMove()
        {
            if (Main.windRandomiser is null || Main.settings.windSeed == 0)
            {
                if (Main.settings.windSeed == 0)
                {
                    Main.settings.windSeed = Main.seedRandomiser.Next();
                    Main.lastWindSeed = Main.settings.windSeed;
                }
                Main.windRandomiser = new System.Random(Main.settings.windSeed);
                Main.settings.Save(Main.mod);
            }

            if (Main.lastWindSeed != Main.settings.windSeed)
            {
                Main.windRandomiser = new System.Random(Main.settings.windSeed);
                Main.lastWindSeed = Main.settings.windSeed;
            }

            float ang = Main.windRandomiser.Next(0, 628) / 100f;
            switch (Main.settings.windLevel)
            {
                case WindLevels.None:
                    Main.wind = new UnityEngine.Vector2(0, 0);
                    break;

                case WindLevels.Low:
                    Main.wind = new UnityEngine.Vector2(0.2f * Main.windRandomiser.Next(-100,100)/100f, 0.1f * Main.windRandomiser.Next(-100, 100) / 100f);
                    break;
                case WindLevels.Medium:
                    Main.wind = new UnityEngine.Vector2((0.2f + 0.3f * Main.windRandomiser.Next(0, 100) / 100f) * (Main.windRandomiser.Next(0,2) == 1 ? 1 : -1),
                        (0.1f + 0.07f * Main.windRandomiser.Next(0, 100) / 100f) * (Main.windRandomiser.Next(0, 2) == 1 ? 1 : -1));
                    break;
                case WindLevels.High:
                    Main.wind = new UnityEngine.Vector2((0.5f + 0.5f * Main.windRandomiser.Next(0, 100) / 100f) * (Main.windRandomiser.Next(0, 2) == 1 ? 1 : -1),
                        (0.17f + 0.17f * Main.windRandomiser.Next(0, 100) / 100f) * (Main.windRandomiser.Next(0, 2) == 1 ? 1 : -1));
                    break;
            }

            return true;
        }
    }
}