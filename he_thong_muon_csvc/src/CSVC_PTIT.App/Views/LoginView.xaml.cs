using CSVC_PTIT.App.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CSVC_PTIT.App.Views;

public partial class LoginView : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // Callback khi login thành công
        _viewModel.OnLoginSuccess = () =>
        {
            this.DialogResult = true;
            this.Close();
        };
    }

    private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
