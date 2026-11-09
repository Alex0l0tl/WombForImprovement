using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace WombForImprovement.Harmony
{
    /// <summary>
    /// Decision 4: Pawn.Sterile() is the real vanilla 1.6 seam. Hard veto rides
    /// here (Phase 2's fertilityFactor=0 stage hediffs reach Sterile through the
    /// Fertility-stat check anyway; this covers anatomy absence directly).
    /// </summary>
    public static class SterileSeamVeto
    {
        public static void Postfix(Pawn __instance, ref bool __result)
        {
            if (__result || !WFISettings.SterileSeamEnabled) return;
            if (__instance == null || __instance.health == null || __instance.health.hediffSet == null) return;
            if (!__instance.RaceProps.Humanlike) return;

            if (!HasUsableGonads(__instance))
                __result = true;
        }

        static bool HasUsableGonads(Pawn pawn)
        {
            var body = pawn.RaceProps.body;
            if (!body.AllParts.Any(p => p.def == WFIBodyPartDefOf.WFI_Testis)
             && !body.AllParts.Any(p => p.def == WFIBodyPartDefOf.WFI_Ovary))
                return true; // unpatched body (HAR etc.) -> degrade silently, no veto

            int testes = 0, ovaries = 0;
            foreach (var part in pawn.health.hediffSet.GetNotMissingParts())
            {
                if (part.def == WFIBodyPartDefOf.WFI_Testis) testes++;
                else if (part.def == WFIBodyPartDefOf.WFI_Ovary) ovaries++;
            }
            // No functional gamete source at all => sterile.
            return testes > 0 || ovaries > 0;
        }
    }
}