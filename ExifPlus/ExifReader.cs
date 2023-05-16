using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

namespace ExifPlus
{
    /// <summary>
    /// Reads Exif Data from a Jpeg Stream.
    /// </summary>
    public class ExifReader
    {
        private bool littleEndian = false;
        private ExifInfo info;

        /// <summary>
        /// Gets the Exif Data from the Jpeg Stream.
        /// </summary>
        public static ExifInfo ReadJpeg(Stream stream)
        {
            DateTime start = DateTime.Now;
            ExifInfo info = new ExifInfo();
            try
            {
                info = new ExifReader(stream).info;
            }
            catch { /* swallow errors */ }
            info.FileSize = (uint)stream.Length;
            info.LoadTime = DateTime.Now - start;
            return info;
        }

        protected ExifReader(Stream stream)
        {
            this.info = new ExifInfo();
            // make sure we are at the start of the stream.
            stream.Position = 0;
            // get first byte
            int read = stream.ReadByte();
            // check that the bytes are as expected
            if (read != JpegId.START && stream.ReadByte() != JpegId.SOI) return;
            this.info.IsValid = true;
            
            // read
            while (true)
            {
                int lastMarker = 0, marker = 0;
                // find real start position
                while (true)
                {
                    marker = stream.ReadByte();
                    if (marker != JpegId.START && lastMarker == JpegId.START)
                    {
                        break;
                    }
                    lastMarker = marker;
                }

                // get the length of the section
                int lenPart1 = stream.ReadByte();
                int lenPart2 = stream.ReadByte();
                int len = lenPart1 << 8 | lenPart2;

                // initialize section data, put length at start
                byte[] section = new byte[len];
                section[0] = (byte)lenPart1;
                section[1] = (byte)lenPart2;

                // read the section bytes
                int sizeCheck = stream.Read(section, 2, len - 2);
                if (sizeCheck != len - 2)
                {
                    break; // wrong size
                }

                bool stop = false;

                // lots of fall-through that needs to be there
                switch (marker)
                {
                    case 192:
                    case 193:
                    case 194:
                    case 195:
                    case 197:
                    case 198:
                    case 199:
                    case 201:
                    case 202:
                    case 203:
                    case 205:
                    case 206:
                    case JpegId.SOF:
                        ProcessSOF(section, marker);
                        break;
                    case 196:
                    case 200:
                    case 204:
                    case 208:
                    case 209:
                    case 210:
                    case 211:
                    case 212:
                    case 213:
                    case 214:
                    case 215:
                    case JpegId.SOI:
                        break;
                    case JpegId.EOI:
                        stop = true;
                        break;
                    case JpegId.SOS:
                        stop = true;
                        break;
                    default:
                        {
                            if (marker == JpegId.EXIF)
                            {
                                if (section[2] == 69 && section[3] == 120 && section[4] == 105 && section[5] == 102)
                                {
                                    ProcessExif(section);
                                }
                            }
                            break;
                        }
                }
                // check for exit loop
                if (stop) break;
            }

        }

        /// <summary>
        /// Process the start of frame.
        /// Gets the Width, Height and Color info.
        /// </summary>
        private void ProcessSOF(byte[] section, int marker)
        {
            this.info.Height = ((uint)section[3] << 8 | (uint)section[4]);
            this.info.Width = ((uint)section[5] << 8 | (uint)section[6]);
            this.info.IsColor = (section[7] == 3);
        }

        private void ProcessExif(byte[] section)
        {
            if (section[6] == 0 && section[7] == 0)
            {
                // endian check
                if (section[8] == 73 && section[9] == 73)
                {
                    this.littleEndian = true;
                }
                else if (section[8] == 77 && section[9] == 77)
                { 
                    this.littleEndian = false;
                }
                else
                {
                    return;
                }
                // read exif type
                int length = ExifIO.ReadUShort(section, 10, this.littleEndian);
                if (length == 42)
                {
                    length = ExifIO.ReadInt(section, 12, this.littleEndian);
                    if (length < 8 || length > 16)
                    {
                        if (length < 16 || length > section.Length - 16)
                        {
                            return;
                        }
                    }
                    this.ProcessExifDir(section, length + 8, 8, section.Length - 8, 0, Exif.ExifIFD.Exif);
                }
            }
        }

        private void ProcessExifDir(byte[] section, int offsetDir, int offsetBase, int length, int depth, Exif.ExifIFD ifd)
        {
            // depth check
            if (depth > 4) return;
 
            // get size info
            int num = (int)ExifIO.ReadUShort(section, offsetDir, this.littleEndian);
            int mainDirOffset = DirOffset(offsetDir, num);

            // section size check
            if (mainDirOffset >= offsetDir + length) return;

            // loop through directories
            int dirOffset;
            for (int i = 0; i < (int)num; i++)
            { 
                dirOffset = this.DirOffset(offsetDir, i);
                ExifTag exifTag = new ExifTag(section, dirOffset, offsetBase, length, this.littleEndian);
                System.Diagnostics.Debug.WriteLine(exifTag.ToString());
                
                if (exifTag.IsValid)
                {
                    int tag = exifTag.Tag;
                    if (tag != (int)Exif.ExifIFD.Exif)
                    {
                        if (tag != (int)Exif.ExifIFD.Gps)
                        {
                            try
                            {
                                // process normal tag
                                exifTag.Populate(this.info, ifd);
                            }
                            catch { /* throw away error */ }
                        }
                        else
                        {
                            // process GPS tag
                            try
                            {
                                int subDirOffset = offsetBase + exifTag.GetInt(0);
                                if (subDirOffset <= offsetBase + length)
                                {
                                    ProcessExifDir(section, subDirOffset, offsetBase, length, depth + 1, Exif.ExifIFD.Gps);
                                }
                            }
                            catch { /* throw away error */ }
                        }
                    }
                    else
                    {
                        // this was the a main Exif tag, go deeper
                        int subDirOffset = offsetBase + exifTag.GetInt(0);
                        if (subDirOffset <= offsetBase + length)
                        {
                            this.ProcessExifDir(section, subDirOffset, offsetBase, length, depth + 1, Exif.ExifIFD.Exif);
                        }
                    }
                }
            }
            // check to see if there is more to read
            dirOffset = mainDirOffset + 4;
            if (dirOffset <= offsetBase + length)
            {
                dirOffset = ExifIO.ReadInt(section, mainDirOffset, this.littleEndian);
                if (dirOffset > 0)
                {
                    int subDirOffset = offsetBase + dirOffset;
                    if (subDirOffset <= offsetBase + length && subDirOffset >= offsetBase)
                    {
                        this.ProcessExifDir(section, subDirOffset, offsetBase, length, depth + 1, ifd);
                    }
                }
            }
            // read thumbnail information
            if (this.info.ThumbnailData == null && this.info.ThumbnailOffset > 0 && this.info.ThumbnailSize > 0)
            {
                this.info.ThumbnailData = new byte[this.info.ThumbnailSize];
                Array.Copy(section, offsetBase + this.info.ThumbnailOffset, this.info.ThumbnailData, 0, this.info.ThumbnailSize);
            }
        }

        private int DirOffset(int start, int num)
        {
            return start + 2 + 12 * num;
        }

    }
}
