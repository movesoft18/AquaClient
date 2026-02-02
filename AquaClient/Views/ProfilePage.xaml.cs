using System;
using System.Net.Http;

namespace AquaClient.Views;

public partial class ProfilePage : ContentPage
{
	private Services.Connection _connection;
	private bool _avatarLoaded = false;
	public ProfilePage(Services.Connection connection)
	{
		InitializeComponent();
		_connection = connection;
		BindingContext = new ViewModels.ProfileViewModel(connection);
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		if (!_avatarLoaded)
		{
			byte[] imageData = await _connection.LoadUserAvatar();
			if (imageData.Length > 0)
			{
				profileAvatar.Source = ImageSource.FromStream(() => new MemoryStream(imageData));
				_avatarLoaded = true;
			}
		}
        if (BindingContext is ViewModels.ProfileViewModel viewModel)
        {
            await viewModel.LoadUserData();
        }
    }
}