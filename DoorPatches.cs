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
            switch (Main.settings.windLevel)
            {
                case WindLevels.None:
                    Main.wind = new UnityEngine.Vector2(0, 0);
                    break;

                case WindLevels.Low:
                    Main.wind = new UnityEngine.Vector2(0.2f * Main.randomiser.Next(-100,100)/100f, 0.1f * Main.randomiser.Next(-100, 100) / 100f);
                    break;
                case WindLevels.Medium:
                    Main.wind = new UnityEngine.Vector2((0.2f + 0.3f * Main.randomiser.Next(0, 100) / 100f) * (Main.randomiser.Next(0,2) == 1 ? 1 : -1),
                        (0.1f + 0.07f * Main.randomiser.Next(0, 100) / 100f) * (Main.randomiser.Next(0, 2) == 1 ? 1 : -1));
                    break;
                case WindLevels.High:
                    Main.wind = new UnityEngine.Vector2((0.5f + 0.5f * Main.randomiser.Next(0, 100) / 100f) * (Main.randomiser.Next(0, 2) == 1 ? 1 : -1),
                        (0.17f + 0.17f * Main.randomiser.Next(0, 100) / 100f) * (Main.randomiser.Next(0, 2) == 1 ? 1 : -1));
                    break;
            }

            return true;
        }
    }
}