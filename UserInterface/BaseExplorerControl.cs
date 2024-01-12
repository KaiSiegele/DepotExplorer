using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Basics;

namespace UserInterface
{
    /// <summary>
    /// Basisklasse für ein UserControl in dem auf der linken
    /// Seite Objekte ausgewählt und gelöscht, bearbeitet und
    /// aktualisiert werden kann. Die Änderung und die Neuerfassung
    /// geschieht auf der rechten Seite
    /// </summary>
    public class BaseExplorerControl : UserControl
    {
        /// <summary>
        /// Gibt das ausgewaehlte Objekt zurück
        /// </summary>
        /// <returns>Ausgewaehltes Objekt</returns>
        protected virtual BaseObject GetSelectedObject()
        {
            return null;
        }

        /// <summary>
        /// Gibt das Objekt zurück, das gerade geändert, angesehen bzw. neu eingefügt wird
        /// </summary>
        /// <returns>Objekt, das gerade editiert wird</returns>
        protected virtual BaseObject GetEditingObject()
        {
            return null;
        }

        /// <summary>
        /// Gibt den Änderungsmodus zurück
        /// </summary>
        /// <returns>Aktuealle Änderungsmodus</returns>
        protected virtual EditingModus GetEditingModus()
        {
            return EditingModus.Nothing;
        }

        /// <summary>
        /// Stellt fest, ob das Neu Einfügen erlaubt ist
        /// Das Ändern ist erlaubt, wenn kein Objekt gerade
        /// editiert wird
        /// </summary>
        /// <returns>Neu Einfügen ist erlaubt</returns>
        protected bool InsertObjectAllowed()
        {
            return (GetEditingModus() == EditingModus.Nothing);
        }

        /// <summary>
        /// Stellt fest, ob das Ändern erlaubt ist
        /// Das Ändern ist erlaubt, wenn
        /// - Ein Objekt wurde ausgewählt
        /// - Kein Objekt wird gerade editiert
        /// </summary>
        /// <returns>Ändern ist erlaubt</returns>
        protected bool OpenObjectAllowed()
        {
            BaseObject bo = GetSelectedObject();
            if (bo != null)
            {
                return (GetEditingModus() == EditingModus.Nothing);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stellt fest, ob das Löschen erlaubt ist
        /// Das Löschen ist erlaubt, wenn
        /// - Ein Objekt wurde ausgewählt
        /// - Dieses Objekt wird nur angezeigt
        /// </summary>
        /// <returns>Löschen ist erlaubt</returns>
        protected bool RemoveObjectAllowed()
        {
            BaseObject bo = GetSelectedObject();
            if (bo != null)
            {
                if (bo.Id == GetEditingObject().Id)
                {
                    return GetEditingModus() == EditingModus.Viewing;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gibt den Modus in dem sich das Control gerade befindet zurück
        /// </summary>
        protected EditingModus Modus { private get; set; }
    }
}
