using Verse;

namespace WombForImprovement
{
    public class WFISettings : ModSettings
    {
        // Sliders per ratified spec; 0 = feature off.
        public static float UnarmedChance = 0.075f;   // "modest" default ~5-10%
        public static float AnimalBiteChance = 0.05f;
        public static bool SterileSeamEnabled = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref UnarmedChance, "unarmedGroinChance", 0.075f);
            Scribe_Values.Look(ref AnimalBiteChance, "animalBiteGroinChance", 0.05f);
            Scribe_Values.Look(ref SterileSeamEnabled, "sterileSeamEnabled", true);
        }
    }
}