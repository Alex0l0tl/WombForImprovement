using Verse;

namespace WombForImprovement
{
    /// <summary>
    /// Transient groin trauma. Pain scales with severity so collapse ("downed
    /// by pain") is emergent through vanilla HediffSet.CalculatePain; decay is
    /// driven by HediffComp_Disappears. Severity set at add-time ∝ damage.
    /// </summary>
    public class Hediff_GroinStruck : HediffWithComps
    {
        private const float PainPerSeverity = 0.9f;

        public override float PainOffset => Severity * PainPerSeverity;

        public override string TipStringExtra =>
            base.TipStringExtra + "\nRaw agony, fading over the next half hour.";
    }
}