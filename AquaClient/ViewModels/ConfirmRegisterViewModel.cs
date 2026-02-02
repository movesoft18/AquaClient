using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AquaClient.Classes.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AquaClient.ViewModels
{
    public partial class ConfirmRegisterViewModel : ObservableObject
    {
        private Services.Connection? _connection = null;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
        [NotifyPropertyChangedFor(nameof(CanConfirmn))]
        private string _code = "";

        private string _phone = "";

        public bool CanConfirmn => CanPressConfirmButton();
        private TaskCompletionSource<Classes.Responses.ResponseBase<ConfirmCodeDataResponse?>?> _tcs;

        public ConfirmRegisterViewModel(Services.Connection? connection, string phone, TaskCompletionSource<Classes.Responses.ResponseBase<ConfirmCodeDataResponse?>?> tcs)
        {
            _connection = connection;
            _phone = phone;
            _tcs = tcs; 
        }

        private bool CanPressConfirmButton()
        {
            return Code.Length == 6 && Code.All(char.IsDigit);
        }

        [RelayCommand(CanExecute = nameof(CanPressConfirmButton))]
        public async Task Confirm()
        {
            if (IsBusy) return;
            try
            {
                if (_connection == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Невозможно выполнить сетевой запрос. Внутренная ошибка", "Закрыть");
                    return;
                }
                var data = await _connection.SendConfirmCode(_phone, Code);
                if (data != null && data.Error == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Информация", "Аккаунт успешно создан", "ОК");
                    _tcs.TrySetResult(data);
                    Application.Current.MainPage.Navigation.PopModalAsync();
                }
                else
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"{(data != null ? data.Message : "Ошибка сервиса")}", "Закрыть");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"{ex.Message}", "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async void Cancel()
        {
            _tcs.TrySetResult(null);
            Application.Current.Windows[0].Page.Navigation.PopModalAsync();
        }
    }       
}
