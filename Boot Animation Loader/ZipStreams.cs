using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

/*
 * 只能使用於本專案中，此類別並未完整支持Zip 
 */
namespace Boot_Animation_Loader
{
    public class ZipStreams
    {
        private Stream filestream;

        private List<ZipFile> files = new List<ZipFile>();

        public ZipFile[] Files => files.ToArray();

        public ZipStreams(string FileName)
        {
            filestream = new FileStream(FileName, FileMode.Open);
            process();
        }

        public void process()
        {
            byte[] header = new byte[4];
            while(true)
            {
                filestream.Read(header, 0, header.Length);
                int header_ = BitConverter.ToInt32(header, 0);
                if(header_== 0x04034b50)
                {
                    BinaryReader reader = new BinaryReader(filestream);
                    byte[] Version = reader.ReadBytes(2);
                    byte[] bitflag = reader.ReadBytes(2);
                    byte[] method = reader.ReadBytes(2);
                    byte[] lastmodificationtime = reader.ReadBytes(2);
                    byte[] lastmodificationdate = reader.ReadBytes(2);
                    byte[] crc32 = reader.ReadBytes(4);
                    byte[] compressed = reader.ReadBytes(4);
                    byte[] uncompressed = reader.ReadBytes(4);
                    byte[] filenamelength = reader.ReadBytes(2);
                    byte[] extralength = reader.ReadBytes(2);
                    short file_length = BitConverter.ToInt16(filenamelength, 0);
                    short extra_length = BitConverter.ToInt16(extralength, 0);
                    byte[] fileName = reader.ReadBytes(file_length);
                    byte[] extraName = reader.ReadBytes(extra_length);
                    int compressedSize = BitConverter.ToInt32(compressed, 0);
                    int uncompressedSize = BitConverter.ToInt32(uncompressed, 0);
                    byte[] FileData = reader.ReadBytes(compressedSize);
                    ZipFileHeader z_header = new ZipFileHeader();
                    z_header.Location = new FileLocation(Encoding.UTF8.GetString(fileName));
                    z_header.CompressedSize = compressedSize;
                    z_header.CRC32 = String.Join("", Array.ConvertAll(crc32, x => Convert.ToString(x, 16)));
                    z_header.UncompressedSize = uncompressedSize;
                    ZipFile zipFile = new ZipFile() { Data=FileData, Header = z_header};
                    if(z_header.Location.FileName !="")files.Add(zipFile);
                }
                else if (header_ == 0x06054b50|| header_== 0x02014b50) break;
            }
            filestream.Close();
        }

        public ZipFile this[string FileName]=>Array.Find(Files,x=>x.Header.Location.FileName==FileName);

        public ZipFile[] GetFiles(string Directory)
        {
            ZipFile[] a = Array.FindAll(this.Files, x => x.Header.Location.DirectoryName == Directory);
            Array.Sort(a);
            return a;
        }

        public bool Exist(string FileName)
        {
            return Array.Exists(Files, x => x.Header.Location.FileName == FileName);
        }

        public void Dispose()
        {
            foreach(ZipFile zf in files)
            {
                zf.Data = null;
            }
            files.Clear();
        }
    }

    public class FileLocation
    {
        public string DirectoryName { get; set; }

        public string FileName { get; set; }

        public FileLocation(string Filename)
        {
            string filename = Filename ?? "";
            string[] split = System.Text.RegularExpressions.Regex.Split(filename, "/");
            this.FileName = split[split.Count() - 1];
            string[] directory_array = new string[split.Count() - 1];
            Array.Copy(split, directory_array, directory_array.Count());
            DirectoryName = String.Join("/", directory_array);
        }
    }
    public struct ZipFileHeader
    {
        public FileLocation Location;
        public string CRC32;
        public int CompressedSize;
        public int UncompressedSize;
    }
    

    public class ZipFile: IComparable, IDisposable
    {
        public ZipFileHeader Header { get; set; }
        public byte[] Data { get; set; }
        public ZipFile()
        {
            Data = default(byte[]);
        }
        public int CompareTo(object obj)
        {
            ZipFile zf = obj as ZipFile;
            return this.Header.Location.FileName.CompareTo(zf.Header.Location.FileName);
        }

        public void Dispose()
        {
            
             Data = null;
            GC.Collect();
        }

    }

    
}
