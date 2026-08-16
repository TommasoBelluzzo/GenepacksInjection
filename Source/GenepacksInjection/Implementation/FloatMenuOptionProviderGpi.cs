using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace GenepacksInjection
{
    public sealed class FloatMenuOptionProviderGpi : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;
        protected override bool Undrafted => true;
        protected override bool Multiselect => false;

        private static void OpenGenepackSelection(Pawn actor, Pawn recipient, List<AvailableGenepack> availableGenepacks)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();

            foreach (AvailableGenepack availableGenepack in availableGenepacks)
                options.Add(new FloatMenuOption(availableGenepack.Genepack.LabelCap,
                    () => { TryStartInjection(actor, recipient, availableGenepack); }));

            Find.WindowStack.Add(new FloatMenu(options));
        }

        private static void TryStartInjection(Pawn actor, Pawn recipient, AvailableGenepack availableGenepack)
        {
            if (!CheckUtilities.IsValidActor(actor) || !CheckUtilities.IsValidRecipient(recipient))
            {
                TextUtilities.ShowMessageInvalidInjection();
                return;
            }

            Genepack genepack = availableGenepack.Genepack;

            if ((genepack == null) || genepack.Destroyed)
            {
                TextUtilities.ShowMessageGenepackUnavailable();
                return;
            }

            AcceptanceReport acceptance = CheckUtilities.CanApplyGenepack(genepack, recipient);

            if (!acceptance.Accepted)
            {
                Messages.Message(acceptance.Reason, MessageTypeDefOf.RejectInput, false);
                return;
            }

            Thing source;

            if (availableGenepack.Container != null)
            {
                source = availableGenepack.Container;

                if ((source == null) || !source.Spawned || source.Destroyed || !CheckUtilities.CanReserveAndReach(actor, source))
                {
                    TextUtilities.ShowMessageGenepackUnavailable();
                    return;
                }

                CompGenepackContainer container = source.TryGetComp<CompGenepackContainer>();

                if ((container == null) || !container.ContainedGenepacks.Contains(genepack))
                {
                    TextUtilities.ShowMessageGenepackUnavailable();
                    return;
                }
            }
            else
            {
                source = genepack;

                if (!genepack.Spawned || !CheckUtilities.CanReserveAndReach(actor, genepack))
                {
                    TextUtilities.ShowMessageGenepackUnavailable();
                    return;
                }
            }

            Job job = JobMaker.MakeJob(JobDefOfGpi.GenepacksInjection_InjectGenepack, recipient, source, genepack);
            job.count = 1;

            actor.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            Pawn actor = context.FirstSelectedPawn;

            if (!CheckUtilities.IsValidActor(actor) || !CheckUtilities.IsValidRecipient(clickedPawn))
                return null;

            List<AvailableGenepack> availableGenepacks = StateUtilities.GetAvailableGenepacks(actor);

            if (availableGenepacks.Count == 0)
                return new FloatMenuOption(TextUtilities.GetLabelInjectGenepack(clickedPawn, false), null)
                {
                    Disabled = true
                };

            return new FloatMenuOption(TextUtilities.GetLabelInjectGenepack(clickedPawn, true),
                () => { OpenGenepackSelection(actor, clickedPawn, availableGenepacks); });
        }
    }
}