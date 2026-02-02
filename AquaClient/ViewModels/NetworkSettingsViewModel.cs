using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AquaClient.Views;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AquaClient.ViewModels
{
    public partial class NetworkSettingsViewModel : ObservableObject
    {
        private Services.Connection? _connection = null;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _uriApi = "";

        public bool CanSave => CanPressSaveButton();

        public NetworkSettingsViewModel(Services.Connection connection)
        {
            _connection = connection;
            if (_connection != null) 
                _uriApi = _connection.UriApi;
        }

        [RelayCommand(CanExecute = nameof(CanPressSaveButton))]
        private async Task Save()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                if (_uriApi[^1] != '/')
                    _uriApi += "/";
                await SecureStorage.SetAsync("uri_api", UriApi);
                if (_connection != null) _connection.UriApi = _uriApi;
                await Application.Current.Windows[0].Page.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanPressSaveButton()
        {
            return UriApi.Length > 0;
        }

    }
}
