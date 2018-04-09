﻿using System;
using CIM.RemoteManager.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Pages
{
    public class BasePage : ContentPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var viewModel = BindingContext as BaseViewModel;
            viewModel?.Resume();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var viewModel = BindingContext as BaseViewModel;
            viewModel?.Suspend();
        }

        protected IMvxMessenger MvxMessenger => Mvx.Resolve<IMvxMessenger>();

        protected MvxSubscriptionToken Subscribe<TMessage>(Action<TMessage> action)
            where TMessage : MvxMessage
        {
            return MvxMessenger.Subscribe<TMessage>(action, MvxReference.Weak);
        }

        protected void Unsubscribe<TMessage>(MvxSubscriptionToken id)
            where TMessage : MvxMessage
        {
            MvxMessenger.Unsubscribe<TMessage>(id);
        }

    }

}
