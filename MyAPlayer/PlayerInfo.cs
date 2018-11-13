using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MyAPlayer
{
    public class PlayerInfo : INotifyPropertyChanged
    {
        private string title { get; set; }
        private string artist { get; set; }
        private Uri path { get; set; }
        private double width { get; set; }
        private string position { get; set; }
        private SolidColorBrush currentColor { get; set; }
        private bool isNotDownloading { get; set; }
        private bool isTopmost { get; set; }
        private bool isShowList { get; set; } = false;
        private bool isShowLyric { get; set; } = false;

        public string Title
        {
            get { return this.title; }
            set
            {
                if (this.title != value)           //如果当前的变量值不等于先前的文件名，说明需要更新文件名
                {
                    this.title = value;                                          //更新文件名
                    if (PropertyChanged != null)                                    //如果已经触发了改变事件
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }

        }

        public string Artist
        {
            get { return this.artist; }
            set
            {
                if (this.artist != value)         
                {
                    this.artist = value;                                       
                    if (PropertyChanged != null)  
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Artist"));
                    }
                }
            }

        }

        public Uri Path
        {
            get { return this.path; }
            set
            {
                if (this.path != value)          
                {
                    this.path = value;  
                    if (PropertyChanged != null) 
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Path"));
                    }
                }
            }

        }

        public double Width
        {
            get { return this.width; }
            set
            {
                if (this.width != value)  
                {
                    this.width = value;        
                    if (PropertyChanged != null) 
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Width"));
                    }
                }
            }

        }

        public string Position
        {
            get { return this.position; }
            set
            {
                if (this.position != value)  
                {
                    this.position = value;              
                    if (PropertyChanged != null)  
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Position"));
                    }
                }
            }

        }

        public SolidColorBrush CurrentColor
        {
            get { return this.currentColor; }
            set
            {
                if (this.currentColor != value)
                {
                    this.currentColor = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("CurrentColor"));
                    }
                }
            }

        }

        public bool IsNotDownloading
        {
            get { return this.isNotDownloading; }
            set
            {
                if (this.isNotDownloading != value)           
                {
                    this.isNotDownloading = value;  
                    if (PropertyChanged != null)  
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsNotDownloading"));
                    }
                }
            }

        }

        public bool IsTopmost
        {
            get { return this.isTopmost; }
            set
            {
                if (this.isTopmost != value)
                {
                    this.isTopmost = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsTopmost"));
                    }
                }
            }

        }

        public bool IsShowList
        {
            get { return this.isShowList; }
            set
            {
                if (this.isShowList != value)
                {
                    this.isShowList = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsShowList"));
                    }
                }
            }

        }

        public bool IsShowLyric
        {
            get { return this.isShowLyric; }
            set
            {
                if (this.isShowLyric != value)
                {
                    this.isShowLyric = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsShowLyric"));
                    }
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
