using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSVC_PTIT.Core.Interfaces;
using CSVC_PTIT.Data.Entities;
using CSVC_PTIT.Data.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CSVC_PTIT.App.ViewModels;

/// <summary>
/// ViewModel cho màn hình Quản lý tài khoản (UC-AD01, AD_BM01).
/// Sprint 1 — Task A.5
/// </summary>
public partial class QuanLyTaiKhoanViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private List<User> _allUsers = new();

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private List<Role> _roles = new();

    [ObservableProperty]
    private List<Department> _departments = new();

    public QuanLyTaiKhoanViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? _allUsers
            : _allUsers.Where(u =>
                (u.FullName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.Username?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

        Users = new ObservableCollection<User>(filtered);
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            _allUsers = await userService.GetAllAsync();
            Roles = await userService.GetRolesAsync();
            Departments = await userService.GetDepartmentsAsync();

            ApplyFilter();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task CreateUserAsync(User newUser)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            await userService.CreateAsync(newUser, "123456"); // Mật khẩu mặc định
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    public async Task UpdateUserAsync(User user)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            await userService.UpdateAsync(user);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    public async Task ToggleLockAsync(User user)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            if (user.Status == UserStatus.Locked)
                await userService.UnlockAsync(user.UserId);
            else
                await userService.LockAsync(user.UserId);

            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    public async Task DeleteUserAsync(User user)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            await userService.DeleteAsync(user.UserId);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
