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
    /// </summary>
    public static class UnarmedGroinRedirect
    {
        // VERIFY against local 1.6 decompile: field on DamageWorker that
        // ChooseHitPart assigns. Standard name through 1.5 is lastHitPartInternal.
        static readonly FieldInfo LastHitPartFieldInfo =
            AccessTools.Field(typeof(DamageWorker), "lastHitPartInternal");

        public static void Postfix(DamageWorker __instance, DamageInfo dinfo, Pawn pawn)
        {
            try
            {
                if (pawn == null || pawn.Dead || pawn.health == null || pawn.health.hediffSet == null) return;
                if (LastHitPartFieldInfo == null)
                {
                    Log.ErrorOnce("[WFI] DamageWorker hit-part field 'lastHitPartInternal' not found — redirect disabled.", 777000);
                    return;
                }

                float chance = __instance is DamageWorker_Bite
                    ? WFISettings.AnimalBiteChance
                    : WFISettings.UnarmedChance;
                if (chance <= 0f || !Rand.Chance(chance)) return;

                var current = (BodyPartRecord)LastHitPartFieldInfo.GetValue(__instance);

                // Height gating: only bottom-of-torso strikes redirect.
                if (current == null || current.height != BodyPartHeight.Bottom) return;
                if (current.coverageAbs <= 0f) return; // never touch unhittable parts

                var testes = IntactTestes(pawn);
                if (testes.Count == 0) return; // wrong belief -> silent torso resolution

                var target = testes.RandomElement();
                LastHitPartFieldInfo.SetValue(__instance, target);

                // Pain + stagger hediff; severity ∝ damage. Pain-driven collapse
                // is emergent via vanilla HediffSet.CalculatePain.
                var hediff = HediffMaker.MakeHediff(WFIHediffDefOf.WFI_GroinStruck, pawn, target);
                hediff.Severity = Mathf.Clamp01(dinfo.Amount * 0.06f);
                pawn.health.AddHediff(hediff);

                // Native stagger: no generic wound->stagger path exists; call it.
                if (pawn.stances != null && pawn.stances.stagger != null)
                    pawn.stances.stagger.StaggerFor(60, 0.05f);
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