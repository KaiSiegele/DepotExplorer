using Basics;
using UserInterface;
using VermoegensData;

namespace DepotExplorer
{
    class ZuordnebaresWertpapierViewModels : ObjectViewModels
    {
        public ZuordnebaresWertpapierViewModels(BaseObjects bos, Zuordnungen zuordnungen)
            : base(bos)
        {
            _zuordnungen = zuordnungen;
        }

        public WertpapierArt WertpapierArt { get; set; }

        public void UpdateWertpapierArt(WertpapierArt wertpapierArt)
        {
            WertpapierArt = wertpapierArt;
            RefreshViewModels();
        }

        protected override bool ShouldInsertViewModel(BaseObject baseObject)
        {
            bool result;
            if (WertpapierInfo.GetWertpapierArt(baseObject.Id) == WertpapierArt)
            {
                result = (_zuordnungen.IsWertPapierZugeordnet(baseObject.Id) == false);
            }
            else
            {
                result = false;
            }
            return result;
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new WertpapierViewModel(baseObject);
        }

        private readonly Zuordnungen _zuordnungen = null;
    }
}