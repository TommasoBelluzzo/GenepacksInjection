using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace GenepacksInjection
{
    public static class StateUtilities
    {
        public static List<AvailableGenepack> GetAvailableGenepacks(Pawn actor)
        {
            List<AvailableGenepack> result = new List<AvailableGenepack>();

            // Spawned genepacks: ground, shelves, stockpiles and other normal storage.
            foreach (Genepack genepack in actor.Map.listerThings.AllThings.OfType<Genepack>())
            {
                if (!genepack.Spawned || !CheckUtilities.HasCompUseEffect(genepack) || !CheckUtilities.CanReserveAndReach(actor, genepack))
                    continue;

                result.Add(new AvailableGenepack(genepack, null));
            }

            // Genepacks stored in genebanks.
            foreach (Building building in actor.Map.listerBuildings.allBuildingsColonist)
            {
                CompGenepackContainer container = building.TryGetComp<CompGenepackContainer>();

                if ((container == null) || !CheckUtilities.CanReserveAndReach(actor, building))
                    continue;

                foreach (Genepack genepack in container.ContainedGenepacks)
                {
                    if ((genepack == null) || genepack.Destroyed || !CheckUtilities.HasCompUseEffect(genepack))
                        continue;

                    result.Add(new AvailableGenepack(genepack, building));
                }
            }

            return result
                .OrderBy(x => x.Genepack.LabelCap.ToString())
                .ToList();
        }

        public static void ApplyGenepack(Genepack genepack, Pawn recipient)
        {
            if ((genepack == null) || (recipient == null))
                return;

            foreach (GeneDef geneDef in genepack.GeneSet.GenesListForReading)
            {
                #pragma warning disable CS0618
                if (!recipient.genes.HasGene(geneDef))
                #pragma warning restore CS0618
                    recipient.genes.AddGene(geneDef, !GenepacksInjectionMod.Settings.UseEndogenes);
            }

            recipient.health.AddHediff(HediffDefOf.XenogerminationComa);
        }
    }
}