namespace AquaClient.Views;
using AquaClient.Classes.Responses;

public partial class ConfirmRegisterPage : ContentPage
{
    private TaskCompletionSource<Classes.Responses.ResponseBase<ConfirmCodeDataResponse?>?> _tcs;
    public TaskCompletionSource<Classes.Responses.ResponseBase<ConfirmCodeDataResponse?>?> ConfirmResult => _tcs;
    public ConfirmRegisterPage(Services.Connection connection, string phone)
    {
        InitializeComponent();
        _tcs = new();
        BindingContext = new ViewModels.ConfirmRegisterViewModel(connection, phone, _tcs);
    }
}