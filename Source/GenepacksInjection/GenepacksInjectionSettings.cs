using Verse;

namespace GenepacksInjection
{
    public class GenepacksInjectionSettings : ModSettings
    {
        public bool UseEndogenes = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref UseEndogenes, "useEndogenes", false);
            base.ExposeData();
        }
    }
}
