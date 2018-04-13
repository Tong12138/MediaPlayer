using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Storage.Streams;
using Windows.Media.Core;



// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MediaPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filepicker = new FileOpenPicker();
            filepicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            filepicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            filepicker.FileTypeFilter.Add(".mp3");
            filepicker.FileTypeFilter.Add(".mp4");
            Windows.Storage.StorageFile file = await filepicker.PickSingleFileAsync();
            if (file != null)
            {
                // Uri uri = new Uri(file.Path);
                //mediaplayer.Source = uri;
                filename.Text = file.Name;
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                mediaplayer.SetSource(stream, file.ContentType);
                //SystemMediaTransportControls _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            }
            else
            {
                filename.Text = "Error";
            }

        }

        private void OnButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri;
            if (textbox.Text=="")
            {
                 uri= new Uri("http://www.neu.edu.cn/indexsource/neusong.mp3");
            }
              else
                 uri = new Uri(textbox.Text);

            filename.Text = System.IO.Path.GetFileName(uri.LocalPath);
            mediaplayer.Source = uri;
        }

        private async void DowmButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri;
           if (textbox.Text == "")
           {
                uri = new Uri("http://www.neu.edu.cn/indexsource/neusong.mp3");
            }
            else
                uri = new Uri(textbox.Text);
            string Filename = System.IO.Path.GetFileName(uri.LocalPath);

            StorageFile file = await KnownFolders.MusicLibrary.CreateFileAsync(Filename,CreationCollisionOption.ReplaceExisting);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                var sourceStream= await httpResponse.Content.ReadAsInputStreamAsync();
                    using (var destinationStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (var destinationOutputStream = destinationStream.GetOutputStreamAt(0))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(sourceStream, destinationStream);
                        }
                    }

                var stream1 = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                mediaplayer.SetSource(stream1, file.ContentType);
                mediaplayer.Play();
                filename.Text = Filename;

            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

        }
    }



}
