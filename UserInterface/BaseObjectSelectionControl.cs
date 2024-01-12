using Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UserInterface
{
    /// <summary>
    /// Signatur fuer eine Callback-Methode, um zu erfahren,
    /// dass ein Objekt eingefügt wurde
    /// </summary>
    /// <param name="baseObject">Ausgewähltes Objekt</param>
    public delegate void OnBaseObjectSelected(BaseObject baseObject);

    public class BaseObjectSelectionControl : UserControl
    {
        public OnBaseObjectSelected Selected;

        protected void ObjectSelected(BaseObject objectSelected)
        {
            if(Selected != null)
            {
                Selected(objectSelected);
            }
        }
    }
}
