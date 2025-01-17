using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;
using SharedCode;

namespace RE4_EVD_INSERT
{
    internal static class INSERT_IN_EVD
    {
        public static void INSERT_IN_EVD_FILE(FileInfo fileInfo, Endianness endianness) 
        {
            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);

            var br = new EndianBinaryReader(fileInfo.OpenRead(), endianness);

            uint magic = br.ReadUInt32(Endianness.BigEndian);
            if (magic != 0x6576656E)
            {
                Console.WriteLine("Invalid file!");
                br.Close();
                return;
            }

            br.Position = 0x48;
            uint Block2EntryCount = br.ReadUInt32();
            uint Block2Offset = br.ReadUInt32();

            List<(string Name, uint Offset, uint Length)> Files = new List<(string Name, uint Offset, uint Length)>();

            br.Position = Block2Offset;
            for (int i = 0; i < Block2EntryCount; i++)
            {
                byte[] bname = br.ReadBytes(0x30);
                uint dataOffset = br.ReadUInt32();
                uint dataLength = br.ReadUInt32();
                br.ReadUInt64(); //padding
                string finalName = Utils.ValidFileName(Encoding.GetEncoding(1252).GetString(bname).Replace('\\', '&').Replace('/', '$'), i);
                Files.Add((finalName, dataOffset, dataLength));
            }


            var idx = new FileInfo(Path.Combine(diretory, name + ".INSERT_IN_EVD")).CreateText();
            idx.WriteLine("# RE4_EVD_INSERT");
            idx.WriteLine("# by: JADERLINK");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine("# github.com/JADERLINK");
            idx.WriteLine("");
            idx.WriteLine("# remove the # to insert the file into the evd:");
            foreach (var item in Files)
            {
                idx.WriteLine("# "+ item.Name);
            }
            idx.Close();

        }

    }
}
