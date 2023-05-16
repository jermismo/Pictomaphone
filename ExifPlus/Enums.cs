using System;

namespace ExifPlus.Exif
{
    /// <summary>
    /// Camera Flash Information
    /// </summary>
    [Flags]
    public enum Flash : ushort
    {
        No = 0,
        Fired = 1,
        StrobeReturnLightDetected = 6,
        On = 8,
        Off = 16,
        Auto = 24,
        FlashFunctionPresent = 32,
        RedEyeReduction = 64
    }

    /// <summary>
    /// Camera Light Source Information
    /// </summary>
    public enum LightSource : ushort
    { 
        Unknown = 0,
        Daylight = 1,
        Fluorescent = 2,
        Tunsten = 3,
        Flash = 4,
        FineWeather = 9,
        Cloudy = 10,
        Shade = 11,
        DaylightFluorescent = 12,
        DayWhiteFluorescent = 13,
        CoolWhiteFluorescent = 14,
        WhiteFluorescent = 15,
        WarmWhiteFluorescent = 16,
        StandardLightA = 17,
        StandardLightB = 18,
        StandardLightC = 19,
        D55 = 20,
        D65 = 21,
        D75 = 22,
        D50 = 23,
        IsoStudioTungsten = 24,
        Other = 255 	 
    }

    /// <summary>
    /// GPS Information
    /// </summary>
    public enum Gps
    {
        Version,
        LatitudeRef,
        Latitude,
        LongitudeRef,
        Longitude,
        AltitudeRef,
        Altitude,
        TimeStamp,
        Satellites,
        Status,
        MeasureMode,
        DOP,
        SpeedRef,
        Speed,
        TrackRef,
        Track,
        ImgDirectionRef,
        ImgDirection,
        MapDatum,
        DestLatitudeRef,
        DestLatitude,
        DestLongitudeRef,
        DestLongitude,
        DestBearingRef,
        DestBearing,
        DestDistanceRef,
        DestDistance,
        ProcessingMethod,
        AreaInformation,
        DateStamp,
        Differential
    }

    /// <summary>
    /// Reference for Latitude
    /// </summary>
    public enum GpsLatitudeRef
    {
        Unknown,
        North,
        South
    }

    /// <summary>
    /// Reference for Longitude
    /// </summary>
    public enum GpsLongitudeRef
    {
        Unknown,
        East,
        West
    }

    /// <summary>
    /// Exif ID tags
    /// </summary>
    public enum ExifId
    {
        /// <summary>
        /// Unknown (0)
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Processing Software (11) [string]
        /// </summary>
        ProcessingSoftware = 0x000B,
        /// <summary>
        /// Image Width (256) [uint32]
        /// </summary>
        ImageWidth = 0x0100,
        /// <summary>
        /// Image Height (257) [uint32]
        /// </summary>
        ImageHeight = 0x0101,
        /// <summary>
        /// Bits Per Sample (258) [uint16]
        /// </summary>
        BitsPerSample = 0x0102,
        /// <summary>
        /// Description (270) [string]
        /// </summary>
        Description = 0x010E,
        /// <summary>
        /// Camera Make (271) [string]
        /// </summary>
        Make = 0x010F,
        /// <summary>
        /// Camera Model (272) [string]
        /// </summary>
        Model = 0x0110,
        /// <summary>
        /// Camera Orientation (274) [uint16]
        /// </summary>
        Orientation = 0x0112,
        /// <summary>
        /// Horizontal Resolution (282) [urational]
        /// </summary>
        XResolution = 0x011A,
        /// <summary>
        /// Vertical Resolution (283) [urational]
        /// </summary>
        YResolution = 0x011B,
        /// <summary>
        /// XY Resolution Unit (296) [uint16]
        /// </summary>
        ResolutionUnit = 0x0128, 
        /// <summary>
        /// Software (305) [string]
        /// </summary>
        Software = 0x0131,
        /// <summary>
        /// Modified Date (306) [datetime string]
        /// </summary>
        ModifyDate = 0x0132, 
        /// <summary>
        /// Artist (315) [string]
        /// </summary>
        Artist = 0x013B,
        /// <summary>
        /// Thumbnail Offset (513) [uint32]
        /// </summary>
        ThumbnailOffset = 0x0201, 
        /// <summary>
        /// Thumbnail Length (514) [uint32]
        /// </summary>
        ThumbnailLength = 0x0202, 
        /// <summary>
        /// Copyright Info (33432) [string]
        /// </summary>
        Copyright = 0x8298, 
        /// <summary>
        /// Exposure Time (33434) [urational]
        /// </summary>
        ExposureTime = 0x829A, 
        /// <summary>
        /// F Number (33437) [urational]
        /// </summary>
        FNumber = 0x829D, 
        /// <summary>
        /// ISO (34855) [uint16]
        /// </summary>
        Iso = 0x8827, 
        /// <summary>
        /// Flash Used (37385) [uint16 flash enum]
        /// </summary>
        FlashUsed = 0x9202, 
        /// <summary>
        /// Light Source () [uint16 lightsource enum]
        /// </summary>
        LightSource = 0x9208,
        /// <summary>
        /// Focal Length (37386) [urational]
        /// </summary>
        FocalLength = 0x920A, 
        /// <summary>
        /// User Comment (37510) [string]
        /// </summary>
        UserComment = 0x9286 
    }

    /// <summary>
    /// Exif IFD tags
    /// </summary>
    public enum ExifIFD
    {
        Unknown = 0,
        /// <summary>
        /// Exif (34665)
        /// </summary>
        Exif = 34665,
        /// <summary>
        /// GPS (34853)
        /// </summary>
        Gps = 34853 
    }

    /// <summary>
    /// Exif 
    /// </summary>
    public enum Orientation
    {
        UNDEFINED = 0,
        Normal = 1,
        MirrorHorizontal = 2,
        Rotate180 = 3,
        MirrorVertical = 4,
        MirrorHorizontalRotate270Cw = 5,
        Rotate90Cw = 6,
        MirrorHorizontalRoate90Cw = 7,
        Rotate270Cw = 8,
    }

    /// <summary>
    /// Exif Tag Formats
    /// </summary>
    public enum ExifTagFormat
    {
        Unknown = 0,
        BYTE = 1,
        STRING,
        USHORT,
        ULONG,
        URATIONAL,
        SBYTE,
        UNDEFINED,
        SSHORT,
        SLONG,
        SRATIONAL,
        SINGLE,
        DOUBLE,
        NUM_FORMATS = 12
    }

    /// <summary>
    /// Resolution Units
    /// </summary>
    public enum Unit
    {
        Unknown = 0,
        Undefined = 1,
        Inch,
        Centimeter
    }

}
