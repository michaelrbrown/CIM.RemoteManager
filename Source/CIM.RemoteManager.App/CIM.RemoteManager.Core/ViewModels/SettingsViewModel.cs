using CIM.RemoteManager.Core.Models;

namespace CIM.RemoteManager.Core.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel()
        {
        }

        private bool _isBusy;
        
        private bool _isFirstRun;
        
        public bool IsFirstRun
        {
            get => _isFirstRun;
            set => SetProperty(ref _isFirstRun, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        
        //private async Task ShowDocumentationAsync()
        //{
            //await App.NavigationService.PushAsync(new DocumentationPage());
        //}
    }
}