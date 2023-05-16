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

namespace ExifPlus
{
    /// <summary>
    /// The Exif Data from the Jpeg.
    /// </summary>
    public class ExifInfo
    {
        /// <summary>
        /// The File Name of the Image.
        /// </summary>
        public ExifData.ExifString FileName = new ExifData.ExifString();
        /// <summary>
        /// The File Size in Bytes.
        /// </summary>
        public uint FileSize;
        /// <summary>
        /// If the Exif data could be read.
        /// </summary>
        public bool IsValid;
        /// <summary>
        /// Pixel Width of the image.
        /// </summary>
        public uint Width;
        /// <summary>
        /// Pixel height of the image.
        /// </summary>
        public uint Height;
        /// <summary>
        /// Number of bits per pixel.
        /// </summary>
        public ExifData.ExifUShort BitsPerSample = new ExifData.ExifUShort();
        /// <summary>
        /// If the image is in color.
        /// </summary>
        public bool IsColor;
        /// <summary>
        /// The orientation of the camera when the photo was taken.
        /// </summary>
        public Exif.Orientation Orientation;
        /// <summary>
        /// The Horizontal print resolution.
        /// </summary>
        public ExifData.ExifURational XResolution = new ExifData.ExifURational();
        /// <summary>
        /// The Vertical print resolution.
        /// </summary>
        public ExifData.ExifURational YResolution = new ExifData.ExifURational();
        /// <summary>
        /// The measurement unit for the x and y resolution.
        /// </summary>
        public Exif.Unit ResolutionUnit;
        /// <summary>
        /// The date/time when the image was created/modified
        /// </summary>
        public ExifData.ExifDateTime ModifiedDate = new ExifData.ExifDateTime();
        /// <summary>
        /// The image description
        /// </summary>
        public ExifData.ExifString Description = new ExifData.ExifString();
        /// <summary>
        /// The make of the camera
        /// </summary>
        public ExifData.ExifString Make = new ExifData.ExifString();
        /// <summary>
        /// The camera model
        /// </summary>
        public ExifData.ExifString Model = new ExifData.ExifString();
        /// <summary>
        /// The software used to create the image
        /// </summary>
        public ExifData.ExifString Software = new ExifData.ExifString();
        /// <summary>
        /// The artist who created the image
        /// </summary>
        public ExifData.ExifString Artist = new ExifData.ExifString();
        /// <summary>
        /// Any copyright information on the image
        /// </summary>
        public ExifData.ExifString Copyright = new ExifData.ExifString();
        /// <summary>
        /// A user comment
        /// </summary>
        public ExifData.ExifString UserComment = new ExifData.ExifString();
        /// <summary>
        /// Exposure time in seconds
        /// </summary>
        public ExifData.ExifURational ExposureTime = new ExifData.ExifURational();
        /// <summary>
        /// The F number
        /// </summary>
        public ExifData.ExifURational FStop = new ExifData.ExifURational();
        /// <summary>
        /// Info on Flash State
        /// </summary>
        public Exif.Flash Flash;
        /// <summary>
        /// Info on the Light Source
        /// </summary>
        public Exif.LightSource LightSource;
        /// <summary>
        /// The GPS Latitude Position
        /// </summary>
        public GpsLatitudePosition Latitude = new GpsLatitudePosition();
        /// <summary>
        /// The GPS Longitude Position
        /// </summary>
        public GpsLogitudePosition Longitude = new GpsLogitudePosition();
        /// <summary>
        /// If the Altitude is above sea-level.
        /// If false, the Altitude should be displayed as negative
        /// </summary>
        public bool GpsAboveSeaLevel;
        /// <summary>
        /// The GPS Altitude above sea level.
        /// </summary>
        public double GpsAltitude;
        /// <summary>
        /// The offset of the Thumbnail data
        /// </summary>
        public int ThumbnailOffset;
        /// <summary>
        /// The size of the Thumbnail in bytes
        /// </summary>
        public int ThumbnailSize;
        /// <summary>
        /// The bytes for the Thumbnail
        /// </summary>
        public byte[] ThumbnailData;
        /// <summary>
        /// How long it took to load the Exif data.
        /// </summary>
        public TimeSpan LoadTime;
        /// <summary>
        /// The software used to post-process the image
        /// </summary>
        public ExifData.ExifString ProcessingSoftware = new ExifData.ExifString();
        /// <summary>
        /// The ISO sensitivity of the camera when the image was taken
        /// </summary>
        public ExifData.ExifUShort Iso = new ExifData.ExifUShort();
    }
}
