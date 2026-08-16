using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace GenepacksInjection
{
    public sealed class JobDriverGpi : JobDriver
    {
        private const int InjectionDurationTicks = 150;
        private const TargetIndex RecipientIndex = TargetIndex.A;
        private const TargetIndex SourceIndex = TargetIndex.B;
        private const TargetIndex GenepackIndex = TargetIndex.C;

        private Pawn Recipient => job.GetTarget(RecipientIndex).Pawn;
        private Thing Source => job.GetTarget(SourceIndex).Thing;
        private Genepack Genepack => job.GetTarget(GenepackIndex).Thing as Genepack;

        private Toil CreateToilExtraction()
        {
            Toil toil = new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Instant,
                initAction = delegate
                {
                    Genepack genepack = Genepack;

                    if ((genepack == null) || genepack.Destroyed)
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    Thing source = Source;
                    CompGenepackContainer container = source?.TryGetComp<CompGenepackContainer>();

                    if ((container == null) || !container.ContainedGenepacks.Contains(genepack))
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    ThingOwner sourceOwner = container.GetDirectlyHeldThings();

                    if ((sourceOwner == null) || !sourceOwner.Contains(genepack))
                    {
                        EndJobWith(JobCondition.Incompletable);
                        return;
                    }

                    int transferred = sourceOwner.TryTransferToContainer(genepack, pawn.carryTracker.innerContainer, 1);

                    if (transferred != 1)
                        EndJobWith(JobCondition.Incompletable);
                }
            };

            return toil;
        }

        private Toil CreateToilInjection()
        {
            Toil toil = Toils_General.WaitWith(
                RecipientIndex,
                InjectionDurationTicks, true,
                false, false, RecipientIndex,
                PathEndMode.Touch
            );

            toil.AddEndCondition(delegate
            {
                if (!CheckUtilities.IsValidRecipient(Recipient))
                    return JobCondition.Incompletable;

                return JobCondition.Ongoing;
            });

            toil.AddFinishAction(delegate
            {
                if (pawn.CurJob != job)
                    return;

                Pawn recipient = Recipient;
                Genepack genepack = pawn.carryTracker.CarriedThing as Genepack;

                if (!CheckUtilities.IsValidRecipient(recipient)
                    || !CheckUtilities.CanApplyGenepack(genepack, recipient).Accepted)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                StateUtilities.ApplyGenepack(genepack, recipient);
                genepack.Destroy(DestroyMode.Vanish);
            });

            return toil;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => !CheckUtilities.IsValidActor(pawn) || !CheckUtilities.IsValidRecipient(Recipient));
            this.FailOnDespawnedOrNull(RecipientIndex);
            this.FailOnDestroyedOrNull(SourceIndex);

            bool isContained = (Source != null) && (Genepack != null) && (Source != Genepack);

            yield return Toils_Goto.GotoThing(SourceIndex, isContained ? PathEndMode.Touch : PathEndMode.ClosestTouch);

            if (isContained)
                yield return CreateToilExtraction();
            else
                yield return Toils_Haul.StartCarryThing(GenepackIndex);

            yield return Toils_Goto.GotoThing(RecipientIndex, PathEndMode.Touch);
            yield return CreateToilInjection();
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(Recipient, job, 1, -1, null, errorOnFailed))
                return false;

            return pawn.Reserve(Source, job, 1, 1, null, errorOnFailed);
        }
    }
}