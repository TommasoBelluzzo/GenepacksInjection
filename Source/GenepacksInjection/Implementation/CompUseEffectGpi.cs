using RimWorld;
using Verse;

namespace GenepacksInjection
{
    public sealed class CompUseEffectGpi : CompUseEffect
    {
        public CompPropertiesGpi Props => (CompPropertiesGpi)props;

        public override AcceptanceReport CanBeUsedBy(Pawn pawn)
        {
            if ((pawn == null) || !pawn.IsColonistPlayerControlled)
                return false;

            Genepack genepack = parent as Genepack;
            return CheckUtilities.CanApplyGenepack(genepack, pawn);
        }

        public override void DoEffect(Pawn user)
        {
            Genepack genepack = parent as Genepack;
            StateUtilities.ApplyGenepack(genepack, user);
        }

        public override void PrepareTick()
        {
            return;
        }
    }
}