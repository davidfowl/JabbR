﻿using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Resources;
using JabbR.Services;
using JabbR.ViewModels;
using Nancy;
using Nancy.Cookies;
using System;
using System.Linq;
using WorldDomination.Web.Authentication;

namespace JabbR.Nancy
{
    public class AccountModule : JabbRModule
    {
        public AccountModule(IApplicationSettings applicationSettings,
                             IAuthenticationTokenService authenticationTokenService,
                             IMembershipService membershipService,
                             IJabbrRepository repository,
                             IAuthenticationService authService,
                             IChatNotificationService notificationService)
            : base("/account")
        {
            Get["/"] = _ =>
            {
                if (Context.CurrentUser == null)
                {
                    return HttpStatusCode.Forbidden;
                }

                ChatUser user = repository.GetUserById(Context.CurrentUser.UserName);

                return GetProfileView(authService, user);
            };

            Get["/login"] = _ =>
            {
                if (Context.CurrentUser != null)
                {
                    return Response.AsRedirect("~/");
                }

                return View["login", GetLoginViewModel(applicationSettings, repository, authService)];
            };

            Post["/login"] = param =>
            {
                if (Context.CurrentUser != null)
                {
                    return Response.AsRedirect("~/");
                }

                string username = Request.Form.username;
                string password = Request.Form.password;

                if (String.IsNullOrEmpty(username))
                {
                    this.AddValidationError("username", LanguageResources.UsernameIsRequired);
                }

                if (String.IsNullOrEmpty(password))
                {
                    this.AddValidationError("password", LanguageResources.PasswordIsRequired);
                }

                try
                {
                    if (ModelValidationResult.IsValid)
                    {
                        ChatUser user = membershipService.AuthenticateUser(username, password);
                        return this.CompleteLogin(authenticationTokenService, user);
                    }
                    else
                    {
                        return View["login", GetLoginViewModel(applicationSettings, repository, authService)];
                    }
                }
                catch
                {
                    this.AddValidationError("_FORM", LanguageResources.LoginFailedCheckYourUsernamePassword);
                    return View["login", GetLoginViewModel(applicationSettings, repository, authService)];
                }
            };

            Post["/logout"] = _ =>
            {
                if (Context.CurrentUser == null)
                {
                    return HttpStatusCode.Forbidden;
                }

                var response = Response.AsJson(new { success = true });

                response.AddCookie(new NancyCookie(Constants.UserTokenCookie, null)
                {
                    Expires = DateTime.Now.AddDays(-1)
                });

                return response;
            };

            Get["/register"] = _ =>
            {
                if (Context.CurrentUser != null)
                {
                    return Response.AsRedirect("~/");
                }

                return View["register"];
            };

            Post["/create"] = _ =>
            {
                if (Context.CurrentUser != null)
                {
                    return Response.AsRedirect("~/");
                }

                string username = Request.Form.username;
                string email = Request.Form.email;
                string password = Request.Form.password;
                string confirmPassword = Request.Form.confirmPassword;

                if (String.IsNullOrEmpty(username))
                {
                    this.AddValidationError("username", LanguageResources.UsernameIsRequired);
                }

                if (String.IsNullOrEmpty(email))
                {
                    this.AddValidationError("email", LanguageResources.EmailIsRequired);
                }

                ValidatePassword(password, confirmPassword);

                try
                {
                    if (ModelValidationResult.IsValid)
                    {
                        ChatUser user = membershipService.AddUser(username, email, password);
                        return this.CompleteLogin(authenticationTokenService, user);
                    }
                }
                catch (Exception ex)
                {
                    this.AddValidationError("_FORM", ex.Message);
                }

                return View["register"];
            };

            Post["/unlink"] = param =>
            {
                if (Context.CurrentUser == null)
                {
                    return HttpStatusCode.Forbidden;
                }

                string provider = Request.Form.provider;
                ChatUser user = repository.GetUserById(Context.CurrentUser.UserName);

                if (user.Identities.Count == 1 && !user.HasUserNameAndPasswordCredentials())
                {
                    this.AddAlertMessage("error", LanguageResources.YouCannotUnlinkThisAccountBecauseYouWouldLoseYourAbilityToLogin);
                    return Response.AsRedirect("~/account/#identityProviders");
                }

                var identity = user.Identities.FirstOrDefault(i => i.ProviderName == provider);

                if (identity != null)
                {
                    repository.Remove(identity);

                    this.AddAlertMessage("success", String.Format(LanguageResources.SuccessfullyUnlinkedXAccount, provider));
                    return Response.AsRedirect("~/account/#identityProviders");
                }

                return HttpStatusCode.BadRequest;
            };

            Post["/newpassword"] = _ =>
            {
                if (Context.CurrentUser == null)
                {
                    return HttpStatusCode.Forbidden;
                }

                string password = Request.Form.password;
                string confirmPassword = Request.Form.confirmPassword;

                ValidatePassword(password, confirmPassword);

                ChatUser user = repository.GetUserById(Context.CurrentUser.UserName);

                try
                {
                    if (ModelValidationResult.IsValid)
                    {
                        membershipService.SetUserPassword(user, password);
                        repository.CommitChanges();
                    }
                }
                catch (Exception ex)
                {
                    this.AddValidationError("_FORM", ex.Message);
                }

                if (ModelValidationResult.IsValid)
                {
                    this.AddAlertMessage("success", LanguageResources.SuccessfullyAddedAPassword);
                    return Response.AsRedirect("~/account/#changePassword");
                }

                return GetProfileView(authService, user);
            };

            Post["/changepassword"] = _ =>
            {
                if (Context.CurrentUser == null)
                {
                    return HttpStatusCode.Forbidden;
                }

                string oldPassword = Request.Form.oldPassword;
                string password = Request.Form.password;
                string confirmPassword = Request.Form.confirmPassword;

                if (String.IsNullOrEmpty(oldPassword))
                {
                    this.AddValidationError("oldPassword", LanguageResources.OldPasswordIsRequired);
                }

                ValidatePassword(password, confirmPassword);

                ChatUser user = repository.GetUserById(Context.CurrentUser.UserName);

                try
                {
                    if (ModelValidationResult.IsValid)
                    {
                        membershipService.ChangeUserPassword(user, oldPassword, password);
                        repository.CommitChanges();
                    }
                }
                catch (Exception ex)
                {
                    this.AddValidationError("_FORM", ex.Message);
                }

                if (ModelValidationResult.IsValid)
                {
                    this.AddAlertMessage("success", LanguageResources.SuccessfullyChangedYourPassword);
                    return Response.AsRedirect("~/account/#changePassword");
                }

                return GetProfileView(authService, user);
            };

            Post["/changeusername"] = _ =>
            {
                if (Context.CurrentUser == null)
                {
                    return HttpStatusCode.Forbidden;
                }

                string username = Request.Form.username;
                string confirmUsername = Request.Form.confirmUsername;

                ValidateUsername(username, confirmUsername);

                ChatUser user = repository.GetUserById(Context.CurrentUser.UserName);
                string oldUsername = user.Name;

                try
                {
                    if (ModelValidationResult.IsValid)
                    {
                        membershipService.ChangeUserName(user, username);
                        repository.CommitChanges();
                    }
                }
                catch (Exception ex)
                {
                    this.AddValidationError("_FORM", ex.Message);
                }

                if (ModelValidationResult.IsValid)
                {
                    notificationService.OnUserNameChanged(user, oldUsername, username);

                    this.AddAlertMessage("success", LanguageResources.SuccessfullyChangedYourUsername);
                    return Response.AsRedirect("~/account/#changeUsername");
                }

                return GetProfileView(authService, user);
            };
        }

        private void ValidatePassword(string password, string confirmPassword)
        {
            if (String.IsNullOrEmpty(password))
            {
                this.AddValidationError("password", LanguageResources.PasswordIsRequired);
            }

            if (!String.Equals(password, confirmPassword))
            {
                this.AddValidationError("confirmPassword", LanguageResources.PasswordsDontMatch);
            }
        }

        private void ValidateUsername(string username, string confirmUsername)
        {
            if (String.IsNullOrEmpty(username))
            {
                this.AddValidationError("username", LanguageResources.UsernameIsRequired);
            }

            if (!String.Equals(username, confirmUsername))
            {
                this.AddValidationError("confirmUsername", LanguageResources.UsernamesDontMatch);
            }
        }

        private dynamic GetProfileView(IAuthenticationService authService, ChatUser user)
        {
            return View["index", new ProfilePageViewModel(user, authService.Providers)];
        }

        private LoginViewModel GetLoginViewModel(IApplicationSettings applicationSettings, 
                                                 IJabbrRepository repository,
                                                 IAuthenticationService authService)
        {
            ChatUser user = null;

            if (Context.CurrentUser != null)
            {
                user = repository.GetUserById(Context.CurrentUser.UserName);
            }

            var viewModel = new LoginViewModel(applicationSettings.AuthenticationMode,
                                               authService.Providers,
                                               user != null ? user.Identities : null);
            return viewModel;
        }
    }
}