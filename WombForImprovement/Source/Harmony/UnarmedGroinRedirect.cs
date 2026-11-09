using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace WombForImprovement.Harmony
{
    /// <summary>
    /// Postfix on DamageWorker_Blunt/Bite.ChooseHitPart. When the worker picked
    /// a Bottom-height torso part and the target HAS intact testes, boost the
    /// odds the strike lands on one. Wrong belief resolves silently: no testes
    /// -> no redirect -> ordinary torso hit (emergent castration upside).
    /// No private-field access needed: ChooseHitPart RETURNS the chosen part.
    /// </summary>
    public static class UnarmedGroinRedirect
    {
        public static void Postfix(DamageWorker __instance, DamageInfo dinfo, Pawn pawn,
            ref BodyPartRecord __result)
        {
            try
            {
                if (pawn == null || pawn.Dead || pawn.health == null || pawn.health.hediffSet == null) return;

                // Height gating: only bottom-of-torso strikes redirect.
                if (__result == null || __result.height != BodyPartHeight.Bottom) return;
                if (__result.coverageAbs <= 0f) return; // never touch unhittable parts

                float chance = __instance is DamageWorker_Bite
                    ? WFISettings.AnimalBiteChance
                    : WFISettings.UnarmedChance;
                if (chance <= 0f || !Rand.Chance(chance)) return;

                var testes = IntactTestes(pawn);
                if (testes.Count == 0) return; // wrong belief -> silent torso resolution

                var target = testes.RandomElement();

                // Pain + stagger hediff; severity ∝ damage. Pain-driven collapse
                // is emergent via vanilla HediffSet.CalculatePain.
                var hediff = HediffMaker.MakeHediff(WFIHediffDefOf.WFI_GroinStruck, pawn, target);
                hediff.Severity = Mathf.Clamp01(dinfo.Amount * 0.06f);
                pawn.health.AddHediff(hediff);

                // Native stagger: no generic wound->stagger path exists; call it.
                if (pawn.stances != null && pawn.stances.stagger != null)
                    pawn.stances.stagger.StaggerFor(60, 0.05f);

                // Redirect the actual hit AFTER hediff/stagger, so failure paths
                // never steal the original part.
                __result = target;
            }
            catch (System.Exception e)
            {
                Log.WarningOnce("[WFI] groin redirect postfix error: " + e, 777001);
            }
        }

        static List<BodyPartRecord> IntactTestes(Pawn pawn)
        {
            var result = new List<BodyPartRecord>();
            foreach (var rec in pawn.RaceProps.body.AllParts)
            {
                if (rec.def != WFIBodyPartDefOf.WFI_Testis) continue;
                if (rec.coverageAbs <= 0f) continue;
                if (!pawn.health.hediffSet.PartIsMissing(rec))
                    result.Add(rec);
            }
            return result;
        }
    }
}