using UserInterface;
using Basics;

namespace DepotExplorer
{
    class JahrSelectionViewModel : SelectionViewModel
    {
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
                UpdateProperty("Jahr");
            }
        }
        private int _jahrId;
    }
}
