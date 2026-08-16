using RimWorld;
using Verse;
using Verse.AI;

namespace GenepacksInjection
{
    public static class CheckUtilities
    {
        private static bool IsValidPrisoner(Pawn pawn)
        {
            return pawn.IsPrisonerOfColony && !PrisonBreakUtility.IsPrisonBreaking(pawn);
        }

        private static bool IsValidSlave(Pawn pawn)
        {
            return pawn.IsSlaveOfColony && !SlaveRebellionUtility.IsRebelling(pawn);
        }

        public static AcceptanceReport CanApplyGenepack(Genepack genepack, Pawn recipient)
        {
            if ((genepack == null) || (recipient == null) || (recipient.genes == null))
                return "GPI_InvalidInjection".Translate();

            foreach (GeneDef geneDef in genepack.GeneSet.GenesListForReading)
            {
                #pragma warning disable CS0618
                if (!recipient.genes.HasGene(geneDef))
                #pragma warning restore CS0618
                    return true;
            }

            return "GPI_AllGenesAlreadyPresent".Translate();
        }

        public static bool CanReserveAndReach(Pawn actor, LocalTargetInfo target)
        {
            if ((actor == null) || (target == null))
                return false;

            return actor.CanReserveAndReach(target, PathEndMode.Touch, Danger.Some);
        }

        public static bool HasCompUseEffect(Genepack genepack)
        {
            if (genepack == null)
                return false;

            return genepack.TryGetComp<CompUseEffectGpi>() != null;
        }

        public static bool IsValidActor(Pawn actor)
        {
            return actor != null
                && actor.Spawned
                && !actor.DeadOrDowned
                && actor.RaceProps.Humanlike
                && (actor.genes != null)
                && actor.IsColonistPlayerControlled;
        }

        public static bool IsValidRecipient(Pawn recipient)
        {
            return recipient != null
                && recipient.Spawned
                && !recipient.DeadOrDowned
                && recipient.RaceProps.Humanlike
                && (recipient.genes != null)
                && !recipient.InAggroMentalState
                && (IsValidPrisoner(recipient) || IsValidSlave(recipient));
        }
    }
}