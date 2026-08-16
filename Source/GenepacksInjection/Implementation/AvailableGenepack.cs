using RimWorld;
using Verse;

namespace GenepacksInjection
{
    public class AvailableGenepack
    {
        public Genepack Genepack { get; }
        public Thing Container { get; }

        public AvailableGenepack(Genepack genepack, Thing container)
        {
            Genepack = genepack;
            Container = container;
        }
    }
}