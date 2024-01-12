using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DepotExplorer
{
    public class VermoegensExplorerCommands
    {
        public static readonly RoutedUICommand ViewBaseObject = new RoutedUICommand("ViewBaseObject", "ViewBaseObject", typeof(VermoegensExplorerCommands));

        public static readonly RoutedUICommand SaveAndClose = new RoutedUICommand("SaveAndClose", "SaveAndClose", typeof(VermoegensExplorerCommands));

        public static readonly RoutedUICommand InsertKursWert = new RoutedUICommand("InsertKurswert", "InsertKurswert", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand RemoveKursWert = new RoutedUICommand("RemoveKurswert", "RemoveKurswert", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand ExtendKursWerte = new RoutedUICommand("ExtendKursWerte", "ExtendKursWerte", typeof(VermoegensExplorerCommands));

        public static readonly RoutedUICommand InsertBestand = new RoutedUICommand("InsertBestand", "InsertBestand", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand RemoveBestand = new RoutedUICommand("RemoveBestand", "RemoveBestand", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand ExtendBestaende = new RoutedUICommand("ExtendBestaende", "ExtendBestaende", typeof(VermoegensExplorerCommands));

        public static readonly RoutedUICommand ManageZuordnungen = new RoutedUICommand("ManageZuordnungen", "ManageZuordnungen", typeof(VermoegensExplorerCommands));

        public static readonly RoutedUICommand InsertZuordnung = new RoutedUICommand("InsertZuordnung", "InsertZuordnung", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand RemoveZuordnung = new RoutedUICommand("RemoveZuordnung", "RemoveZuordnung", typeof(VermoegensExplorerCommands));

        public static readonly RoutedUICommand LoadDepotUebersichtPosten = new RoutedUICommand("LoadDepotUebersichtPosten", "LoadDepotUebersichtPosten", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand LoadJahresAbschlussPosten = new RoutedUICommand("LoadJahresAbschlussPosten", "LoadJahresAbschlussPosten", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand LoadDepotEntwicklungPosten = new RoutedUICommand("LoadDepotEntwicklungPosten", "LoadDepotEntwicklungPosten", typeof(VermoegensExplorerCommands));
        public static readonly RoutedUICommand LoadFondEntwicklungPosten = new RoutedUICommand("LoadFondEntwicklungPosten", "LoadFondEntwicklungPosten", typeof(VermoegensExplorerCommands));
    }

}
