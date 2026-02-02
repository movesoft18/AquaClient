using System.Diagnostics;

namespace AquaClient.Views;

public partial class AquaControlPage : ContentPage
{
	private Services.Connection _connection;

    public AquaControlPage(Services.Connection connection)
	{
        _connection = connection;
        InitializeComponent();
        BindingContext = new ViewModels.AquaControlViewModel(_connection, this);
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ViewModels.AquaControlViewModel viewModel)
        {
            Task.Run(async () => await viewModel.RefreshDevicesList());
        }

    }
}