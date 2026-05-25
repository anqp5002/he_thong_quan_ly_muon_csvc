using CSVC_PTIT.App.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace CSVC_PTIT.App.Views;

public partial class ChangePasswordView : Window
{
    private readonly ChangePasswordViewModel _viewModel;

    public ChangePasswordView(ChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        _viewModel.OnSuccess = () =>
        {
            MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        };
    }

    private void txtOldPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm)
            vm.OldPassword = ((PasswordBox)sender).Password;
    }

    private void txtNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm)
            vm.NewPassword = ((PasswordBox)sender).Password;
    }

    private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChangePasswordViewModel vm)
            vm.ConfirmPassword = ((PasswordBox)sender).Password;
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}
