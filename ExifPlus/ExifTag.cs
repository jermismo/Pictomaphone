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

using ExifPlus.ExifData;

namespace ExifPlus
{
    public class ExifTag
    {
        private static int[] bytesPerFormat = new [] {
			0, 1, 1, 2, 4, 8, 1, 1, 2, 4, 8, 4, 8
		};

        /// <summary>
        /// The Tag ID
        /// </summary>
        public int Tag { get; private set; }

        /// <summary>
        /// The Tag Format
        /// </summary>
        public Exif.ExifTagFormat Format { get; private set; }

        /// <summary>
        /// The Component Count
        /// </summary>
        public int Components { get; private set; }

        /// <summary>
        /// The Byte Data
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// If the data is little endian
        /// </summary>
        public bool LittleEndian { get; private set; }

        /// <summary>
        /// If the data is valid
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// If the data is numeric
        /// </summary>
        public bool IsNumeric
        {
            get
            {
                Exif.ExifTagFormat format = this.Format;
                return (format != Exif.ExifTagFormat.STRING && format != Exif.ExifTagFormat.UNDEFINED);
            }
        }

        /// <summary>
        /// Creates a new instance of the Exif Tag class.
        /// </summary>
        public ExifTag(byte[] section, int sectionOffset, int offsetBase, int length, bool littleEndian)
        {
            this.IsValid = false;
            this.Tag = (int)ExifIO.ReadUShort(section, sectionOffset, littleEndian);

            int tagFormat = (int)ExifIO.ReadUShort(section, sectionOffset + 2, littleEndian);
            if (tagFormat >= 1 && tagFormat <= 12)
            {
                this.Format = (Exif.ExifTagFormat)tagFormat;
                this.Components = ExifIO.ReadInt(section, sectionOffset + 4, littleEndian);
                if (this.Components < 65536)
                {
                    this.LittleEndian = littleEndian;
                    int size = this.Components * ExifTag.bytesPerFormat[tagFormat];
                    int sourceIndex = 0;
                    if (size > 4)
                    {
                        int startIndex = ExifIO.ReadInt(section, sectionOffset + 8, littleEndian);
                        if (startIndex + size > length)
                        {
                            return;
                        }
                        sourceIndex = offsetBase + startIndex;
                    }
                    else
                    {
                        sourceIndex = sectionOffset + 8;
                    }
                    this.Data = new byte[size];
                    Array.Copy(section, sourceIndex, this.Data, 0, size);
                    this.IsValid = true;
                }
            }
        }

        /// <summary>
        /// Reads the numeric value from the component index and returns it as an int.
        /// </summary>
        public int GetInt(int componentIndex)
        {
            return (int)this.GetNumericValue(componentIndex);
        }

        /// <summary>
        /// Reads a numeric value from the component index. Returns it as a double, regardless of original type.
        /// </summary>
        public double GetNumericValue(int componentIndex)
        {
            double ret = 0.0;
            try
            {
                switch (this.Format)
                {
                    case Exif.ExifTagFormat.BYTE: return (double)this.Data[componentIndex];
                    case Exif.ExifTagFormat.USHORT: return (double)this.ReadUShort(componentIndex * 2);
                    case Exif.ExifTagFormat.ULONG: return this.ReadUInt(componentIndex * 4);
                    case Exif.ExifTagFormat.URATIONAL: return this.ReadUInt(componentIndex * 8) / this.ReadUInt(componentIndex * 8 + 4);
                    case Exif.ExifTagFormat.SBYTE: return (double)((sbyte)this.Data[componentIndex]);
                    case Exif.ExifTagFormat.SSHORT: return (double)this.ReadShort(componentIndex * 2);
                    case Exif.ExifTagFormat.SLONG: return (double)this.ReadInt(componentIndex * 4);
                    case Exif.ExifTagFormat.SRATIONAL: return (double)this.ReadInt(componentIndex * 8) / (double)this.ReadInt(componentIndex * 8 + 4);
                    case Exif.ExifTagFormat.SINGLE: return (double)this.ReadSingle(componentIndex * 4);
                    case Exif.ExifTagFormat.DOUBLE: return this.ReadDouble(componentIndex * 8);
                }
            }
            catch
            {
                ret = 0.0;
            }
            return ret;
        }

        /// <summary>
        /// Reads a string value.
        /// </summary>
        public string GetStringValue()
        {
            return GetStringValue(0);
        }
        
        /// <summary>
        /// Reads a string value from the component index.
        /// </summary>
        public string GetStringValue(int componentIndex)
        {
            Exif.ExifTagFormat format = this.Format;
            if (format != Exif.ExifTagFormat.STRING)
            {
                if (format == Exif.ExifTagFormat.URATIONAL)
                {
                    uint num = this.ReadUInt(componentIndex * 8);
                    uint num2 = this.ReadUInt(componentIndex * 8 + 4);
                    return num.ToString() + "/" + num2.ToString();
                }
                else if (format == Exif.ExifTagFormat.SRATIONAL)
                {
                    int num = this.ReadInt(componentIndex * 8);
                    int num2 = this.ReadInt(componentIndex * 8 + 4);
                    return num.ToString() + "/" + num2.ToString();
                }
                else if (format != Exif.ExifTagFormat.UNDEFINED)
                {
                    return this.GetNumericValue(componentIndex).ToString();
                }
            }

            // read the string
            string val = System.Text.Encoding.UTF8.GetString(this.Data, 0, this.Data.Length);
            char[] array = new char[5];
            array[0] = ' ';
            array[1] = '\t';
            array[2] = '\r';
            array[3] = '\n';
            return val.Trim(array);
        }

        /// <summary>
        /// Populates this tag's data correct field in the ExifInfo.
        /// </summary>
        public virtual void Populate(ExifInfo info, Exif.ExifIFD ifd)
        {
            if (ifd == Exif.ExifIFD.Exif)
            {
                PopulateExif(info);
            }
            else if (ifd == Exif.ExifIFD.Gps)
            {
                PopulateGps(info);
            }
        }

        #region * Private Methods *

        private short ReadShort(int offset)
        {
            return ExifIO.ReadShort(this.Data, offset, this.LittleEndian);
        }

        private ushort ReadUShort(int offset)
        {
            return ExifIO.ReadUShort(this.Data, offset, this.LittleEndian);
        }

        private int ReadInt(int offset)
        {
            return ExifIO.ReadInt(this.Data, offset, this.LittleEndian);
        }

        private uint ReadUInt(int offset)
        {
            return ExifIO.ReadUInt(this.Data, offset, this.LittleEndian);
        }

        private float ReadSingle(int offset)
        {
            return ExifIO.ReadSingle(this.Data, offset, this.LittleEndian);
        }

        private double ReadDouble(int offset)
        {
            return ExifIO.ReadDouble(this.Data, offset, this.LittleEndian);
        }

        private DateTime GetDateValue()
        {
            string dt = GetStringValue();
            if (!string.IsNullOrWhiteSpace(dt) && dt.Length == 19)
            {
                int year, month, day, hour, minute, second;

                int.TryParse(dt.Substring(0, 4), out year);
                int.TryParse(dt.Substring(5, 2), out month);
                int.TryParse(dt.Substring(8, 2), out day);

                int.TryParse(dt.Substring(11, 2), out hour);
                int.TryParse(dt.Substring(14, 2), out minute);
                int.TryParse(dt.Substring(17, 2), out second);

                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        #endregion

        /// <summary>
        /// Gets the Exif tag information.
        /// </summary>
        /// <param name="info"></param>
        private void PopulateExif(ExifInfo info)
        {
            Exif.ExifId tag = (Exif.ExifId)this.Tag;
            ExifDataBase exifData = ExifDataBase.GetData(this.Format, this.Data, 0, this.LittleEndian);

            switch (tag)
            {
                case Exif.ExifId.ProcessingSoftware: info.ProcessingSoftware = (ExifString)exifData; return;
                case Exif.ExifId.BitsPerSample: info.BitsPerSample = (ExifUShort)exifData; return;
                case Exif.ExifId.Description: info.Description = (ExifString)exifData; return;
                case Exif.ExifId.Make: info.Make = (ExifString)exifData; return;
                case Exif.ExifId.Model: info.Model = (ExifString)exifData; return;
                case Exif.ExifId.Orientation: info.Orientation = (Exif.Orientation)this.GetInt(0); return;
                case Exif.ExifId.XResolution: info.XResolution = (ExifURational)exifData; return;
                case Exif.ExifId.YResolution: info.YResolution = (ExifURational)exifData; return;
                case Exif.ExifId.ResolutionUnit: info.ResolutionUnit = (Exif.Unit)this.GetInt(0); return;
                case Exif.ExifId.Software: info.Software = (ExifString)exifData; return;
                case Exif.ExifId.ModifyDate: info.ModifiedDate = ExifDateTime.FromExifString((ExifString)exifData); return;
                case Exif.ExifId.ThumbnailOffset: info.ThumbnailOffset = this.GetInt(0); return;
                case Exif.ExifId.ThumbnailLength: info.ThumbnailSize = this.GetInt(0); return;
                case Exif.ExifId.Artist: info.Artist = (ExifString)exifData; return;
                case Exif.ExifId.Copyright: info.Copyright = (ExifString)exifData; return;
                case Exif.ExifId.ExposureTime: info.ExposureTime = (ExifURational)exifData; return;
                case Exif.ExifId.FNumber: info.FStop = (ExifURational)exifData; return;
                case Exif.ExifId.Iso: info.Iso = (ExifUShort)exifData; return;
                case Exif.ExifId.FlashUsed: info.Flash = (Exif.Flash)this.GetInt(0); return;
                case Exif.ExifId.LightSource: info.LightSource = (Exif.LightSource)this.GetInt(0); return;
                case Exif.ExifId.UserComment: info.UserComment = (ExifString)exifData; return;
                default: return;
            }
        }

        /// <summary>
        /// Gets the GPS info for this Tag.
        /// </summary>
        private void PopulateGps(ExifInfo info)
        {
            Exif.Gps tag = (Exif.Gps)this.Tag;
            // note: split this into 2 groups to speed up reading data
            if (this.Tag <= 3)
            {

                if (tag == Exif.Gps.LatitudeRef)
                {
                    // Latitude Reference
                    string latRef = this.GetStringValue();
                    if (latRef == "S") info.Latitude.Reference = Exif.GpsLatitudeRef.South;
                    else if (latRef == "N") info.Latitude.Reference = Exif.GpsLatitudeRef.North;
                    else info.Latitude.Reference = Exif.GpsLatitudeRef.Unknown;
                }
                else if (tag == Exif.Gps.Latitude)
                {
                    // Latitude
                    if (this.Components != 3) return;
                    info.Latitude.Hours = this.GetNumericValue(0);
                    info.Latitude.Minutes = this.GetNumericValue(1);
                    info.Latitude.Seconds = this.GetNumericValue(2);
                }
                else if (tag == Exif.Gps.LongitudeRef)
                {
                    // Longitude Reference
                    string lonRef = this.GetStringValue();
                    if (lonRef == "W") info.Longitude.Reference = Exif.GpsLongitudeRef.West;
                    else if (lonRef == "E") info.Longitude.Reference = Exif.GpsLongitudeRef.East;
                    else info.Longitude.Reference = Exif.GpsLongitudeRef.Unknown;
                }
            }
            else
            {
                if (tag == Exif.Gps.Longitude)
                {
                    // Longitude
                    if (this.Components != 3) return;
                    info.Longitude.Hours = this.GetNumericValue(0);
                    info.Longitude.Minutes = this.GetNumericValue(1);
                    info.Longitude.Seconds = this.GetNumericValue(2);
                }
                else if (tag == Exif.Gps.AltitudeRef)
                {
                    info.GpsAboveSeaLevel = (this.GetInt(0) == 0);
                }
                else if (tag == Exif.Gps.Altitude)
                {
                    info.GpsAltitude = this.GetNumericValue(0);
                }
            }
        }

        /// <summary>
        /// Returns data as string, in format:
        /// 0x0000-{TagName}: (data, ...)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
            sb.Append("0x").Append(this.Tag.ToString("X4"));
            sb.Append("-");
            sb.Append(((Exif.ExifId)this.Tag).ToString());
            if (this.Components > 0)
            {
                sb.Append(": (");
                sb.Append(this.GetStringValue(0));
                if (this.Format != Exif.ExifTagFormat.UNDEFINED && this.Format != Exif.ExifTagFormat.STRING)
                {
                    for (int i = 1; i < this.Components; i++)
                    {
                        sb.Append(", " + this.GetStringValue(i));
                    }
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

    }
}
