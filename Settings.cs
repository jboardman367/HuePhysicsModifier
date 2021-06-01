using UnityModManagerNet;

namespace HuePhysicsModifier
{
    public enum WindLevels
    {
        None, Low, Medium, High
    }
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Number of extra jumps:", Type = DrawType.Slider, Min = 0, Max = 5)] public int maxJumps = 0;
        [Draw("Scale ground friction (lower = slippery):", Type = DrawType.Slider, Min = 0.05, Max = 5, Precision = 2)] public float groundFrictScale = 1f;
        [Draw("Scale air resistace:", Type = DrawType.Slider, Min = 0.05, Max = 5, Precision = 2)] public float airFrictScale = 1f;
        [Draw("Wind strength:", Type = DrawType.ToggleGroup)] public WindLevels windLevel;
        //[Draw("Dev finding wind stronk x", Type = DrawType.Field)] public float xWind = 0f;
        //[Draw("Dev finding wind stronk y", Type = DrawType.Field)] public float yWind = 0f;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
        public void OnChange()
        {
            if (windLevel == WindLevels.None)
                Main.wind = new UnityEngine.Vector2(0, 0);
        }
    }
}
