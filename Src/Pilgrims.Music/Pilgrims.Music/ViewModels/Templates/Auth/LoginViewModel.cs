﻿using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Pilgrims.Music.Auth;
using Pilgrims.Music.Helpers;
using Pilgrims.Music.Helpers.Validations;
using Pilgrims.Music.Helpers.Validations.Rules;
using Pilgrims.Music.Views;
using Pilgrims.Music.Views.Dialogs;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Pilgrims.Music.ViewModels.Templates.Auth
{
    public class LoginViewModel : BaseRegionViewModel
    {
        #region Private & Protected

        private IDialogService _dialogService;

        #endregion

        #region Properties

        public ValidatableObject<string> Email { get; set; }
        public ValidatableObject<string> Password { get; set; }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; set; }
        public ICommand ValidateCommand { get; set; }

        #endregion

        #region Constructors

        public LoginViewModel(
            DialogService dialogService,
            INavigationService navigationService) : base(navigationService)
        {
            _dialogService = dialogService;

            LoginCommand = new Command(LoginCommandHandler);

            ValidateCommand = new Command<string>(ValidateCommandHandler);

            AddValidations();
        }

        #endregion

        #region Validation Handlers

        private void ValidateCommandHandler(string field)
        {
            switch (field)
            {
                case "email": Email.Validate(); break;
                case "password": Password.Validate(); break;
            }
        }

        #endregion

        #region Command Handlers

        private async void LoginCommandHandler()
        {
            try
            {
                MainState = LayoutState.Loading;
                if (ValidateLoginData())
                {
                    var auth = DependencyService.Get<IFirebaseAuthentication>();
                    var user = await auth.LoginWithEmailAndPassword(Email.Value, Password.Value);

                    if (user != null)
                    {
                        ClearAuthData();
                        Preferences.Set("taskFilterByList", "All lists");
                        await _navigationService.NavigateAsync($"/{nameof(TasksPage)}");
                    }
                    else
                    {
                        var param = new DialogParameters()
                        {
                            { "message", Constants.Errors.WrongUserOrPasswordError }
                        };
                        _dialogService.ShowDialog(nameof(ErrorDialog), param);
                    }
                }
            }
            catch (Exception ex)
            {
                var param = new DialogParameters()
                {
                    { "message", Constants.Errors.GeneralError }
                };
                _dialogService.ShowDialog(nameof(ErrorDialog), param);
                Debug.WriteLine(ex);
            }
            finally
            {
                MainState = LayoutState.None;
            }
        }

        #endregion

        #region Private Functionality

        private void AddValidations()
        {
            Email = new ValidatableObject<string>();
            Password = new ValidatableObject<string>();

            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A email is required." });
            Email.Validations.Add(new IsEmailRule<string> { ValidationMessage = "Email format is not correct" });
            Password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "A password is required." });
        }

        private bool ValidateLoginData()
        {
            if (Email.IsValid == false ||
                Password.IsValid == false)
                return false;
            return true;
        }

        private void ClearAuthData()
        {
            Email.Value = Password.Value = string.Empty;
        }

        #endregion
    }
}
