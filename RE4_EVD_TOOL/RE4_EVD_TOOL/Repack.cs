using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SimpleEndianBinaryIO;
using SharedCode;

namespace RE4_EVD_TOOL
{
    internal static class Repack
    {
        public static void RepackFile(FileInfo fileInfo, Endianness endianness)
        {
            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var outputName = Path.Combine(diretory, name + ".evd");
            var headerName = Path.Combine(diretory, name, name + ".evdhdr");
            if (endianness == Endianness.BigEndian)
            {
                headerName = Path.Combine(diretory, name, name + ".evdhdrbig");
            }

            if (!File.Exists(headerName))
            {
                Console.WriteLine("Error the file does not exist: " + Path.GetFileName(headerName));
                return;
            }

            var br = new EndianBinaryReader(new FileInfo(headerName).OpenRead(), endianness);

            uint magic = br.ReadUInt32(Endianness.BigEndian);
            if (magic != 0x6576656E)
            {
                Console.WriteLine("Invalid file: " + Path.GetFileName(headerName));
                br.Close();
                return;
            }

            br.Position = 0;
            byte[] mainHeader = br.ReadBytes(0x40);

            uint Block1Offset = br.ReadUInt32();
            uint Block1Length = br.ReadUInt32();

            br.Position = Block1Offset;
            byte[] Block1 = br.ReadBytes((int)Block1Length);
            br.Close();

            string[] files = LoadIdx.LoadIdxevd(fileInfo);

            var bw = new EndianBinaryWriter(new FileInfo(outputName).Create(), endianness);

            bw.Write(mainHeader);
            bw.Write((uint)0x50);
            bw.Write(Block1Length);
            bw.Write((uint)files.Length);
            bw.Write((uint)Block1Length + 0x50);
            bw.Write(Block1);

            uint[] OffsetToOffset = new uint[files.Length];

            //block2
            for (int i = 0; i < files.Length; i++)
            {
                byte[] bName = Encoding.GetEncoding(1252).GetBytes(Utils.ValidFileName(files[i],i).Replace('&', '\\').Replace('$', '/'));
                bName = bName.Length > 0x2F ? bName.Take(0x2F).ToArray() : bName;
                byte[] insertName = new byte[0x30];
                bName.CopyTo(insertName, 0);
                bw.Write(insertName);
                OffsetToOffset[i] = (uint)bw.Position;
                bw.Write(new byte[0x10]);
            }

            // insert files
            int filesInserted = 0;
            uint nextOffset = (uint)bw.Position;
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = Path.Combine(diretory, name, files[i]);
                if (File.Exists(filePath))
                {
                    //verificação de tamanho do arquivo
                    FileInfo info = new FileInfo(filePath);
                    uint length = (uint)info.Length;
                    uint lines = length / 16;
                    uint rest = length % 16;
                    lines += rest != 0 ? 1u : 0u;
                    uint diff = (lines * 16) - length;

                    //escreve no arquivo de destino
                    bw.Position = OffsetToOffset[i];
                    bw.Write(nextOffset);
                    bw.Write((uint)info.Length);

                    bw.Position = nextOffset;
                    var fileStream = info.OpenRead();
                    fileStream.CopyTo(bw.BaseStream);
                    fileStream.Close();
                    bw.Write(new byte[diff]);

                    nextOffset = (uint)bw.Position;

                    Console.WriteLine("Inserted file: " + files[i]);
                    filesInserted++;
                }
                else
                {
                    Console.WriteLine("File \"" + files[i] + "\" does not exist, has not been added to the EVD");
                }

            }

            bw.Close();

            Console.WriteLine(filesInserted + "/" + files.Length + " files were inserted.");
        }

    }
}
