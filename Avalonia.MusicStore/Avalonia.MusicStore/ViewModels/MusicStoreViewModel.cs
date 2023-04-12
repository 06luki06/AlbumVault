using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.MusicStore.Models;
using ReactiveUI;

namespace Avalonia.MusicStore.ViewModels
{
	public class MusicStoreViewModel : ViewModelBase
	{

        private string? _searchText;
        public string? SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }

        private AlbumViewModel? _selectedAlbum;
        public AlbumViewModel? SelectedAlbum
		{
			get { return _selectedAlbum; }
			set { this.RaiseAndSetIfChanged(ref _selectedAlbum, value); }
		}

        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

        public ReactiveCommand<Unit, AlbumViewModel?> BuyMusicCommand { get; }

        public MusicStoreViewModel()
        {
            BuyMusicCommand = ReactiveCommand.Create(() =>
            {
                return SelectedAlbum;
            });

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch!);
		}

		private CancellationTokenSource? _cancellationTokenSource;

		private async void DoSearch(string s)
        {
            IsBusy = true;
            SearchResults.Clear();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            if(!string.IsNullOrWhiteSpace(s))
            {
                var albums = await Album.SearchAsync(s);

                foreach(var album in albums)
                {
                    var vm = new AlbumViewModel(album);
                    SearchResults.Add(vm);
                }

				if (!cancellationToken.IsCancellationRequested)
				{
					LoadCovers(cancellationToken);
				}
			}
            IsBusy = false;
        }

        private async void LoadCovers(CancellationToken cancellationToken)
        {
            foreach (var album in SearchResults.ToList())
            {
                await album.LoadCover();

                if(cancellationToken.IsCancellationRequested) return;
            }
        }
	}
}
