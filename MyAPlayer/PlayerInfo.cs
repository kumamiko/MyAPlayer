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
        private string album { get; set; }
        private Uri path { get; set; }
        private double width { get; set; }
        private string position { get; set; }
        private bool isNoRepeat { get; set; } = true;
        private bool isRepeatOne { get; set; } = false;
        private bool isRepeatAll { get; set; } = false;
        private bool isNotDownloading { get; set; }
        private bool isTopmost { get; set; }
        private bool isShowList { get; set; } = false;

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

        public string Album
        {
            get { return this.album; }
            set
            {
                if (this.album != value)
                {
                    this.album = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Album"));
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

        public bool IsNoRepeat
        {
            get { return this.isNoRepeat; }
            set
            {
                if (this.isNoRepeat != value)
                {
                    this.isNoRepeat = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsNoRepeat"));
                    }
                }
            }

        }

        public bool IsRepeatOne
        {
            get { return this.isRepeatOne; }
            set
            {
                if (this.isRepeatOne != value)
                {
                    this.isRepeatOne = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRepeatOne"));
                    }
                }
            }

        }

        public bool IsRepeatAll
        {
            get { return this.isRepeatAll; }
            set
            {
                if (this.isRepeatAll != value)
                {
                    this.isRepeatAll = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRepeatAll"));
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
