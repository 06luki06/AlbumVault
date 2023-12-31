﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using AlbumVault.Models;
using ReactiveUI;

namespace AlbumVault.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{

		public ICommand BuyMusic_Command { get; }
		public Interaction<MusicStoreViewModel, AlbumViewModel?> ShowDialog { get; }

		private bool _collectionEmpty;
		public bool CollectionEmpty
		{
			get { return _collectionEmpty; }
			set { this.RaiseAndSetIfChanged(ref _collectionEmpty, value); }
		}

		public ObservableCollection<AlbumViewModel> Albums { get; } = new();

		public MainWindowViewModel()
		{
			ShowDialog = new Interaction<MusicStoreViewModel, AlbumViewModel?>();
			RxApp.MainThreadScheduler.Schedule(LoadAlbums);

			BuyMusic_Command = ReactiveCommand.CreateFromTask(async () =>
			{
				var store = new MusicStoreViewModel();
				var result = await ShowDialog.Handle(store);
				if (result != null)
				{
					Albums.Add(result);
					await result.SaveToDiskAsync();
				}
			});

			this.WhenAnyValue(x => x.Albums.Count)
				.Subscribe(x => CollectionEmpty = x == 0);
		}

		private async void LoadAlbums()
		{
			var albums = (await Album.LoadCachedAsync()).Select(x => new AlbumViewModel(x));

			foreach (var album in albums)
			{
				Albums.Add(album);
			}

			foreach (var album in Albums.ToList())
			{
				await album.LoadCover();
			}
		}
	}
}