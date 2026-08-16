using UnityEngine;
using Verse;

namespace GenepacksInjection
{
    public class GenepacksInjectionMod : Mod
    {
        public static GenepacksInjectionSettings Settings { get; private set; }

        public GenepacksInjectionMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<GenepacksInjectionSettings>();
        }

        public override string SettingsCategory()
        {
            return "GPI_ModName".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();

            listing.Begin(inRect);
            listing.CheckboxLabeled(
                "GPI_UseEndogenesLabel".Translate(),
                ref Settings.UseEndogenes,
                "GPI_UseEndogenesDescription".Translate()
            );
            listing.End();

            base.DoSettingsWindowContents(inRect);
        }
    }
}
