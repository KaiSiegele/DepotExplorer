using Basics;
using UserInterface;

namespace DepotExplorer
{
    class JahrViewModel : ViewObjectViewModel
    {
        public JahrViewModel(BaseObject jahr)
            : base(jahr)
        {
        }
        public JahrViewModel()
            : this(null)
        {
        }

        public string Name
        {
            get
            {
                if (_bo != null)
                {
                    return _bo.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }

    class JahrViewModels : ObjectViewModels
    {
        public JahrViewModels(BaseObjects bos)
          : base(bos)
        {
        }

        protected override ObjectViewModel CreateViewModel(BaseObject baseObject)
        {
            return new JahrViewModel(baseObject);
        }
    }
}
