using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WombForImprovement
{
    public static class GonadManager
    {
        // Tag traversal via actual BodyPartTagDef defs — tags are defs, not strings.
        public static IEnumerable<BodyPartRecord> RecordsWithTag(Pawn pawn, BodyPartTagDef tag)
        {
            if (tag == null) yield break;
            foreach (var p in pawn.RaceProps.body.AllParts)
                if (p.coverageAbs > 0f && p.def.tags.Contains(tag))
                    yield return p;
        }

        public static bool HasGonadRecords(Pawn pawn) =>
            RecordsWithTag(pawn, WFIBodyPartTagDefOf.WFI_Gonad).Any();

        /// <summary>Phase 0.7 migration: relocate test-save gonad hediffs from
        /// borrowed records onto WFI_* records, preserving endogene payload +
        /// xenotype label. BLOCKED on Phase 0 source (defNames + payload
        /// fields) — scaffold only.</summary>
        public static void MigrateExistingSave(Pawn pawn)
        {
            throw new System.NotImplementedException(
                "Awaiting Phase 0 gonad hediff defNames + payload structure.");
        }
    }
}