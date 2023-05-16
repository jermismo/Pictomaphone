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

namespace ExifPlus.ExifData
{
    /// <summary>
    /// Base class for Exif Tag data.
    /// </summary>
    public abstract class ExifDataBase
    {
        protected bool readError = false;
        /// <summary>
        /// There was an error reading the data.
        /// </summary>
        public bool ReadError { get { return readError; } set { readError = value; } }

        protected bool hasValue = false;
        /// <summary>
        /// The tag value existed.
        /// </summary>
        public bool HasValue { get { return hasValue; } }

        /// <summary>
        /// The value of the tag.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Get the Data from an Exif tag.
        /// </summary>
        /// <returns></returns>
        public static ExifDataBase GetData(Exif.ExifTagFormat format, byte[] data, int offset, bool littleEndian)
        {
            switch (format)
            {
                case Exif.ExifTagFormat.STRING: return new ExifString(data, offset, littleEndian);
                case Exif.ExifTagFormat.BYTE: return new ExifByte(data, offset, littleEndian);
                case Exif.ExifTagFormat.USHORT: return new ExifUShort(data, offset, littleEndian);
                case Exif.ExifTagFormat.ULONG: return new ExifULong(data, offset, littleEndian);
                case Exif.ExifTagFormat.URATIONAL: return new ExifURational(data, offset, littleEndian);
                case Exif.ExifTagFormat.SBYTE: return new ExifSByte(data, offset, littleEndian);
                case Exif.ExifTagFormat.SSHORT: return new ExifShort(data, offset, littleEndian);
                case Exif.ExifTagFormat.SLONG: return new ExifInt(data, offset, littleEndian);
                case Exif.ExifTagFormat.SRATIONAL: return new ExifRational(data, offset, littleEndian);
                case Exif.ExifTagFormat.SINGLE: return new ExifSingle(data, offset, littleEndian);
                case Exif.ExifTagFormat.DOUBLE: return new ExifDouble(data, offset, littleEndian);
            }
            return new ExifString("unknown");
        }

    }


    /// <summary>
    /// Base class for Exif Tag data.
    /// </summary>
    /// <typeparam name="T">The data type of the value.</typeparam>
    public abstract class ExifDataBase<T> : ExifDataBase, System.Xml.Serialization.IXmlSerializable
    {

        protected T value = default(T);
        /// <summary>
        /// The value of the tag.
        /// </summary>
        new public T Value 
        {
            get { return value; }
            set { this.value = value; this.hasValue = true; }
        }

        /// <summary>
        /// Read this data type from the data array.
        /// </summary>
        /// <param name="data">The data array.</param>
        /// <param name="offset">The offset to read from.</param>
        /// <param name="littleEndian">If the data is little endian.</param>
        /// <returns>New ExifData of T.</returns>
        protected abstract void Read(byte[] data, int offset, bool littleEndian);

        public override string ToString()
        {
            if (this.readError) return "error";
            else if (!this.hasValue) return string.Empty;
            return this.value.ToString();
        }

        #region * IXmlSerializable *

        /// <summary>
        /// Used to parse a value returned during serialization.
        /// </summary>
        protected abstract void ParseValue(string s);

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            // always return null for schemas
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            string readerror = reader.GetAttribute("ReadError");
            string hasvalue = reader.GetAttribute("HasValue");

            Boolean isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                string val = reader.ReadElementContentAsString();
                this.ParseValue(val);
                reader.ReadEndElement();
            }

            // set these after, so we have the correct hasValue
            bool.TryParse(readerror, out this.readError);
            bool.TryParse(hasvalue, out this.hasValue);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("ReadError", this.readError.ToString());
            writer.WriteAttributeString("HasValue", this.hasValue.ToString());
            if (this.Value != null)
                writer.WriteElementString("Value", this.Value.ToString());
        }

        #endregion
    }
}
