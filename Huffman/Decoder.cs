using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;

namespace Huffman
{
    public class Decoder
    {
        public async Task DecodeFile(StorageFile readFile, StorageFile writeFile)
        {
            byte[] output = new byte[8192];
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
                                uint unDecodedBitSize = 0;
                                await dataReader.LoadAsync(sizeof(ulong));
                                ulong fileSize = dataReader.ReadUInt64();

                                byte[] lastNotDecoded = new byte[4];

                                while (true)
                                {
                                    uint loadSize = await dataReader.LoadAsync(1024);
                                    readSize += loadSize;
                                    double percent = (double)readSize * 100 / readFileSize;
                                    ApplicationView.GetForCurrentView().Title = percent.ToString("0.0") + "%已完成";
                                    byte[] input = new byte[loadSize];
                                    dataReader.ReadBytes(input);

                                    uint resultSize;

                                    if (loadSize < 1024)
                                    {
                                        resultSize = DecodeBytes(lastNotDecoded.Concat(input).ToArray(), loadSize * 8 + 32, 32 - unDecodedBitSize, out unDecodedBitSize, output, tree, true);
                                        dataWriter.WriteBytes(output.Take((int)fileSize).ToArray());
                                        break;
                                    }
                                    else
                                    {
                                        resultSize = DecodeBytes(lastNotDecoded.Concat(input).ToArray(), loadSize * 8 + 32, 32 - unDecodedBitSize, out unDecodedBitSize, output, tree, false);
                                        fileSize -= resultSize;
                                        lastNotDecoded[0] = input[loadSize - 4];
                                        lastNotDecoded[1] = input[loadSize - 3];
                                        lastNotDecoded[2] = input[loadSize - 2];
                                        lastNotDecoded[3] = input[loadSize - 1];
                                        dataWriter.WriteBytes(output.Take((int)resultSize).ToArray());
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

        private uint DecodeBytes(byte[] input, uint inputBitSize, uint pointer, out uint unDecodedBitSize, byte[] output, HuffmanTree tree, bool ignore32)
        {
            uint counter = 0;
            byte character;
            int reserved = ignore32 ? 0 : 32;
            while (pointer < inputBitSize - reserved)
            {
                character = DecodeByte(input, pointer, tree, out bool isNew, out uint bitLength);
                pointer += bitLength;
                if (isNew)
                {
                    character = GetByte(input, pointer);
                    pointer += 8;
                }
                output[counter] = character;
                tree.Add(character);
                counter++;
            }
            unDecodedBitSize = inputBitSize - pointer;
            return counter;
        }

        private byte DecodeByte(byte[] input, uint bitOffset, HuffmanTree tree, out bool isNew, out uint bitLength)
        {
            int code = 0;
            for(uint i = 0; i < 32 && bitOffset + i < input.Length * 8; i++)
            {
                code |= (GetBit(input, bitOffset + i) ? 1 : 0) << (int)i;
            }
            return tree.GetCharacter(code, out isNew, out bitLength);
        }

        private bool GetBit(byte[] input, uint bitOffset)
        {
            uint byteOffset = bitOffset / 8;
            bitOffset %= 8;
            return (input[byteOffset] & (1 << (int)bitOffset)) != 0;
        }

        private byte GetByte(byte[] input, uint bitOffset)
        {
            byte code = 0;
            for (byte i = 0; i < 8 && bitOffset + i < input.Length * 8; i++)
            {
                code |= (byte)((GetBit(input, bitOffset + i) ? 1 : 0) << i);
            }
            return code;
        }
    }
}
