using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace WombForImprovement.Harmony
{
    public sealed class PatchBootstrap
    {
        private const string HarmonyId = "wfi.wombforimprovement";
        private static bool _applied;

        public void Apply(ModContentPack content)
        {
            if (_applied) return;
            _applied = true;

            SanityCheckDefs();

            var h = new HarmonyLib.Harmony(HarmonyId);

            // Blunt + Bite ONLY (never the base class — Blunt/Bite override
            // ChooseHitPart; a base-class patch would never fire for them).
            // This scoping IS the unarmed/animal filter: armed melee uses
            // Scratch/Cut workers, deliberately excluded.
            foreach (var t in new[] { typeof(DamageWorker_Blunt), typeof(DamageWorker_Bite) })
            {
                var mi = AccessTools.Method(t, "ChooseHitPart");
                if (mi == null) { Log.Error("[WFI] ChooseHitPart not found on " + t.Name); continue; }
                h.Patch(mi, postfix: new HarmonyMethod(typeof(UnarmedGroinRedirect), nameof(UnarmedGroinRedirect.Postfix)));
            }

            h.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.Sterile)),
                postfix: new HarmonyMethod(typeof(SterileSeamVeto), nameof(SterileSeamVeto.Postfix)));

            PawnGonadSpawner.Apply(h);

            Log.Message("[WFI] Phase 0.7 harmony patches applied.");
        }

        // Fails LOUDLY at boot if defs/patch/assembly are broken.
        static void SanityCheckDefs()
        {
            var missingDefs = new List<string>();
            foreach (var n in new[] { "WFI_Testis", "WFI_Ovary", "WFI_Uterus" })
                if (DefDatabase<Verse.BodyPartDef>.GetNamedSilentFail(n) == null)
                    missingDefs.Add(n);

            if (missingDefs.Count > 0)
            {
                Log.Error("[WFI] BodyPartDefs missing (defs XML not loaded?): "
                    + string.Join(", ", missingDefs));
                return;
            }

            var human = DefDatabase<BodyDef>.GetNamedSilentFail("Human");
            bool patched = human != null
                && human.AllParts.Any(p => p.def == WFIBodyPartDefOf.WFI_Testis);

            if (!patched)
                Log.Error("[WFI] Human body NOT patched — Patches/Patch_Body_Human.xml "
                    + "missing or failed. (File must live in <mod>/Patches/, NOT under Defs/.)");
            else
                Log.Message("[WFI] sanity: defs loaded, Human body patched, assembly alive.");
        }
    }
}