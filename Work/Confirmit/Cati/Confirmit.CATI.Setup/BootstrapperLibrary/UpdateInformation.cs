using BootstrapperLibrary.Interfaces;

namespace BootstrapperLibrary
{
    public class UpdateInformation
    {
        public string ProductNamePathBeforeSxSName { get; private set; }

        public SystemType ProductType { get; private set; }

        public ISelectActionForm SelectActionForm { get; private set; }

        public UpdateInformation(string productNamePathBeforeSxSName, SystemType productType, ISelectActionForm selectActionForm)
        {
            ProductNamePathBeforeSxSName = productNamePathBeforeSxSName;
            SelectActionForm = selectActionForm;
            ProductType = productType;
        }
    }
}