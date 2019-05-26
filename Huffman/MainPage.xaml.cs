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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Huffman
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            StorageFile inputFile = await HuffmanFileManager.GetFileToBeCompressed();
            if (inputFile == null)
            {
                tips.Text = "您取消了操作";
                goto finish;
            }

            tips.Text = "您需要将文件存放到哪里？";
            await Task.Delay(1000);
            StorageFile outputFile = await HuffmanFileManager.GetFileCompressedToSave(inputFile.DisplayName + ".wugzh3");
            if (outputFile == null)
            {
                tips.Text = "您取消了操作";
                goto finish;
            }

            tips.Text = "正在压缩\n请将程序保持在前台运行";
            await new Encoder().EncodeFile(inputFile, outputFile);
            tips.Text = "压缩完成";

            finish:
            await Task.Delay(1000);
            App.Current.Exit();
        }
    }
}
