using System;
using System.Drawing;
using Acr.UserDialogs;

namespace CIM.RemoteManager.Core.Extensions
{
    /// <summary>
    /// Class IUserDialogsExtensions.
    /// </summary>
    public static class IUserDialogsExtensions
    {
        /// <summary>
        /// Error toast message.
        /// </summary>
        /// <param name="dialogs">The dialogs.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        public static IDisposable ErrorToast(this IUserDialogs dialogs, string title, string message, TimeSpan duration)
        {
            return dialogs.Toast(new ToastConfig($" {message}") { BackgroundColor = Color.FromArgb(255, 42, 0), Duration = duration });
        }

        /// <summary>
        /// Information toast message.
        /// </summary>
        /// <param name="dialogs">The dialogs.</param>
        /// <param name="message">The message.</param>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        public static IDisposable InfoToast(this IUserDialogs dialogs, string message, TimeSpan duration)
        {
            return dialogs.Toast(new ToastConfig($" {message}") { BackgroundColor = Color.White, MessageTextColor = Color.FromArgb(31, 74, 114), Duration = duration });
        }
    }
}
