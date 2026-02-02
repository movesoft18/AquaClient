using AquaClient.Services;
using AquaClient.Views;
namespace AquaClient
{
    public partial class App : Application
    {
        private Connection _connection;
        public App(Connection connection)
        {
            _connection = connection;
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new SplashPage(_connection));
            return window;
        }
    }
}