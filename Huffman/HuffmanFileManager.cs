using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Huffman
{
    public static class HuffmanFileManager
    {
        public static async Task<StorageFile> GetFileToBeCompressed()
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                CommitButtonText = "压缩",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List,
                
            };
            picker.FileTypeFilter.Add("*");
            StorageFile file = await picker.PickSingleFileAsync();
            return file;
        }

        public static async Task<StorageFile> GetFileToBeDecompressed()
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                CommitButtonText = "解压",
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            picker.FileTypeFilter.Add(".wugzh3");
            StorageFile file = await picker.PickSingleFileAsync();
            return file;
        }

        public static async Task<StorageFile> GetFileCompressedToSave(string suggestedName)
        {
            FileSavePicker picker = new FileSavePicker
            {
                CommitButtonText = "保存",
                SuggestedFileName = suggestedName,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeChoices.Add("压缩文件", new List<string> { ".wugzh3" });
            StorageFile file = await picker.PickSaveFileAsync();
            return file;
        }

        public static async Task<StorageFile> GetFileDecompressedToSave(string suggestedName)
        {
            FileSavePicker picker = new FileSavePicker
            {
                CommitButtonText = "保存",
                SuggestedFileName = suggestedName,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeChoices.Add("文件", new List<string> { ".file" });
            StorageFile file = await picker.PickSaveFileAsync();
            return file;
        }
    }
}
