using AquaClient.ViewModels;
namespace AquaClient.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(Services.Connection connection)
	{
		InitializeComponent();
		BindingContext = new LoginViewModel(connection);
	}
}