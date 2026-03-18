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
    public partial class LoginViewModel : ObservableObject
    {
        private Services.Connection? _connection = null;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        private string _phone = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        [NotifyPropertyChangedFor(nameof(CanLogin))]
        private string _password = "";

        [ObservableProperty]
        private bool _rememberMe = false;

        public bool CanLogin => CanPressLoginButton();

        public LoginViewModel(Services.Connection connection)
        {
            _connection = connection;
        }

        [RelayCommand(CanExecute = nameof(CanPressLoginButton))]
        private async Task Login()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                if (_connection == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Невозможно выполнить сетевой запрос. Внутренная ошибка", "Закрыть");
                    return;
                }
                var data = await _connection.Login(Phone, Password);
                if (data != null && data.Error == 0)
                {
                    if (RememberMe)
                    {
                        await Task.WhenAll(
                            SecureStorage.SetAsync("phone", Phone),
                            SecureStorage.SetAsync("password", Password)
                        );
                    }
                    await Application.Current.MainPage.DisplayAlert("Информация", "Вы успешно авторизовались в системе", "ОК");
                    Application.Current.Windows[0].Page = new AppShell();
                    Application.Current.MainPage = new AppShell();
                }
                else
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"{(data != null ? data.Message : "Ошибка сервиса")}", "ОК");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Авторизация не удалась", "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanPressLoginButton()
        {
            return Phone.Length > 0 && Password.Length > 0;
        }

        [RelayCommand]
        private async Task NetworkSettings()
        {
            if (IsBusy) return;
            await Application.Current?.Windows[0]?.Page.Navigation.PushAsync(new NetworkSettingsPage(_connection));
        }

        [RelayCommand]
        private async Task Register()
        {
            if (IsBusy) return;
            await Application.Current?.Windows[0]?.Page.Navigation.PushAsync(new RegisterPage(_connection));
        }
    }
}
