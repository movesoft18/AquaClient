namespace AquaClient.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(Services.Connection connection)
	{
		InitializeComponent();
		BindingContext = new ViewModels.RegisterViewModel(connection);
	}
}