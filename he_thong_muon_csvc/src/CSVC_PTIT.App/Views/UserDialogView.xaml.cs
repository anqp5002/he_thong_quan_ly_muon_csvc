using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CSVC_PTIT.App.Views;

/// <summary>
/// Dialog thêm/sửa tài khoản người dùng.
/// Sprint 1 — Task A.5
/// </summary>
public partial class UserDialogView : Window
{
    private readonly User? _editingUser;

    /// <summary>Kết quả trả về sau khi user bấm Lưu</summary>
    public User? ResultUser { get; private set; }

    /// <summary>
    /// Constructor: truyền danh sách roles/departments cho ComboBox.
    /// Nếu editUser != null → chế độ Sửa, ngược lại → chế độ Thêm.
    /// </summary>
    public UserDialogView(List<Role> roles, List<Department> departments, User? editUser = null)
    {
        InitializeComponent();

        _editingUser = editUser;

        // Nạp dữ liệu cho ComboBox
        CmbRole.ItemsSource = roles;
        CmbDepartment.ItemsSource = departments;
        CmbBorrowerType.ItemsSource = Enum.GetValues<BorrowerType>();

        if (editUser != null)
        {
            // Chế độ SỬA
            TxtDialogTitle.Text = "SỬA TÀI KHOẢN";
            TxtFullName.Text = editUser.FullName;
            TxtEmail.Text = editUser.Email;
            TxtPhone.Text = editUser.Phone;
            TxtStudentCode.Text = editUser.StudentCode;
            TxtClassName.Text = editUser.ClassName;
            CmbRole.SelectedValue = editUser.RoleId;
            CmbDepartment.SelectedValue = editUser.DepartmentId;
            CmbBorrowerType.SelectedItem = editUser.BorrowerType;

            // Ẩn trường mật khẩu khi sửa
            TxtPassword.Visibility = Visibility.Collapsed;
        }
        else
        {
            // Chế độ THÊM — mặc định vai trò SV
            CmbRole.SelectedIndex = 0;
            CmbBorrowerType.SelectedIndex = 0;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(TxtFullName.Text))
        {
            ShowError("Vui lòng nhập họ tên.");
            return;
        }
        if (string.IsNullOrWhiteSpace(TxtEmail.Text))
        {
            ShowError("Vui lòng nhập email.");
            return;
        }
        if (CmbRole.SelectedValue == null)
        {
            ShowError("Vui lòng chọn vai trò.");
            return;
        }

        // Tạo User object kết quả
        var user = _editingUser ?? new User();
        user.FullName = TxtFullName.Text.Trim();
        user.Email = TxtEmail.Text.Trim();
        user.Username = TxtEmail.Text.Trim().Split('@')[0]; // Tự sinh username từ email
        user.Phone = TxtPhone.Text.Trim();
        user.StudentCode = TxtStudentCode.Text.Trim();
        user.ClassName = TxtClassName.Text.Trim();
        user.RoleId = (int)CmbRole.SelectedValue;
        user.DepartmentId = CmbDepartment.SelectedValue as int?;
        user.BorrowerType = CmbBorrowerType.SelectedItem is BorrowerType bt ? bt : BorrowerType.Student;

        ResultUser = user;
        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ShowError(string message)
    {
        TxtError.Text = message;
        TxtError.Visibility = Visibility.Visible;
    }
}
