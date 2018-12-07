using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using Newtonsoft.Json;
using Digimezzo.Utilities.Utils;
using System.Text.RegularExpressions;
using MyAPlayer.NeteaseService;

namespace MyAPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum RepeatType
        {
            NoRepeat,
            RepeatOne,
            RepeatAll
        }

        public bool IsPlaying = false;
        public bool IsSearching = false;
        public bool haveLyric = false;
        public bool IsApiUsing = false;
        public bool IsShowLrc = false;
        public string iniPath = AppDomain.CurrentDomain.BaseDirectory + "config.ini";
        public string songPath = string.Empty;
        public string savePath = string.Empty;
        public RepeatType currentRepeatType = RepeatType.NoRepeat;
        public int songIndex = 0;
        public int page = 0;
        public int downloadtimeout = 10;
        public WebClient client = new WebClient();
        public BitmapImage bi;
        public string timeFormat = "mm:ss";
        public Lrc lrc = new Lrc();
        public string lrcTemp = string.Empty;
        public string currentLrc = string.Empty;
        public string downloadingFilePath = string.Empty;
        public About singletonAbout = null;
        public LyricWindow lyricWindow = new LyricWindow();
        private ObservableCollection<Song> mySongList = new ObservableCollection<Song>();

        public PlayerInfo playerInfo = new PlayerInfo
        {
            Title = "",
            Artist = "",
            Width = 0,
            Position = "00:00",
            IsNotDownloading = true
        };

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        public delegate void ShowLrc(string lrc);
        public event ShowLrc DoShowLrc;

        public delegate void ShowMsg(string msg);
        public event ShowMsg DoShowMsg;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listViewSongList.ItemsSource = mySongList;
            //绑定控件
            this.DataContext = playerInfo;
            txtTitle.DataContext = playerInfo;
            txtSinger.DataContext = playerInfo;
            media.DataContext = playerInfo;
            txtCurrentSeconds.DataContext = playerInfo;
            btnDownload.DataContext = playerInfo;
            gridList.DataContext = playerInfo;
            //下载完成提示
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(MyDownloadComplete);

            ContentControl.SlideDirection = Digimezzo.WPFControls.Enums.SlideDirection.DownToUp;

            StartHistoryPlay();

            //歌词窗口
            if (IsShowLrc) this.lyricWindow.Show();
            this.lyricWindow.ShowInTaskbar = false;
            this.lyricWindow.Topmost = true;
            //委托
            this.DoShowLrc += (lrc) => lyricWindow.ShowLyc(lrc);
            DoShowMsg += (msg) => mcMsg.ShowMsg(msg);

            //下载超时时间
            if (downloadtimeout < 10) {
                downloadtimeout = 10;
                DoShowMsg("自定义下载超时时间过短 已恢复为10秒");
            } 
            downloadTimer.Tick += new EventHandler(downloadTimer_Tick);
            downloadTimer.Interval = TimeSpan.FromSeconds(downloadtimeout);
           
        }

        public void StartHistoryPlay()
        {
            try
            {
                Loadini();
                if (Directory.Exists(songPath))
                {
                    mySongList.Clear();
                    Task.Run(() => GetAllFilePath(songPath));
                }
            }
            catch (Exception)
            {
            }
        }

        #region 读写设置
        /// <summary>    
        /// 读取INI文件    
        /// </summary>    
        /// <param name="section">项目名称(如 [section] )</param>    
        /// <param name="skey">键</param>   
        /// <param name="path">路径</param> 
        public string IniReadValue(string section, string skey, string path)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(section, skey, "", temp, 500, path);
            return temp.ToString();
        }


        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="section">项目名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="path">路径</param>
        public void IniWrite(string section, string key, string value, string path)
        {
            WritePrivateProfileString(section, key, value, path);
        }

        /// <summary>
        /// 初始化，载入设置
        /// </summary>
        private void Loadini()
        {
            try { songPath = IniReadValue("info", "songPath", iniPath); } catch { }
            try { savePath = IniReadValue("info", "savePath", iniPath); } catch { }
            try { songIndex = int.Parse(IniReadValue("info", "songIndex", iniPath)); } catch { }
            try { this.Top = int.Parse(IniReadValue("info", "posY", iniPath)); } catch { }
            try { this.Left = int.Parse(IniReadValue("info", "posX", iniPath)); } catch { }
            try { this.lyricWindow.Left = int.Parse(IniReadValue("info", "lrcposX", iniPath)); } catch { }
            try { this.lyricWindow.Top = int.Parse(IniReadValue("info", "lrcposY", iniPath)); } catch { }
            try { this.lyricWindow.Width = double.Parse(IniReadValue("info", "lrcwidth", iniPath)); } catch { }
            try { IsShowLrc = bool.Parse(IniReadValue("info", "isShowLrc", iniPath)); } catch { }
            try { media.Volume = double.Parse(IniReadValue("info", "volume", iniPath)); } catch { }
            try { playerInfo.IsTopmost = this.Topmost = bool.Parse(IniReadValue("info", "topmost", iniPath)); } catch { }
            try { playerInfo.IsShowList = bool.Parse(IniReadValue("info", "isShowList", iniPath)); } catch { }
            try { downloadtimeout = int.Parse(IniReadValue("info", "downloadtimeout", iniPath)); } catch { }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void Saveini()
        {
            IniWrite("info", "songPath", songPath, iniPath);
            IniWrite("info", "savePath", savePath, iniPath);
            IniWrite("info", "songIndex", listViewSongList.SelectedIndex.ToString(), iniPath);
            IniWrite("info", "posX", this.Left.ToString(), iniPath);
            IniWrite("info", "posY", this.Top.ToString(), iniPath);
            IniWrite("info", "lrcposX", this.lyricWindow.Left.ToString(), iniPath);
            IniWrite("info", "lrcposY", this.lyricWindow.Top.ToString(), iniPath);
            IniWrite("info", "lrcwidth", this.lyricWindow.Width.ToString(), iniPath);
            IniWrite("info", "volume", media.Volume.ToString("f1"), iniPath);
            IniWrite("info", "topmost", this.Topmost.ToString(), iniPath);
            IniWrite("info", "isShowList", playerInfo.IsShowList.ToString(), iniPath);
            IniWrite("info", "isShowLrc", IsShowLrc.ToString(), iniPath);
            IniWrite("info", "downloadtimeout", downloadtimeout.ToString(), iniPath);
        }
        #endregion

        #region 窗口拖动
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        #endregion

        #region 播放暂停动画
        private async void btnPlayOrPause_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPlaying)
            {
                media.Play();
                IsPlaying = true;
                btn_background.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/img_pause.png", UriKind.Absolute));
                await SlideAndScale(btnPlayOrPause, this.ActualWidth, 100, 1, 0.6, 18, 45);
            }
            else
            {
                if (media.CanPause) media.Pause(); else return;
                IsPlaying = false;
                btn_background.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/img_play.png", UriKind.Absolute));
                await SlideAndScale(btnPlayOrPause, this.ActualWidth, 100, 0.6, 1, 45, 18);
            }
        }

        public static async Task SlideAndScale(Button btn, double width, float milliSeconds, double scaleFrom, double scaleTo, double topleftFrom, double topleftTo)
        {
            var sb = new Storyboard();

            sb.AddScale(milliSeconds, scaleFrom, scaleTo, scaleFrom, scaleTo);
            sb.AddCanvasSlide(milliSeconds, topleftFrom, topleftTo, topleftFrom, topleftTo);

            sb.Begin(btn);

            btn.Visibility = Visibility.Visible;

            await Task.Delay((int)milliSeconds);
        }
        #endregion

        //private void rootGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    MediaOpen();
        //}

        /// <summary>
        /// 打开本地音乐
        /// </summary>
        private void MediaOpen()
        {

            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "mp3|*.mp3";
            if (file.ShowDialog() == true)
            {
                haveLyric = false;
                IsApiUsing = false;

                //清空旧歌词,显示空
                lrc.LrcWord.Clear();
                DoShowLrc("");

                if (File.Exists(file.FileName))
                {
                    songPath = System.IO.Path.GetDirectoryName(file.FileName);
                    mySongList.Clear();
                    GetAllFilePath(songPath);
                    if (mySongList.Count > 0)
                    {
                        //保存路径
                        IniWrite("info", "songPath", songPath, iniPath);

                        var item = mySongList.First(s => s.path == file.FileName);
                        var index = mySongList.IndexOf(item);
                        listViewSongList.SelectedIndex = index;
                    }
                }
            }
        }


        //获取当前文件夹所有路径
        public void GetAllFilePath(string directoryPath)
        {
            // 访问当前目录所有子文件
            DirectoryInfo currentDir = new DirectoryInfo(directoryPath);
            FileInfo[] files = currentDir.GetFiles();
            for (int i = 0, j = 1; i < files.Length; i++)
            {
                if (files[i].Extension == ".mp3")
                {
                    var songinfo = GetSongInfoFromPath(files[i].FullName);
                    this.Dispatcher.Invoke(() => mySongList.Add(new Song { id = j++, title = songinfo.title, artist = songinfo.artist, path = files[i].FullName }));
                }
            }

            this.Dispatcher.Invoke(() =>
            {
                if (mySongList.Count > songIndex)
                {
                    listViewSongList.SelectedIndex = songIndex;
                }
            });
        }

        /// <summary>
        /// 获取MP3的歌曲名或文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Song GetSongInfoFromPath(string path)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(path);
                var title = string.Empty;
                var artist = string.Empty;

                if (file.Tag.Title != null && file.Tag.Title.Length > 0)
                    title = file.Tag.Title;
                else
                    title = System.IO.Path.GetFileName(path);

                if (file.Tag.JoinedPerformers != null && file.Tag.JoinedPerformers.Length > 0)
                    artist = string.Join("/", file.Tag.JoinedPerformers);
                else
                    artist = "未知";

                return new Song
                {
                    title = title,
                    artist = artist
                };
            }
            catch
            {
                return new Song
                {
                    title = System.IO.Path.GetFileName(path),
                    artist = "未知"
                };
            }
        }

        public void PlayAndChangeInfo(string str)
        {
            media.Stop();
            dispatcherTimer.Stop();
            dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
            playerInfo.Width = 0;
            playerInfo.Position = "00:00";

            playerInfo.Title = mySongList[listViewSongList.SelectedIndex].title;
            playerInfo.Artist = mySongList[listViewSongList.SelectedIndex].artist;
            playerInfo.Path = new Uri(mySongList[listViewSongList.SelectedIndex].path);
            IsPlaying = false;
            btnPlayOrPause_Click(btnPlayOrPause, null);
        }

        public double MaxLength = 0;
        public double CurrentLentgh = 0;
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer(); // 定义一个DT
        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            //更改颜色
            Task.Run(() => InterpolateAccentColorAsync(ImageUtils.GetDominantColor(ImageToByte(bi))));

            playerInfo.Width = 0;
            playerInfo.Position = "00:00";
            if (media.NaturalDuration.HasTimeSpan)
            {
                var ts = media.NaturalDuration.TimeSpan;
                MaxLength = ts.TotalSeconds;  //设置slider最大值
                txtTotalSeconds.Text = TimeSpanToDateTime(ts);
            }
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick); //超过计时间隔时发生
            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.1); //DT间隔
            dispatcherTimer.Start(); //DT启动
        }

        private async void InterpolateAccentColorAsync(Color accentColor)
        {
            Color InterpolateColor(Color from, Color to, double progress)
            {
                return from + ((to - from) * (float)progress);
            }

            await Task.Run(async () =>
            {
                int loop = 0;
                int loopMax = 30;
                var oldColor = (Color)Application.Current.Resources["Color_Accent"];

                while (loop < loopMax)
                {
                    loop++;
                    Color color = InterpolateColor(oldColor, accentColor, loop / (double)loopMax);
                    Application.Current.Resources["Color_Accent"] = color;
                    Application.Current.Resources["Brush_Accent"] = new SolidColorBrush(color);
                    await Task.Delay(7);
                }
            });
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            playerInfo.Position = TimeSpanToDateTime(media.Position);
            playerInfo.Width = media.Position.TotalSeconds / MaxLength * 284; //slider滑动值随播放内容位置变化

            var currentPosition = media.Position;
            var currentlrc = this.lrc;
            if (haveLyric && IsShowLrc)
                Task.Run(() => FreshLyric(currentPosition, currentlrc));

        }

        public void AlbumSlideEffect()
        {
            //var border = new Border();
            //await border.Dispatcher.InvokeAsync(() => border.Background = new ImageBrush(bi)).Task;
            //border.Background = new ImageBrush(bi);
            //ContentControl.Content = border;
            ContentControl.Content = new Border() { Background = new ImageBrush(bi) };
        }

        public static string TimeSpanToDateTime(TimeSpan timeSpan, string format = "mm:ss")
        {
            try
            {
                DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                DateTime ret = start.Add(timeSpan);
                return ret.ToString(format);
            }
            catch (Exception)
            {
                return "00:00";
            }
        }

        #region 功能按钮
        private void btnVolume_Click(object sender, RoutedEventArgs e)
        {
            if (media.Volume != 0)
            {
                media.Volume = 0;
                faUnMute.Visibility = Visibility.Collapsed;
                faMute.Visibility = Visibility.Visible;
            }
            else
            {
                media.Volume = 0.5;
                faUnMute.Visibility = Visibility.Visible;
                faMute.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (IsSearching)
            {
                btnClearText.Visibility = Visibility.Collapsed;
                await StoryBoardHelpers.ElementMargin(new Thickness(0), new Thickness(300, 0, 0, 0), 300, txtSearch);
                IsSearching = false;
            }
            else
            {
                //txtSearch.Text = string.Empty;
                await StoryBoardHelpers.ElementMargin(new Thickness(300, 0, 0, 0), new Thickness(0), 300, txtSearch);
                IsSearching = true;
                btnClearText.Visibility = Visibility.Visible;
            }
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            playerInfo.IsShowList = !playerInfo.IsShowList;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Saveini();
            this.lyricWindow.Close();
            Application.Current.Shutdown();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            MediaOpen();
        }

        private void btnLyric_Click(object sender, RoutedEventArgs e)
        {
            IsShowLrc = !IsShowLrc;
            if (IsShowLrc)
            {
                btnLyric.ToolTip = "关闭歌词";
                lyricWindow.Show();
            }
            else
            {
                btnLyric.ToolTip = "显示歌词";
                lyricWindow.Hide();
            }
        }
        #endregion

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    btnSearch_Click(null, null);
                    break;
                case Key.Left:
                    if (media.Position == TimeSpan.Zero) break;
                    if (media.Position <= TimeSpan.FromSeconds(5))
                    {
                        media.Position = TimeSpan.Zero;
                        playerInfo.Width = 0;
                        break;
                    }
                    media.Position -= TimeSpan.FromSeconds(5);
                    playerInfo.Width = media.Position.TotalSeconds / MaxLength * 284;
                    break;
                case Key.Up:
                    if (mySongList.Count > 0 && listViewSongList.SelectedIndex > 0)
                    {
                        playerInfo.Width = 0;
                        listViewSongList.SelectedIndex--;
                    }
                    break;
                case Key.Right:
                    if (media.Position == TimeSpan.Zero) break;
                    media.Position += TimeSpan.FromSeconds(5);
                    playerInfo.Width = media.Position.TotalSeconds / MaxLength * 284;
                    break;
                case Key.Down:
                    if (listViewSongList.SelectedIndex < mySongList.Count - 1)
                    {
                        playerInfo.Width = 0;
                        listViewSongList.SelectedIndex++;
                    }
                    break;
                case Key.Space:
                    btnPlayOrPause_Click(null, null);
                    break;
                case Key.Enter:
                    if (txtSearch.IsFocused)
                    {
                        if (!string.IsNullOrEmpty(txtSearch.Text))
                        {
                            mySongList.Clear();
                            page = 0;
                            GetSongList(page++);
                            IsApiUsing = true;
                        }
                    }
                    break;
            }
        }


        public void RandomColor()
        {
            int iSeed = 10;
            Random ro = new Random(iSeed);
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            int R = ran.Next(255);
            int G = ran.Next(255);
            int B = ran.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
            B = (B > 255) ? 255 : B;
            rec.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)R, (byte)G, (byte)B));
        }

        private void listViewSongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (mySongList.Count > 0)
                {
                    PlayAndChangeInfo(mySongList[listViewSongList.SelectedIndex].path);

                    Task.Factory.StartNew(() => ChangeCoverAndLyric());

                    playerInfo.Width = 0;
                    if (!IsApiUsing) songIndex = listViewSongList.SelectedIndex;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 更改封面和歌词
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeCoverAndLyric()
        {
            DispatchService.Invoke(async () =>
            {
                if (mySongList.Count > 0 && listViewSongList.SelectedIndex >= 0)
                {
                    if (IsApiUsing)
                    {
                        var path = GetCover(mySongList[listViewSongList.SelectedIndex].apid);
                        if (!string.IsNullOrEmpty(path))
                        {
                            path += "?param=500x500";
                            bi = await GetNewImageAsync(new Uri(path));
                        }
                        else
                        {
                            bi = new BitmapImage(new Uri("pack://application:,,,/images/default_cover.jpg", UriKind.Absolute));
                        }
                        //获取封面的同时更新歌手信息
                        playerInfo.Artist = mySongList[listViewSongList.SelectedIndex].artist;
                    }
                    else
                    {
                        string path = mySongList[listViewSongList.SelectedIndex].path;
                        string lrcPath = path.Replace(".mp3", ".lrc");

                        bi = GetImageFromMp3(path);
                        //尝试获取本地歌词并显示
                        if (File.Exists(lrcPath))
                        {
                            haveLyric = true;
                            lrc = Lrc.InitLrc(path.Replace(".mp3", ".lrc"));
                        }
                        else haveLyric = false;

                        DoShowLrc("");
                    }

                    AlbumSlideEffect();
                }
            });
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (Repeat()) return;

            if (listViewSongList.SelectedIndex + 1 < mySongList.Count)
            {
                listViewSongList.SelectedIndex++;
            }
            else
            {
                dispatcherTimer.Stop();
                dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
                playerInfo.Width = 0;
                playerInfo.Position = "00:00";
                btnPlayOrPause_Click(null, null);
                media.Close();
                //listViewSongList.SelectedIndex = -1;
            }
        }

        public void GetSongList(int page)
        {
            DispatchService.Invoke(() =>
            {
                if (string.IsNullOrEmpty(txtSearch.Text)) return;


                try
                {
                    var jsonString = MusicApi.Search(txtSearch.Text, 10, 10 * (page));
                    dynamic jsonObj = JsonConvert.DeserializeObject(jsonString);
                    var result = jsonObj.result;
                    var songlist = result.songs;
                    int j = 1;
                    foreach (var item in songlist)
                    {
                        var album = item.album;
                        var artist = album.artist;
                        DispatchService.Invoke(() =>
                        {
                            mySongList.Add(new Song
                            {
                                id = 10 * page + j++,
                                apid = item.id,
                                title = item.name,
                                path = @"http://music.163.com/song/media/outer/url?id=" + item.id + ".mp3"
                            });
                        });
                    }
                    //Listview回到顶端
                    if (mySongList.Count > 0)
                    {
                        listViewSongList.ScrollIntoView(listViewSongList.Items[0]);

                        if (!playerInfo.IsShowList) playerInfo.IsShowList = true;
                    }
                }
                catch (Exception)
                {
                }
            });
        }

        /// <summary>
        /// 获取本地MP3封面
        /// </summary>
        /// <param name="path"></param>
        public static BitmapImage GetImageFromMp3(string path)
        {
            BitmapImage bitmap = null;
            TagLib.File file = TagLib.File.Create(path);
            var pics = file.Tag.Pictures;

            // Load you image data in MemoryStream
            if (pics != null && pics.Length > 0)
            {
                TagLib.IPicture pic = pics[0];
                using (var stream = new MemoryStream(pic.Data.Data))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
                return bitmap;
            }
            else return new BitmapImage(new Uri("pack://application:,,,/images/default_cover.jpg", UriKind.Absolute));
        }

        /// <summary>
        /// 获取网易音乐图片
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> GetNewImageAsync(Uri uri)
        {
            BitmapImage bitmap = null;
            var httpClient = new HttpClient();

            using (var response = await httpClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = new MemoryStream())
                    {
                        await response.Content.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                }
            }

            return bitmap;
        }

        public string GetCover(string songId)
        {
            try
            {
                string detailjson = MusicApi.Detail(songId);
                if (string.IsNullOrEmpty(detailjson)) { haveLyric = false; return null; }
                var detailjsonObj = JsonConvert.DeserializeObject<dynamic>(detailjson);
                if (detailjsonObj == null) { haveLyric = false; return null; }
                var artists = detailjsonObj.songs[0].artists;
                var album = detailjsonObj.songs[0].album.name;
                string duration = detailjsonObj.songs[0].duration;
                if (duration != null)
                {
                    try
                    {
                        TimeSpan ts = TimeSpan.FromMilliseconds(double.Parse(duration));
                        MaxLength = ts.TotalSeconds;
                        txtTotalSeconds.Text = TimeSpanToDateTime(ts);
                    }
                    catch { }
                }

                if (album != null)
                {
                    playerInfo.Album = album;
                }

                //获取歌词
                string lyricjson = MusicApi.Lyric(songId);
                var lyricjsonObj = JsonConvert.DeserializeObject<dynamic>(lyricjson);
                string lyric = string.Empty;
                if (lyricjsonObj != null && lyricjsonObj.nolyric == null && lyricjsonObj.uncollected == null) lyric = lrcTemp = lyricjsonObj.lrc.lyric;
                else lyric = lrcTemp = string.Empty;

                List<string> listArtists = new List<string>();
                if (artists != null)
                {
                    foreach (var item in artists)
                    {
                        listArtists.Add((string)item.name);
                    }
                    string strArtists = string.Join("/", listArtists);
                    mySongList[listViewSongList.SelectedIndex].artist = strArtists;
                }
                else
                {
                    mySongList[listViewSongList.SelectedIndex].artist = "未知";
                }


                //mySongList[listViewSongList.SelectedIndex].lyric = lyric;

                if (!string.IsNullOrEmpty(lyric))
                {
                    //byte[] array = Lrc.ByteArrayCut(Encoding.Default.GetBytes(lyric), 0x3f);
                    //将默认编码转到Unicode
                    byte[] arraydefalut = Encoding.UTF8.GetBytes(lyric);
                    byte[] arrayunicode = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, arraydefalut);

                    MemoryStream stream = new MemoryStream(arrayunicode);
                    lrc = Lrc.InitLrc(stream);
                    haveLyric = true;
                    //有歌词
                    DoShowLrc("");
                }
                else
                {
                    haveLyric = false;
                    DoShowLrc("没有歌词");
                }

                return detailjsonObj.songs[0].album.picUrl;
            }
            catch (Exception)
            {
                haveLyric = false;
                DoShowLrc("歌词获取失败");

                return string.Empty;
            }
        }

        public static string EncodingConvert(Encoding src, Encoding desc, string srcString)
        {
            byte[] arraysrc = src.GetBytes(srcString);
            byte[] arraydesc = Encoding.Convert(src, desc, arraysrc);
            return desc.GetString(arraydesc);
        }

        //图片转byte[]   
        public static byte[] ImageToByte(BitmapImage imageSource)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageSource));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                return ms.ToArray();
            }
        }


        /// <summary>
        /// 刷新歌词
        /// </summary>
        /// <param name="ts"></param>
        public void FreshLyric(TimeSpan ts, Lrc lrc)
        {
            var lrcs = from n in lrc.LrcWord
                       where n.Key <= ts.TotalMilliseconds
                       select n.Value;

            if (lrcs.Count() > 0)
            {
                if (currentLrc == lrcs.Last())
                    return;
                else
                    currentLrc = lrcs.Last();

                if (DoShowLrc != null)
                    this.Dispatcher.Invoke(() => DoShowLrc(currentLrc));
            }
        }

        #region 下载文件
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">下载文件地址</param>
        /// <param name="Filename">下载后的存放地址</param>
        public void DownloadFile(Uri url, string filename)
        {

            if (File.Exists(filename))
            {
                DoShowMsg($"{txtTitle.Text} 文件已存在");
                return;
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            using (client)
            {
                client.DownloadFileAsync(url, filename);
                playerInfo.IsNotDownloading = false;
                DoShowMsg("开始下载");

                downloadTimer.Start();
            }
        }

        //作为下载歌曲的超时检测
        DispatcherTimer downloadTimer = new DispatcherTimer();
        private void downloadTimer_Tick(object sender, EventArgs e)
        {
            this.client.CancelAsync();
        }

        public void MyDownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            downloadTimer.Stop();

            if (e.Cancelled)
            {
                DoShowMsg("下载超时");
                playerInfo.IsNotDownloading = true;
                File.Delete(downloadingFilePath);
                return;
            }

            var filename = txtTitle.Text + ".mp3";
            if (File.Exists(downloadingFilePath))
            {
                if (new FileInfo(downloadingFilePath).Length > 0)
                {
                    //添加封面
                    AddCoverToMp3(downloadingFilePath);

                    DoShowMsg($"{txtTitle.Text} 下载完成");
                }
                else
                {
                    DoShowMsg("下载失败");
                    File.Delete(downloadingFilePath);
                }
            }
            else
            {
                DoShowMsg("下载失败");
            }
            playerInfo.IsNotDownloading = true;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsApiUsing)
                {

                    if (!Directory.Exists(savePath))
                    {
                        System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                        dialog.Description = "请选择下载路径";

                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            savePath = dialog.SelectedPath + "\\";
                        else return;

                    }

                    var filename = RemoveSpecialCharacter(txtTitle.Text) + ".mp3";
                    downloadingFilePath = savePath + filename;

                    //保存歌词到文件
                    if (haveLyric && lrcTemp.Length > 0)
                        Task.Run(() => SaveLrcToFile(downloadingFilePath, EncodingConvert(Encoding.UTF8, Encoding.Unicode, lrcTemp)));


                    DownloadFile(media.Source, downloadingFilePath);
                }
                else
                    DoShowMsg("正在播放本地音乐");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 去掉音乐标题的特殊字符，否则无法新建文件
        /// </summary>
        /// <param name="beforeFileName"></param>
        /// <returns></returns>
        public static string RemoveSpecialCharacter(string beforeFileName)
        {
            string reg = @"\:" + @"|\;" + @"|\/" + @"|\\" + @"|\|" + @"|\," + @"|\*" + @"|\?" + @"|\""" + @"|\<" + @"|\>";//特殊字符
            Regex r = new Regex(reg);
            return r.Replace(beforeFileName, "");//将特殊字符替换为""
        }

        /// <summary>
        /// 保存歌词到文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void SaveLrcToFile(string path, string lrc)
        {
            using (var sw = new StreamWriter(path.Replace(".mp3", ".lrc")))
            {

                sw.Write(lrc);
            }
        }
        #endregion

        #region MenuItem
        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            MediaOpen();
        }

        private void menuSearchMore_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => GetSongList(page++));
        }

        private void menuTopMost_Click(object sender, RoutedEventArgs e)
        {
            playerInfo.IsTopmost = this.Topmost = !this.Topmost;

        }

        private void menuChangeDownloadPath_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.Description = "请选择下载路径";

            if ((bool)dialog.ShowDialog(this))
                savePath = dialog.SelectedPath + "\\";
            else return;
        }

        private void menuAddCoverToFile_Click(object sender, RoutedEventArgs e)
        {
            if (AddCoverToMp3(downloadingFilePath))
            {
                DoShowMsg("已添加封面");
            }
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            if (singletonAbout == null)
            {
                singletonAbout = new About();
            }
            singletonAbout.Show();
            singletonAbout.Activate();
        }
        #endregion

        public bool AddCoverToMp3(string filePath)
        {
            try
            {
                TagLib.File file = TagLib.File.Create(filePath);
                SetAlbumArt(bi, file);
                file.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetAlbumArt(BitmapImage bi, TagLib.File file)
        {
            byte[] imageBytes;

            try
            {
                imageBytes = ImageToByte(bi);
                TagLib.Id3v2.AttachedPictureFrame cover = new TagLib.Id3v2.AttachedPictureFrame
                {
                    Type = TagLib.PictureType.FrontCover,
                    Description = "Cover",
                    MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                    Data = imageBytes,
                    TextEncoding = TagLib.StringType.UTF16
                };
                file.Tag.Pictures = new TagLib.IPicture[] { cover };
                file.Tag.Performers = new string[] { playerInfo.Artist ?? string.Empty };
                file.Tag.Title = playerInfo.Title;
                file.Tag.Album = playerInfo.Album;
            }
            catch { }
        }


        /// <summary>
        /// 清空搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearText_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
        }

        /// <summary>
        /// 调节音量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rootGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && this.media.Volume + 0.1 <= 1.0)
            {
                this.media.Volume += 0.1;
                DoShowMsg($"音量：{this.media.Volume * 100.0}%");

            }

            if (e.Delta < 0 && this.media.Volume - 0.1 >= 0.0)
            {
                this.media.Volume = (double)((int)(this.media.Volume * 10.0) - 1) / 10.0;
                if (this.media.Volume <= 0.0)
                {
                    DoShowMsg("  静音  ");
                }
                else
                {
                    DoShowMsg($"音量：{this.media.Volume * 100.0}%");
                }
            }
        }

        #region 循环
        private void MenuNoRepeat_Click(object sender, RoutedEventArgs e)
        {
            if (currentRepeatType == RepeatType.NoRepeat) return;
            currentRepeatType = RepeatType.NoRepeat;
            playerInfo.IsNoRepeat = true;
            playerInfo.IsRepeatOne = playerInfo.IsRepeatAll = false;
            DoShowMsg("  不循环  ");
        }

        private void MenuRepeatOne_Click(object sender, RoutedEventArgs e)
        {
            if (currentRepeatType == RepeatType.RepeatOne) return;
            currentRepeatType = RepeatType.RepeatOne;
            playerInfo.IsRepeatOne = true;
            playerInfo.IsNoRepeat = playerInfo.IsRepeatAll = false;
            DoShowMsg("  单曲循环  ");
        }

        private void MenuRepeatAll_Click(object sender, RoutedEventArgs e)
        {
            if (currentRepeatType == RepeatType.RepeatAll) return;
            currentRepeatType = RepeatType.RepeatAll;
            playerInfo.IsRepeatAll = true;
            playerInfo.IsNoRepeat = playerInfo.IsRepeatOne = false;
            DoShowMsg("  列表循环  ");
        }

        public bool Repeat()
        {
            bool ret = false;
            switch (currentRepeatType)
            {
                case RepeatType.RepeatOne:
                    media.Position = TimeSpan.FromSeconds(1);
                    ret = true;
                    break;
                case RepeatType.RepeatAll:
                    if (listViewSongList.SelectedIndex == listViewSongList.Items.Count - 1)
                    {
                        listViewSongList.SelectedIndex = 0;
                        ret = true;
                    }
                    break;
                case RepeatType.NoRepeat:
                default:
                    break;
            }
            return ret;
        }
        #endregion
    }
}
