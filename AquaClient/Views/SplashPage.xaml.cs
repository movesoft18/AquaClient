namespace AquaClient.Views;

public partial class SplashPage : ContentPage
{
	private Services.Connection _connection;

    public SplashPage(Services.Connection connection)
	{
		_connection = connection;
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            //await Task.Delay(1000);
            var phone = await SecureStorage.GetAsync("phone");
            var password = await SecureStorage.GetAsync("password");
            var uriApi = await SecureStorage.GetAsync("uri_api");
            if (uriApi != null)
                _connection.UriApi = uriApi;
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(phone))
            {
                var data = await _connection.Login(phone, password);
                if (data != null && data.Error == 0)
                {
                    Application.Current.Windows[0].Page = new AppShell();
                    return;
                }
                else
                    await DisplayAlert("Ошибка", $"{data.Message}. Проверьте правильность сетевых настроек и интернет соединения.", "Закрыть");
            }
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage(_connection));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"{ex.Message}", "Закрыть");
            Application.Current.Windows[0].Page = new NavigationPage(new LoginPage(_connection));
        }
    }
}