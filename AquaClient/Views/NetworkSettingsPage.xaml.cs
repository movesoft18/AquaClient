namespace AquaClient.Views;

public partial class NetworkSettingsPage : ContentPage
{
	public NetworkSettingsPage(Services.Connection connection)
	{
		InitializeComponent();
        BindingContext = new ViewModels.NetworkSettingsViewModel(connection);
    }
}