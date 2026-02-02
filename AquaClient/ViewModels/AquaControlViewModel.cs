using AquaClient.Classes.Responses;
using AquaClient.Converters;
using AquaClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AquaClient.ViewModels
{
    public partial class AquaControlViewModel : ObservableObject
    {
        private Services.Connection? _connection = null;
        private AquaControlPage _page;


        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private ObservableCollection<Classes.Responses.AquaDeviceInfo> _devicesList;

        private Grid? _devicesGrid = null;
        public AquaControlViewModel(Services.Connection connection, AquaControlPage page)
        {
            _connection = connection;
            _page = page;
            _devicesGrid = _page.FindByName<Grid>("devices");
        }

        [RelayCommand]
        public async Task RefreshDevicesList()
        {
            if (IsBusy) return;
            try
            {
                if (_connection == null) return;
                IsBusy = true;
                var data = await _connection.GetDevicesInfo();
                if (data == null || data.Error != 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"{(data == null ? "Ошибка сервиса" : data.Message)}", "Закрыть");
                    return;
                }
                DevicesList = new ObservableCollection<Classes.Responses.AquaDeviceInfo>(data.Data);
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

        [RelayCommand]
        public async Task UpdateState(object parameter)
        {
            if (parameter is int id)
            {
                if (IsBusy) return;
                try
                {
                    IsBusy = true;
                    //await Task.Delay(1000);
                    var device = DevicesList.FirstOrDefault(d => d.DeviceId == id);
                    if (device == null) return;
                    var index = DevicesList.IndexOf(device);
                    var newValue = device.DeviceStatus == 0 ? 1 : 0;
                    var data = await _connection.SetDeviceStatus(id, newValue);
                    if (data == null || data.Error != 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", $"{(data == null ? "Ошибка сервиса" : data.Message)}", "Закрыть");
                        return;
                    }
                    DevicesList[index] = data.Data;
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
        }
    }
}
