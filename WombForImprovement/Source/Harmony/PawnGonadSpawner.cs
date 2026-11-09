using HarmonyLib;
using RimWorld;
using Verse;

namespace WombForImprovement.Harmony
{
    /// <summary>
    /// Attaches the gonad-state hediff at pawn generation, only when the body
    /// actually carries WFI records (HAR bodies without our patch degrade
    /// silently — never programmatically touch records that don't exist).
    ///
    /// TODO (Phase 0 integration): endogene payload + xenotype-origin label;
    /// migration pass (relocate test-save gonad hediffs onto WFI_* records)
    /// belongs in GonadManager.MigrateExistingSave().
    /// </summary>
    public static class PawnGonadSpawner
    {
        public static void Apply(HarmonyLib.Harmony h)
        {
            var mi = AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn),
                new[] { typeof(PawnGenerationRequest) });
            if (mi == null) { Log.Error("[WFI] PawnGenerator.GeneratePawn seam not found"); return; }
            h.Patch(mi, postfix: new HarmonyMethod(typeof(PawnGonadSpawner), nameof(Postfix)));
        }

        public static void Postfix(Pawn __result)
        {
            try
            {
                var pawn = __result;
                if (pawn == null || pawn.health == null || pawn.health.hediffSet == null) return;
                if (pawn.RaceProps.Animal) return;
                if (WFIHediffDefOf.WFI_GonadState == null) return; // def missing: skip quietly
                if (pawn.health.hediffSet.HasHediff(WFIHediffDefOf.WFI_GonadState)) return; // idempotent

                bool hasTestes = false, hasOvaries = false;
                foreach (var p in pawn.RaceProps.body.AllParts)
                {
                    if (p.def == WFIBodyPartDefOf.WFI_Testis) hasTestes = true;
                    else if (p.def == WFIBodyPartDefOf.WFI_Ovary) hasOvaries = true;
                }
                if (!hasTestes && !hasOvaries) return; // unpatched body

                // TODO: Phase 0 payload logic (xenotype payload, intersex rolls,
                // born-null races) replaces this flat default.
                var gonadHediff = HediffMaker.MakeHediff(WFIHediffDefOf.WFI_GonadState, pawn);
                pawn.health.AddHediff(gonadHediff);
            }
            catch (System.Exception e)
            {
                Log.WarningOnce("[WFI] gonad spawn postfix error: " + e, 777002);
            }
        }
    }
}