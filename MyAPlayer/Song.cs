using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAPlayer
{
    public class Song : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string title { get; set; }
        public string artist { get; set; }
        public string lyric { get; set; }
        public string path { get; set; }
        public string apid { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
