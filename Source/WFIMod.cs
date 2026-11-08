using System;
using UnityEngine;
using Verse;

namespace WombForImprovement
{
    public class WFIMod : Mod
    {
        public WFIMod(ModContentPack content) : base(content)
        {
            try
            {
                GetSettings<WFISettings>();
                new Harmony.PatchBootstrap().Apply(content);
            }
            catch (Exception e)
            {
                Log.Error("[WFI] init failed: " + e);
            }
        }

        public override string SettingsCategory() => "Womb for Improvement";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(inRect);

            ls.Label("Unarmed groin-strike redirect: " + WFISettings.UnarmedChance.ToStringPercent());
            WFISettings.UnarmedChance = ls.Slider(WFISettings.UnarmedChance, 0f, 0.5f);

            ls.Label("Animal bite redirect: " + WFISettings.AnimalBiteChance.ToStringPercent());
            WFISettings.AnimalBiteChance = ls.Slider(WFISettings.AnimalBiteChance, 0f, 0.5f);

            ls.Gap(4f);
            ls.CheckboxLabeled("Sterility from absent/destroyed gonads (vanilla seam)",
                ref WFISettings.SterileSeamEnabled,
                "Absent or destroyed gonads make the pawn sterile for all vanilla fertility paths.");

            ls.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}