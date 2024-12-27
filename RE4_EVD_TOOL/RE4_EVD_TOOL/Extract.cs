using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_EVD_TOOL
{
    internal static class Extract
    {
        public static void ExtractFile(FileInfo fileInfo, Endianness endianness, bool useDiff)
        {
            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);

            var br = new EndianBinaryReader(fileInfo.OpenRead(), endianness);

            uint magic = br.ReadUInt32(Endianness.BigEndian);
            if (magic != 0x6576656E)
            {
                Console.WriteLine("invalid file!");
                br.Close();
                return;
            }

            br.Position = 0;
            byte[] mainHeader = br.ReadBytes(0x40);

            uint Block1Offset = br.ReadUInt32();
            uint Block1Length = br.ReadUInt32();
            uint Block2EntryCount = br.ReadUInt32();
            uint Block2Offset = br.ReadUInt32();

            br.Position = Block1Offset;
            byte[] Block1 = br.ReadBytes((int)Block1Length);

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

            if (useDiff)
            {
                //usa a diferença entre os offsets.
                var orderedOffset = Files.Select(x => x.Offset).OrderBy(x => x).ToArray();

                for (int i = 0; i < Files.Count; i++)
                {
                    uint nextOffset = (uint)br.Length;
                    foreach (var offset in orderedOffset)
                    {
                        if (offset < nextOffset && offset > Files[i].Offset)
                        {
                            nextOffset = offset;
                        }
                    }

                    var o = Files[i];
                    o.Length = nextOffset - Files[i].Offset;
                    Files[i] = o;
                }

            }

            var baseDirectory = Path.Combine(diretory, name);
            Directory.CreateDirectory(baseDirectory);

            var idx = new FileInfo(Path.Combine(diretory, name + ".idxevd")).CreateText();
            idx.WriteLine("# RE4_EVD_TOOL");
            idx.WriteLine("# by: JADERLINK");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine("# github.com/JADERLINK");
            idx.WriteLine("");
            foreach (var item in Files)
            {
                idx.WriteLine(item.Name);
            }
            idx.Close();

            string headerName = Path.Combine(diretory, name, name + ".evdhdr");
            if (endianness == Endianness.BigEndian)
            {
                headerName = Path.Combine(diretory, name, name + ".evdhdrbig");
            }

            var header = new EndianBinaryWriter(new FileInfo(headerName).Create(), endianness);
            header.Write(mainHeader);
            header.Write((uint)0x50);
            header.Write((uint)Block1.Length);
            header.Write((uint)0x0);
            header.Write((uint)0x0);
            header.Write(Block1);
            header.Close();

            foreach (var item in Files)
            {
                br.Position = item.Offset;
                var arr = br.ReadBytes((int)item.Length);
                File.WriteAllBytes(Path.Combine(diretory, name, item.Name), arr);
            }

            br.Close();

            Console.WriteLine(Files.Count + " files were extracted.");
        }


    }
}
