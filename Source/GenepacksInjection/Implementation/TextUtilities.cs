using RimWorld;
using Verse;

namespace GenepacksInjection
{
    public static class TextUtilities
    {
        public static string GetLabelInjectGenepack(Pawn recipient, bool availableGenepacks)
        {
            if (recipient == null)
                return string.Empty;

            if (availableGenepacks)
                return "GPI_InjectGenepack".Translate(recipient);

            return "GPI_InjectGenepack".Translate(recipient) + ": " + "GPI_NoGenepacksAvailable".Translate();
        }

        public static void ShowMessageGenepackUnavailable()
        {
            Messages.Message("GPI_GenepackUnavailable".Translate(), MessageTypeDefOf.RejectInput, false);
        }

        public static void ShowMessageInvalidInjection()
        {
            Messages.Message("GPI_InvalidInjection".Translate(), MessageTypeDefOf.RejectInput, false);
        }
    }
}