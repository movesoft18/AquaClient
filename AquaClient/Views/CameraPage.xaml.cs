namespace AquaClient.Views;

public partial class CameraPage : ContentPage
{
    private Services.Connection _connection = null;

    public CameraPage(Services.Connection connection)
	{
        _connection = connection;   
		InitializeComponent();
	}

    private void StartClicked(object sender, EventArgs e)
    {
        var videoRoute = _connection.GetUriByName("video");
        videoWebView.Source = videoRoute;
        //videoWebView.Source = "http://192.168.3.11:5000/api/v1/video_feed";
    }

    private void StopClicked(object sender, EventArgs e)
    {
        videoWebView.Source = "about:blank";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        videoWebView.Source = "about:blank";
    }
}