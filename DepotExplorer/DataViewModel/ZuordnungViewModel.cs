using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class ZuordnungViewModel : ViewObjectViewModel
    {
        public ZuordnungViewModel(BaseObject zuordnung)
          : base(zuordnung)
        {
        }

        public override string ToString()
        {
            return WertpapierName + " (" + WertpapierArt.ToString() + ")";
        }

        public int Wertpapier
        {
            get
            {
                return _bo.Id;
            }
        }

        public string WertpapierName
        {
            get
            {
                var wp = GetWertpapier();
                return (wp != null ? wp.Name : string.Empty);
            }
        }

        public string WKN
        {
            get
            {

                var wp = GetWertpapier();
                return (wp != null ? wp.WKN : string.Empty);
            }
        }

        public WertpapierArt WertpapierArt
        {
            get
            {
                var wp = GetWertpapier();
                return (wp != null ? wp.Art : WertpapierArt.None);
            }
        }

        private Wertpapier GetWertpapier()
        {
            if (_wp == null)
            {
                var zu = _bo as Zuordnung;
                if (zu != null)
                {
                    _wp = WertpapierInfo.GetWertpapierById(zu.Wertpapier);
                }
            }
            return _wp;
        }

        private Wertpapier _wp = null;
    }

    class ZuordnungViewModels : ObjectViewModels
    {
        public ZuordnungViewModels(BaseObjects bos)
          : base(bos, "Wertpapier", false)
        {
        }

        protected override bool ShouldInsertViewModel(BaseObject baseObject)
        {
            return baseObject.Removed == false;
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new ZuordnungViewModel(baseObject);
        }
    }
}
