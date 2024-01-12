namespace UserInterface
{
    public enum EditingModus
    {
        Nothing,
        Inserting,
        Viewing,
        Updating
    }

    public static class EditingModusExtesion
    {
        public static bool IsEditing(this EditingModus modus)
        {
            return (modus == EditingModus.Inserting || modus == EditingModus.Updating || modus == EditingModus.Viewing);
        }

        public static bool IsModifying(this EditingModus modus)
        {
            return (modus == EditingModus.Inserting || modus == EditingModus.Updating);
        }
    }
}
