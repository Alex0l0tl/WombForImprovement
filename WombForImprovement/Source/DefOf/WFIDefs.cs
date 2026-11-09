namespace WombForImprovement
{
    [RimWorld.DefOf]
    public static class WFIBodyPartDefOf
    {
        public static Verse.BodyPartDef WFI_Testis;
        public static Verse.BodyPartDef WFI_Ovary;
        public static Verse.BodyPartDef WFI_Uterus;

        static WFIBodyPartDefOf()
        {
            RimWorld.DefOfHelper.EnsureInitializedInCtor(typeof(WFIBodyPartDefOf));
        }
    }

    [RimWorld.DefOf]
    public static class WFIBodyPartTagDefOf
    {
        public static Verse.BodyPartTagDef WFI_Gonad;
        public static Verse.BodyPartTagDef WFI_Reproductive;
        public static Verse.BodyPartTagDef WFI_Gestation;
        public static Verse.BodyPartTagDef WFI_Urology;

        static WFIBodyPartTagDefOf()
        {
            RimWorld.DefOfHelper.EnsureInitializedInCtor(typeof(WFIBodyPartTagDefOf));
        }
    }

    [RimWorld.DefOf]
    public static class WFIHediffDefOf
    {
        public static Verse.HediffDef WFI_GroinStruck;
        // Placeholder until Phase 0's real gonad-state def is integrated.
        public static Verse.HediffDef WFI_GonadState;

        static WFIHediffDefOf()
        {
            RimWorld.DefOfHelper.EnsureInitializedInCtor(typeof(WFIHediffDefOf));
        }
    }
}