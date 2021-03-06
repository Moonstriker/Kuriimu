﻿using System.Runtime.InteropServices;
using Kuriimu.Kontract;
using System.IO;
using Kuriimu.IO;
using Kuriimu.Compression;

namespace archive_zlib
{
    public class ZLIBFileInfo : ArchiveFileInfo
    {
        public uint decompSize;

        public override Stream FileData
        {
            get
            {
                return new MemoryStream(ZLib.Decompress(base.FileData));
            }
        }

        public override long? FileSize => decompSize;

        public void Write(Stream input)
        {
            using (var bw = new BinaryWriterX(input, true, ByteOrder.BigEndian))
                if (State == ArchiveFileState.Archived)
                {
                    bw.Write(decompSize);
                    base.FileData.CopyTo(bw.BaseStream);
                }
                else
                {
                    bw.Write((uint)base.FileData.Length);
                    bw.Write(ZLib.Compress(base.FileData));
                }
        }
    }
}
