using RimWorld;
using Verse;

namespace GenepacksInjection
{
    [DefOf]
    public static class JobDefOfGpi
    {
        public static JobDef GenepacksInjection_InjectGenepack;

        static JobDefOfGpi()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOfGpi));
        }
    }
}