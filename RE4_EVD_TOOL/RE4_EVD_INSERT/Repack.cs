using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using SimpleEndianBinaryIO;
using SharedCode;

namespace RE4_EVD_INSERT
{
    internal static class Repack
    {
        public static void RepackFile(FileInfo fileInfo, Endianness endianness)
        {
            var diretory = Path.GetDirectoryName(fileInfo.FullName);
            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var EvdName = Path.Combine(diretory, name + ".evd");

            if (!File.Exists(EvdName))
            {
                Console.WriteLine("Error the file does not exist: " + Path.GetFileName(EvdName));
                return;
            }

            string[] filesToInsert = LoadIdx.LoadIdxevd(fileInfo);

            //evd file

            FileStream evd_stream = new FileStream(EvdName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            var br = new EndianBinaryReader(evd_stream, endianness);
            var bw = new EndianBinaryWriter(evd_stream, endianness);

            uint magic = br.ReadUInt32(Endianness.BigEndian);
            if (magic != 0x6576656E)
            {
                Console.WriteLine("Invalid file: " + Path.GetFileName(EvdName));
                evd_stream.Close();
                return;
            }

            br.Position = 0x48;
            uint Block2EntryCount = br.ReadUInt32();
            uint Block2Offset = br.ReadUInt32();

            List<(string Name, uint Offset, uint Length, long OffsetToOffset)> Files = new List<(string Name, uint Offset, uint Length, long OffsetToOffset)>();

            br.Position = Block2Offset;
            for (int i = 0; i < Block2EntryCount; i++)
            {
                byte[] bname = br.ReadBytes(0x30);
                long offsetToOffset = br.Position;
                uint dataOffset = br.ReadUInt32();
                uint dataLength = br.ReadUInt32();
                br.ReadUInt64(); //padding
                string finalName = Utils.ValidFileName(Encoding.GetEncoding(1252).GetString(bname).Replace('\\', '&').Replace('/', '$'), i);
                Files.Add((finalName, dataOffset, dataLength, offsetToOffset));
            }

            //use diff
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

            // Insert files

            int filesInserted = 0;
            foreach (var item in Files)
            {
                // Passo 1, verificar se o arquivo deve ser inserido no EVD.
                if (! filesToInsert.Contains(item.Name)) // negativa
                {
                    continue; // Não inserir esse arquivo.
                }

                string filePath = Path.Combine(diretory, name, item.Name);
                if (File.Exists(filePath)) // Verifica se o arquivo existe.
                {
                    // Verificação de tamanho do arquivo.
                    FileInfo info = new FileInfo(filePath);
                    uint length = (uint)info.Length;
                    uint lines = length / 16;
                    uint rest = length % 16;
                    lines += rest != 0 ? 1u : 0u;
                    uint diff = (lines * 16) - length;

                    // Verifica se dá para sobrepor o arquivo ou se tem que colocar no final.
                    bool Overwrite = false;
                    if (length <= item.Length)
                    {
                        Overwrite = true;
                    }
                    // Verifica se o arquivo está no final, então também pode ser sobrescrito.
                    if (item.Offset + item.Length + 1 >= bw.Length)
                    {
                        Overwrite = true;
                    }

                    // novo offset
                    long new_OffsetToOffset = bw.Length;
                    {
                        uint bw_length = (uint)bw.Length;
                        uint bw_lines = bw_length / 16;
                        uint bw_rest = bw_length % 16;
                        bw_lines += bw_rest != 0 ? 1u : 0u;
                        new_OffsetToOffset = (bw_lines * 16);
                    }
                    if (Overwrite)
                    {
                        new_OffsetToOffset = item.Offset;
                    }

                    //escreve no arquivo de destino
                    bw.Position = item.OffsetToOffset;
                    bw.Write((uint)new_OffsetToOffset);
                    bw.Write((uint)info.Length);

                    bw.Position = new_OffsetToOffset;
                    var fileStream = info.OpenRead();
                    fileStream.CopyTo(bw.BaseStream);
                    fileStream.Close();
                    bw.Write(new byte[diff]);

                    if (Overwrite)
                    {
                        Console.WriteLine("Inserted file (Overwrite): " + item.Name);
                    }
                    else 
                    {
                        Console.WriteLine("Inserted file (End of file): " + item.Name);
                    }
                  
                    filesInserted++;
                }
                else
                {
                    Console.WriteLine("File \"" + item.Name + "\" does not exist, has not been inserted to the EVD");
                }

            }

            bw.Close();

            Console.WriteLine(filesInserted + "/" + filesToInsert.Length + " files were inserted.");
        }

    }
}
