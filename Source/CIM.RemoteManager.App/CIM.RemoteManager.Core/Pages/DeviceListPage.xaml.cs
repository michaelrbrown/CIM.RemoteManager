using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Pages
{
    /// <summary>
    /// Class DeviceListPage.
    /// </summary>
    /// <seealso cref="CIM.RemoteManager.Core.Pages.BaseTabbedPage" />
    public partial class DeviceListPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceListPage"/> class.
        /// </summary>
        public DeviceListPage()
        {
            InitializeComponent();

            // Add device settings toolbar icon and handle selection
            ToolbarItems.Add(new ToolbarItem("Refresh Devices", "nav_RefreshDevices.png", () =>
            {
                var deviceListViewModel = (DeviceListViewModel)this.BindingContext;
                deviceListViewModel.RefreshCommand.Execute();
            }, ToolbarItemOrder.Primary, 0));
        }
    }
}
