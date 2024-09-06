using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistQuickAdd.Models
{
    public class Track
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public ImageSource AlbumCover { get; set; }
        public string ID { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Track()
        {
            
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
