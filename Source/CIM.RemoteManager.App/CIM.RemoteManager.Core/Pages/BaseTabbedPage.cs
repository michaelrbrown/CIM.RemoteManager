using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Pages
{
    /// <summary>
    /// Class BaseTabbedPage.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.TabbedPage" />
    public class BaseTabbedPage : TabbedPage
    {
        /// <summary>
        /// When overridden, allows application developers to customize behavior immediately prior to the <see cref="T:Xamarin.Forms.Page" /> becoming visible.
        /// </summary>
        /// <remarks>To be added.</remarks>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var viewModel = BindingContext as BaseViewModel;
            viewModel?.Resume();
        }

        /// <summary>
        /// When overridden, allows the application developer to customize behavior as the <see cref="T:Xamarin.Forms.Page" /> disappears.
        /// </summary>
        /// <remarks>To be added.</remarks>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var viewModel = BindingContext as BaseViewModel;
            viewModel?.Suspend();
        }

    }
}
