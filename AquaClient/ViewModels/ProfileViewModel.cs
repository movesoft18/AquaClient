using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AquaClient.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private Services.Connection? _connection = null;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private bool _editMode = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _phone = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _firstname = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _surname = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _middlename = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _email = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _password = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _password1 = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveProfileCommand))]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private string _password2 = "";

        [ObservableProperty]
        private bool _isValidPhone = true;

        [ObservableProperty]
        private bool _isValidEmail = true;

        [ObservableProperty]
        private bool _isValidName = true;

        [ObservableProperty]
        private bool _isValidPassword = false;

        [ObservableProperty]
        private bool _isValidPassword1 = false;

        [ObservableProperty]
        private bool _isValidPassword2 = false;

        private string _oldsurname = "";
        private string _oldfirstname = "";
        private string _oldmiddlename = "";
        private string _oldphone = "";
        private string _oldemail = "";

        public bool CanSave => CanPressSaveButton();

        public ProfileViewModel(Services.Connection connection)
        {
            _connection = connection;
        }

        [RelayCommand]
        public void EditProfile()
        {
            EditMode = true;
            Password = "";
            _oldfirstname = Firstname;
            _oldmiddlename = Middlename;
            _oldsurname = Surname;
            _oldphone = Phone;
            _oldemail = Email;
        }

        [RelayCommand]
        public async Task Logout()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                bool answer = await Application.Current.MainPage.DisplayAlert(
                    "Подтверждение",          // Заголовок
                    "Вы уверены что хотите выйти из аккаунта?",
                    "Да",                     // Кнопка подтверждения
                    "Нет"                     // Кнопка отмены
                );
                if (answer)
                {
                    var data = await _connection.Logout();
                    if (data != null && data.Error == 0)
                    {
                        SecureStorage.Remove("phone");
                        SecureStorage.Remove("password");
                        Application.Current.MainPage = new NavigationPage(new Views.LoginPage(_connection));
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Ошибка", data.Message, "Закрыть");
                }
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

        //[RelayCommand]
        public async Task LoadUserData()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                var data = await _connection.GetUserInfo();
                if (data != null && data.Error == 0)
                {
                    Firstname = data.Data.Firstname;
                    Middlename = data.Data.Middlename;
                    Surname = data.Data.Surname;
                    Phone = data.Data.Phone;
                    Email = data.Data.Email;
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanPressSaveButton))]
        public async Task SaveProfile()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                var data = await _connection.SaveUserInfo(Firstname, Middlename, Surname, Phone, Email, Password);
                if (data != null && data.Error == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Информация", "Данные профиля успешно сохранены", "Закрыть");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", data.Message, "Закрыть");
                    Cancel();
                }                   
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "Закрыть");
                Cancel();
            }
            finally
            {
                EditMode = false;
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void Cancel()
        {
            EditMode = false;
            Surname = _oldsurname;
            Middlename = _oldmiddlename;
            Firstname = _oldfirstname;
            Phone = _oldphone;
            Email = _oldemail;
        }

        private bool CanPressSaveButton()
        {
            IsValidPhone = Classes.PhoneValidator.IsValidStrictPhone(Phone);
            IsValidEmail = Classes.EmailValidator.IsValidEmail(Email);
            IsValidName = !string.IsNullOrEmpty(Firstname) && Firstname.All(char.IsLetter);
            IsValidPassword1 = Password1.Length > 2 || string.IsNullOrEmpty(Password);
            IsValidPassword2 = (Password2.Length > 2 && Password2 == Password1) || string.IsNullOrEmpty(Password);
            return IsValidPhone && IsValidEmail && IsValidName && IsValidPassword1 && IsValidPassword2;
        }

    }
}
