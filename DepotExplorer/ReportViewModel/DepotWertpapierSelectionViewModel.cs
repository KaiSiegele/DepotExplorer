using UserInterface;
using Basics;
using VermoegensData;

namespace DepotExplorer
{
    /// <summary>
    /// SelectionViewModel für die Auswahl eines
    /// Depots und eines zugeordneten Wertpapiers
    /// </summary>
    class DepotWertpapierSelectionViewModel : SelectionViewModel
    {
        public DepotWertpapierSelectionViewModel()
        {
            _depotId = BaseObject.NotSpecified;
            _wpName = string.Empty;
        }

        public override bool IsValid()
        {
            return (!string.IsNullOrEmpty(_wpName) && DepotId != BaseObject.NotSpecified);
        }

        public bool IsWertpapierSelectable
        {
            get
            {
                return DepotId != BaseObject.NotSpecified;
            }
        }

        public object DepotSelected
        {
            get
            {
                return _depotViewModel;
            }
            set
            {
                var depot = value as DepotViewModel;
                if (depot != null)
                {
                    _depotViewModel = depot;
                    DepotId = depot.ObjectId;
                }
                else
                {
                    DepotId = BaseObject.NotSpecified;
                }
            }
        }
        DepotViewModel _depotViewModel;

        public int DepotId
        {
            get
            {
                return _depotId;
            }
            set
            {
                _depotId = value;
                RefreshWertpapiere();
                UpdateProperty("IsWertpapierSelectable");
                UpdateProperty("Depot");
            }
        }
        private int _depotId;
        
        public string WertpapierName
        {
            get
            {
                return _wpName;
            }
            set
            {
                _wpName = value;
                UpdateProperty("WertpapierName");
            }
        }
        private string _wpName;

        public int WertpapierId
        {
            get
            {
                if (IsValid())
                {
                    return _wertpaiere.GetObjectId(_wpName);
                }
                else
                {
                    return BaseObject.NotSpecified;
                }
            }
        }

        public BaseObjects Wertpapiere
        {
            get
            {
                return _wertpaiere;
            }
        }
        private readonly BaseObjects _wertpaiere = new BaseObjects();

        void RefreshWertpapiere()
        {
            _wertpaiere.ClearObjects();
            if (DepotId != BaseObject.NotSpecified)
            {
                string condition = string.Format("wp.Id in (select Wertpapier from Zuordnungen where Depot = {0})", _depotId);
                WertpapierDataObject.LoadWertpapiere(condition, _wertpaiere);
            }
            WertpapierName = string.Empty;
        }
    }
}
