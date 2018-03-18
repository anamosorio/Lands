﻿namespace Lands.ViewModels
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Domain;
    using Helpers;
    using Services;
    using Xamarin.Forms;

    public class ChangePasswordViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiService;
        #endregion

        #region Attributes
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }

        public string CurrentPassword
        {
            get;
            set;
        }

        public string NewPassword
        {
            get;
            set;
        }

        public string Confirm
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public ChangePasswordViewModel()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand ChangePasswordCommand
        {
            get
            {
                return new RelayCommand(ChangePassword);
            }
        }

        private async void ChangePassword()
        {
            if (string.IsNullOrEmpty(this.CurrentPassword))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation,
                    Languages.Accept);
                return;
            }

            if (this.CurrentPassword.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation2,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.NewPassword))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation,
                    Languages.Accept);
                return;
            }

            if (this.NewPassword.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation2,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Confirm))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConfirmValidation,
                    Languages.Accept);
                return;
            }

            if (this.NewPassword != this.Confirm)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConfirmValidation2,
                    Languages.Accept);
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            var request = new ChangePasswordRequest
            {
                CurrentPassword = this.CurrentPassword,
                Email = MainViewModel.GetInstance().User.Email,
                NewPassword = this.NewPassword,
            };

            var apiSecurity = Application.Current.Resources["APISecurity"].ToString();
            var response = await this.apiService.ChangePassword(
                apiSecurity, 
                "/api", 
                "/Users/ChangePassword",
                MainViewModel.GetInstance().TokenType, 
                MainViewModel.GetInstance().Token, 
                request);

            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            this.IsRunning = false;
            this.IsEnabled = true;

            await Application.Current.MainPage.DisplayAlert(
                Languages.ConfirmLabel,
                Languages.ChagePasswordConfirm,
                Languages.Accept);
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        #endregion
    }
}