using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserInterface;
using System.Threading.Tasks;
using Basics;

namespace DepotExplorer
{
    class DepotSelectionViewModel : SelectionViewModel
    {
        public DepotSelectionViewModel()
        {
            DepotId = BaseObject.NotSpecified;
            DepotSelected = null;
        }
        public override bool IsValid()
        {
            return (DepotId != BaseObject.NotSpecified);
        }

        public object DepotSelected
        {
            get
            {
                return _dvm;
            }
            set
            {
                var depot = value as DepotViewModel;
                if (depot != null)
                {
                    _dvm = depot;
                    DepotId = depot.ObjectId;
                }
                else
                {
                    DepotId = BaseObject.NotSpecified;
                }
            }
        }
        private DepotViewModel _dvm;
        
        public int DepotId
        {
            get
            {
                return _depotId;
            }
            set
            {
                _depotId = value;
                UpdateProperty("Depot");
            }
        }
        private int _depotId;
    }
}
