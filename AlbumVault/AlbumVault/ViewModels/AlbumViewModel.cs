using Avalonia.Media.Imaging;
using AlbumVault.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlbumVault.Models;

namespace AlbumVault.ViewModels
{
	public class AlbumViewModel : ViewModelBase
	{
		private readonly Album _album;

		public string Artist { get { return _album.Artist; } }
		public string Title { get { return _album.Title; } }

        private Bitmap? _cover;
        public Bitmap? Cover
        {
            get { return _cover; }
            set { this.RaiseAndSetIfChanged(ref _cover, value); }
        }
        public AlbumViewModel(Album album)
        {
            _album = album;   
        }

        public async Task LoadCover()
        {
            using(var imageStream = await _album.LoadCoverBitmapAsync())
            {
                Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
            }
        }

        public async Task SaveToDiskAsync()
        {
            await _album.SaveAsync();

            if(Cover != null)
            {
                var bitmap = Cover;

                await Task.Run(() =>
                {
                    using (var fs = _album.SaveCoverBitmapStream())
                    {
                        bitmap.Save(fs);
                    }
                });
            }
        }
    }
}
