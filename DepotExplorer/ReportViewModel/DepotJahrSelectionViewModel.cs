using UserInterface;
using Basics;

namespace DepotExplorer
{
    public class DepotJahrSelectionViewModel : SelectionViewModel
    {
        public DepotJahrSelectionViewModel()
        {
            DepotId = BaseObject.NotSpecified;
            DepotSelected = null;
        }
        public override bool IsValid()
        {
            return (JahrId != BaseObject.NotSpecified && DepotId != BaseObject.NotSpecified);
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
        DepotViewModel _dvm;
        public object JahrSelected
        {
            get
            {
                return _jvm;
            }
            set
            {
                var jahr = value as JahrViewModel;
                if (jahr != null)
                {
                    _jvm = jahr;
                    JahrId = jahr.ObjectId;
                }
                else
                {
                    JahrId = BaseObject.NotSpecified;
                }
            }
        }
        private JahrViewModel _jvm;


        public int JahrId
        {
            get
            {
                return _jahrId;
            }
            set
            {
                _jahrId = value;
                UpdateProperty("JahrId");
            }
        }

        private int _jahrId;

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
