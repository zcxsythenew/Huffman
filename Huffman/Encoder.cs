using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;

namespace Huffman
{
    public class Encoder
    {
        public async Task EncodeFile(StorageFile readFile, StorageFile writeFile)
        {
            byte[] output = new byte[4096];
            ulong readFileSize = (await readFile.GetBasicPropertiesAsync()).Size;
            ulong readSize = 0;

            HuffmanTree tree = new HuffmanTree();

            using (IInputStream fin = await readFile.OpenSequentialReadAsync())
            {
                using (DataReader dataReader = new DataReader(fin))
                {
                    using (IRandomAccessStream fout = await writeFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (IOutputStream outputStream = fout.GetOutputStreamAt(0))
                        {
                            using (DataWriter dataWriter = new DataWriter(outputStream))
                            {
                                int outputInitializedOffset = 0;
                                dataWriter.WriteUInt64((await readFile.GetBasicPropertiesAsync()).Size);

                                while (true)
                                {
                                    uint loadSize = await dataReader.LoadAsync(2048);
                                    readSize += loadSize;
                                    double percent = (double)readSize * 100 / readFileSize;
                                    ApplicationView.GetForCurrentView().Title = percent.ToString("0.0") + "%已完成";
                                    byte[] input = new byte[loadSize];
                                    dataReader.ReadBytes(input);

                                    int resultBitSize = EncodeBytes(input, loadSize, output, outputInitializedOffset, tree);

                                    dataWriter.WriteBytes(output.Take(resultBitSize / 8).ToArray());

                                    if (loadSize < 2048)
                                    {
                                        if (resultBitSize % 8 != 0)
                                        {
                                            dataWriter.WriteByte(output[resultBitSize / 8]);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        if (resultBitSize % 8 != 0)
                                        {
                                            output[0] = output[resultBitSize / 8];
                                            outputInitializedOffset = resultBitSize % 8;
                                        }
                                        else
                                        {
                                            outputInitializedOffset = 0;
                                        }
                                    }
                                }
                                await dataWriter.StoreAsync();
                                await outputStream.FlushAsync();
                            }
                        }
                    }
                }
            }
        }

        private int EncodeBytes(byte[] input, uint inputSize, byte[] output, int outputInitializedOffset, HuffmanTree tree)
        {
            int resultSize = outputInitializedOffset, code;
            for(int i = 0; i < inputSize; i++)
            {
                if(tree.Exists(input[i]))
                {
                    code = tree.GetCode(input[i], out int bitsCount);
                    for (int j = 0; j < bitsCount; j++)
                    {
                        SetBit(output, resultSize + j, GetBit(code, j));
                    }
                    resultSize += bitsCount;
                }
                else
                {
                    code = tree.GetCode(tree.newNode, out int bitsCount);
                    for (int j = 0; j < bitsCount; j++)
                    {
                        SetBit(output, resultSize + j, GetBit(code, j));
                    }
                    resultSize += bitsCount;
                    code = input[i];
                    bitsCount = 8;
                    for (int j = 0; j < bitsCount; j++)
                    {
                        SetBit(output, resultSize + j, GetBit(code, j));
                    }
                    resultSize += bitsCount;
                }
                tree.Add(input[i]);
            }
            return resultSize;
        }

        private bool GetBit(int input, int bitOffset)
        {
            return (input & (1 << bitOffset)) != 0;
        }

        private void SetBit(byte[] output, int bitOffset, bool bit)
        {
            int byteOffset = bitOffset / 8;
            
            bitOffset %= 8;
            if (bit)
            {
                output[byteOffset] |= (byte)(1 << bitOffset);
            }
            else
            {
                output[byteOffset] &= (byte)~(byte)(1 << bitOffset);
            }
        }
    }
}
