using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    /// <summary>
    /// Schnittstelle für Componenten,
    /// mit denen innerhalb eines BaseEditControls
    /// das Object verändert wird
    /// </summary>
    public interface IObjectEditComponent
    {
        /// <summary>
        /// Stellt fest, ob alle Angaben 
        /// in Ordnung sind
        /// </summary>
        bool Valid { get; }
        /// <summary>
        /// Stellt fest, ob Änderungen
        /// gemacht wurden
        /// </summary>
        bool Changed { get; }
    }

    public static class ObjectEditComponentExtensions
    {
        /// <summary>
        /// Stellt fest, ob die Änderungen im Objekt
        /// gespeichert werden können.
        /// </summary>
        /// <param name="component">Componente, die das Objekt veraendert</param>
        /// <returns>Änderungen können gespeichert werden</returns>
        public static bool CanSave(this IObjectEditComponent component)
        {
            return component.Valid && component.Changed;
        }
    }
}
