using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAPlayer
{
    public class PlayerInfo : INotifyPropertyChanged
    {
        private string title { get; set; }
        private string artist { get; set; }
        private Uri path { get; set; }
        private double width { get; set; }
        private string position { get; set; }
        private bool isNotDownloading { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
