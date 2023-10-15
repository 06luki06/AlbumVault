using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AlbumVault.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;

namespace AlbumVault.Views;

public partial class MusicStoreWindow : ReactiveWindow<MusicStoreViewModel>
{
    public MusicStoreWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.BuyMusicCommand.Subscribe(Close)));
    }
}