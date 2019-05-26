using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Huffman
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FileActivatedPage : Page
    {
        StorageFile inputFile = null;

        public FileActivatedPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            inputFile = e.Parameter as StorageFile;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFile outputFile = null;
            await Task.Delay(1000);
            if(inputFile != null)
            {
                outputFile = await HuffmanFileManager.GetFileDecompressedToSave(inputFile.DisplayName);
            }
            else
            {
                tips.Text = "程序发生异常";
                goto finish;
            }
            if (outputFile == null)
            {
                tips.Text = "您取消了操作";
                goto finish;
            }

            tips.Text = "正在解压\n请将程序保持在前台运行";
            await new Decoder().DecodeFile(inputFile, outputFile);
            tips.Text = "解压完成";

        finish:
            await Task.Delay(1000);
            App.Current.Exit();
        }
    }
}
