using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AquaClient.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private Services.Connection? _connection = null;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private string _phone = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private string _email = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private string _password1 = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private string _password2 = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [NotifyPropertyChangedFor(nameof(CanRegister))]
        private string _firstname = "";

        [ObservableProperty]
        private string _middlename = "";

        [ObservableProperty]
        private string _surname = "";

        [ObservableProperty]
        private bool _isValidPhone = false;

        [ObservableProperty]
        private bool _isValidEmail = false;

        [ObservableProperty]
        private bool _isValidName = false;

        [ObservableProperty]
        private bool _isValidPassword1 = false;

        [ObservableProperty]
        private bool _isValidPassword2 = false;

        public bool CanRegister => CanPressRegisterButton();

        public RegisterViewModel(Services.Connection connection)
        {
            _connection = connection;
        }

        [RelayCommand(CanExecute = nameof(CanPressRegisterButton))]
        private async Task Register()
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
                var data = await _connection.Signup(
                    Phone, Email, Firstname, Password1,
                    string.IsNullOrEmpty(Surname) ? null : Surname,
                    string.IsNullOrEmpty(Middlename) ? null : Middlename
                    );
                if (data != null && data.Error == 0)
                {
                    var confirmPage = new Views.ConfirmRegisterPage(_connection, Phone);
                    await Application.Current?.Windows[0]?.Page.Navigation.PushModalAsync(confirmPage);
                    var result = await confirmPage.ConfirmResult.Task;
                    if (result == null) 
                    {
                        await Application.Current.MainPage.DisplayAlert("Предупреждение", $"Вы отказались от ввода кода подтверждения. Аккаунт не может быть создан", "Закрыть");
                       return;
                    }
                    if (result.Error != 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", $"{result.Message}", "Закрыть");
                        return;
                    }
                    if (result.Data != null)
                    {
                        await Task.WhenAll(
                            SecureStorage.SetAsync("phone", Phone),
                            SecureStorage.SetAsync("password", Password1)
                        );
                        Application.Current.MainPage = new AppShell();
                    }
                }
                else
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"{(data != null ? data.Message : "Ошибка сервиса")}", "Закрыть");

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось зарегистрировать аккаунт", "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanPressRegisterButton()
        {
            IsValidPhone = Classes.PhoneValidator.IsValidStrictPhone(Phone);
            IsValidEmail = Classes.EmailValidator.IsValidEmail(Email);
            IsValidName = !string.IsNullOrEmpty(Firstname) && Firstname.All(char.IsLetter);
            IsValidPassword1 = Password1.Length > 2;
            IsValidPassword2 = Password2.Length > 2 && Password2 ==Password1;
            return IsValidPhone && IsValidEmail && IsValidName && IsValidPassword1 && IsValidPassword2;
        }

    }
}