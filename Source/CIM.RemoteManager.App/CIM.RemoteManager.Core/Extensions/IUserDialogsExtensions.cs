﻿using System;
using System.Drawing;
using Acr.UserDialogs;

namespace CIM.RemoteManager.Core.Extensions
{
    public static class IUserDialogsExtensions
    {
        public static IDisposable ErrorToast(this IUserDialogs dialogs, string title, string message, TimeSpan duration)
        {
            return dialogs.Toast(new ToastConfig($" {message}") { BackgroundColor = Color.Red, Duration = duration });
        }
        public static IDisposable InfoToast(this IUserDialogs dialogs, string message, TimeSpan duration)
        {
            return dialogs.Toast(new ToastConfig($" {message}") { BackgroundColor = Color.White, MessageTextColor = Color.DodgerBlue, Duration = duration });
        }
    }
}
