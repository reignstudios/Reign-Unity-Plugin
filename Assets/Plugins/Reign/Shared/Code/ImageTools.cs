using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using UnityEngine;

namespace Reign.ImageTools
{
	/// <summary>
    /// The exception that is thrown when a image should be loaded which format is not supported.
    /// </summary>
    internal class UnsupportedImageFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
        /// </summary>
        public UnsupportedImageFormatException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> internal class with the name of the 
        /// parameter that causes this exception.
        /// </summary>
        /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
        public UnsupportedImageFormatException(string errorMessage)
            : base(errorMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> internal class with a specified 
        /// error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">The error message that explains the reason for this exception.</param>
        /// <param name="innerEx">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) 
        /// if no inner exception is specified.</param>
        public UnsupportedImageFormatException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx) { }
	}

	/// <summary>
    /// A collection of <see cref="ImageFrame"/> objects.
    /// </summary>
    internal class ImageFrameCollection : Collection<ImageFrame>
    {

    }

	internal struct Rectangle : IEquatable<Rectangle>
    {
		#region Data Members

        /// <summary>
        /// Zero rectangle with no width and no height.
        /// </summary>
        public static readonly Rectangle Zero = new Rectangle(0, 0, 0, 0);

        /// <summary>
        /// The height of this rectangle.
        /// </summary>
        private int _height;
        /// <summary>
        /// The width of this rectangle.
        /// </summary>
        private int _width;
        /// <summary>
        /// The x-coordinate of the upper-left corner.
        /// </summary>
        private int _x;
        /// <summary>
        /// The y-coordinate of the upper-left corner.
        /// </summary>
        private int _y;
        /// <summary>
        /// Gets the y-coordinate of the bottom edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The y-coordinate of the bottom edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Bottom
        {
            get { return _y + _height; }
        }

        /// <summary>
        /// Gets or sets the height of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The width of this <see cref="Rectangle"/> structure.</value>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Gets the x-coordinate of the left edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The x-coordinate of the left edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Left
        {
            get { return _x; }
        }

        /// <summary>
        /// Gets the x-coordinate of the right edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The x-coordinate of the right edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Right
        {
            get { return _x + _width; }
        }

        /// <summary>
        /// Gets the y-coordinate of the top edge of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The y-coordinate of the top edge of this 
        /// <see cref="Rectangle"/> structure.</value>
        public int Top
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets or sets the width of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The width of this <see cref="Rectangle"/> structure.</value>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the upper-left corner of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The x-coordinate of the upper-left corner 
        /// of this <see cref="Rectangle"/> structure.</value>
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Gets or sets the y-coordinate of the upper-left corner of this 
        /// <see cref="Rectangle"/> structure.
        /// </summary>
        /// <value>The y-coordinate of the upper-left corner 
        /// of this <see cref="Rectangle"/> structure.</value>
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

		#endregion

		#region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> struct
        /// with the specified location and size.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner 
        /// of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner 
        /// of the rectangle. </param>
        /// <param name="width">The width of the rectangle. </param>
        /// <param name="height">The height of the rectangle. </param>
        public Rectangle(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> internal struct 
        /// from a <see cref="Rectangle"/>
        /// </summary>
        /// <param name="other">The other rectangle.</param>
        public Rectangle(Rectangle other)
            : this(other.X, other.Y, other.Width, other.Height)
        {
        }

		#endregion

        #region IEquatable<Rectangle> Members

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same 
        /// type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            bool result = false;
            if (obj is Rectangle)
            {
                result = Equals((Rectangle)obj);
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the 
        /// <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Rectangle other)
        {
            return _x == other._x && _y == other._y && _width == other._width && _height == other._height;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return X ^ Y ^ Width ^ Height;
        }

        /// <summary>
        /// Tests whether two <see cref="Rectangle"/> structures have 
        /// equal location and size.
        /// </summary>
        /// <param name="left">The <see cref="Rectangle"/> structure that is to the 
        /// left of the equality operator.</param>
        /// <param name="right">The <see cref="Rectangle"/> structure that is to the 
        /// right of the equality operator.</param>
        /// <returns>This operator returns true if the two <see cref="Rectangle"/> structures 
        /// have equal X, Y, Width, and Height properties.</returns>
        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests whether two <see cref="Rectangle"/> structures differ
        /// in location or size.
        /// </summary>
        /// <param name="left">The <see cref="Rectangle"/> structure that is to the 
        /// left of the inequality  operator.</param>
        /// <param name="right">The <see cref="Rectangle"/> structure that is to the 
        /// right of the inequality  operator.</param>
        /// <returns>This operator returns true if any of the X, Y, Width or Height 
        /// properties of the two <see cref="Rectangle"/> structures are unequal; otherwise false.</returns>
        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !left.Equals(right);
        }

        #endregion
    }

	/// <summary>
    /// Represents a frame in a animation.
    /// </summary>
    internal class ImageFrame : ImageBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFrame"/> class
        /// by making a copy from another image.
        /// </summary>
        /// <param name="other">The other image, where the clone should be made from.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="other"/> is null
        /// (Nothing in Visual Basic).</exception>
        public ImageFrame(ImageFrame other)
            : base(other)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFrame"/> class.
        /// </summary>
        public ImageFrame()
        {
        }

        #endregion
    }

	/// <summary>
    /// Stores meta information about a image, like the name of the author,
    /// the copyright information, the date, where the image was created
    /// or some other information.
    /// </summary>
    internal sealed class ImageProperty : IEquatable<ImageProperty>
    {
        #region Properties

        private string _name;
        /// <summary>
        /// Gets or sets the name of the value, indicating
        /// which kind of information this property stores.
        /// </summary>
        /// <value>The name of the property.</value>
        /// <example>Typical properties are the author, copyright
        /// information or other meta information.</example>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets or sets the value of the property, e.g. the name
        /// of the author.
        /// </summary>
        /// <value>The value of this property.</value>
        public string Value { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProperty"/> class
        /// by setting the name and the value of the property.
        /// </summary>
        /// <param name="name">The name of the property. Cannot be null or empty</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is null or empty.</exception>
        public ImageProperty(string name, string value)
        {
            _name = name;

            Value = value;
        }

        #endregion

        #region IEquatable<ImageProperty> Members

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is 
        /// equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with 
        /// the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the 
        /// current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Equals(obj as ImageProperty);
        }

        /// <summary>
        /// Indicates whether the current object is equal to 
        /// another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> 
        /// parameter; otherwise, false.
        /// </returns>
        public bool Equals(ImageProperty other)
        {
            if (other == null)
            {
                return false;
            }

            return System.Object.Equals(Name, other.Name) &&
                   System.Object.Equals(Value, other.Value);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 1;

            if (Name != null)
            {
                hashCode ^= Name.GetHashCode();
            }

            if (Value != null)
            {
                hashCode ^= Value.GetHashCode();
            }

            return hashCode;
        }

        #endregion
    }

	/// <summary>
    /// Represents a list of <see cref="ImageProperty"/> instances.
    /// </summary>
    internal class ImagePropertyCollection : Collection<ImageProperty>
    {
    }

    /// <summary>
    /// Base classes for all Images.
    /// </summary>
    internal partial class ImageBase
    {
        #region Constants

        /// <summary>
        /// The default animation speed, when the image is animated.
        /// </summary>
        public const int DefaultDelayTime = 10;

        #endregion

        #region Properties

        private int _delayTime;
        /// <summary>
		/// If not 0, this field specifies the number of hundredths (1/100) of a second to 
		/// wait before continuing with the processing of the Data Stream. 
		/// The clock starts ticking immediately after the graphic is rendered. 
		/// This field may be used in conjunction with the User Input Flag field. 
		/// </summary>
        public int DelayTime
        {
            get
            {
                int delayTime = _delayTime;

                if (delayTime <= 0)
                {
                    delayTime = DefaultDelayTime;
                }

                return delayTime;
            }
            set { _delayTime = value; }
        }

        private bool _isFilled;
        /// <summary>
        /// Gets or sets a value indicating whether this image has been loaded.
        /// </summary>
        /// <value><c>true</c> if this image has been loaded; otherwise, <c>false</c>.</value>
        public bool IsFilled
        {
            get
            {
                return _isFilled;
            }
        }

        private byte[] _pixels;
        /// <summary>
        /// Returns all pixels of the image as simple byte array.
        /// </summary>
        /// <value>All image pixels as byte array.</value>
        /// <remarks>The returned array has a length of Width * Length * 4 bytes
        /// and stores the red, the green, the blue and the alpha value for
        /// each pixel in this order.</remarks>
        public byte[] Pixels
        {
            get
            {
                return _pixels;
            }
        }

        private int _pixelHeight;
        /// <summary>
        /// Gets the height of this <see cref="ExtendedImage"/> in pixels.
        /// </summary>
        /// <value>The height of this image.</value>
        /// <remarks>The height will be initialized by the constructor
        /// or when the data will be pixel data will set.</remarks>
        public int PixelHeight
        {
            get 
            {
                return _pixelHeight; 
            }
        }

        private int _pixelWidth;
        /// <summary>
        /// Gets the width of this <see cref="ExtendedImage"/> in pixels.
        /// </summary>
        /// <value>The width of this image.</value>
        /// <remarks>The width will be initialized by the constructor
        /// or when the data will be pixel data will set.</remarks>
        public int PixelWidth
        {
            get
            {
                return _pixelWidth;
            }
        }

        /// <summary>
        /// Gets the ratio between the width and the height of this <see cref="ImageBase"/> instance.
        /// </summary>
        /// <value>The ratio between the width and the height.</value>
        public float PixelRatio
        {
            get
            {
                if (IsFilled)
                {
                    return (float)PixelWidth / PixelHeight;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of a pixel at the specified position.
        /// </summary>
        /// <param name="x">The x-coordinate of the pixel. Must be greater
        /// than zero and smaller than the width of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel. Must be greater
        /// than zero and smaller than the width of the pixel.</param>
        /// <value>The color of the pixel.</value>
        /// <exception cref="ArgumentException">
        ///     <para><paramref name="x"/> is smaller than zero or greater than
        ///     the width of the image.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="y"/> is smaller than zero or greater than
        ///     the height of the image.</para>
        /// </exception>
        public Color32 this[int x, int y]
        {
            get
            {
                int start = (y * PixelWidth + x) * 4;

                Color32 result = new Color32();

                result.r = _pixels[start + 0];
                result.g = _pixels[start + 1];
                result.b = _pixels[start + 2];
                result.a = _pixels[start + 3];

                return result;
            }
            set
            {
                int start = (y * PixelWidth + x) * 4;

                _pixels[start + 0] = value.r;
                _pixels[start + 1] = value.g;
                _pixels[start + 2] = value.b;
                _pixels[start + 3] = value.a;
            }
        }

        /// <summary>
        /// Calculates a new rectangle which represents 
        /// the dimensions of this image.
        /// </summary>
        /// <value>The <see cref="Rectangle"/> object, which
        /// represents the image dimension.</value>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, PixelWidth, PixelHeight);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class
        /// with the height and the width of the image.
        /// </summary>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        /// <exception cref="ArgumentException">
        ///     <para><paramref name="width"/> is equals or less than zero.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="height"/> is equals or less than zero.</para>
        /// </exception>
        public ImageBase(int width, int height)
        {
            _pixelWidth  = width;
            _pixelHeight = height;

            _pixels = new byte[PixelWidth * PixelHeight * 4];

            _isFilled = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class
        /// by making a copy from another image.
        /// </summary>
        /// <param name="other">The other, where the clone should be made from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null
        /// (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="other"/> is not loaded.</exception>
        public ImageBase(ImageBase other)
        {
            byte[] pixels = other.Pixels;

            _pixelWidth  = other.PixelWidth;
            _pixelHeight = other.PixelHeight;
            _pixels = new byte[pixels.Length];

            Array.Copy(pixels, _pixels, pixels.Length);

            _isFilled = other.IsFilled;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBase"/> class.
        /// </summary>
        public ImageBase()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the pixel array of the image.
        /// </summary>
        /// <param name="width">The new width of the image.
        /// Must be greater than zero.</param>
        /// <param name="height">The new height of the image.
        /// Must be greater than zero.</param>
        /// <param name="pixels">The array with colors. Must be a multiple
        /// of four, width and height.</param>
        /// <exception cref="ArgumentException">
        /// 	<para><paramref name="width"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="height"/> is smaller than zero.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="pixels"/> is not a multiple of four, 
        /// 	width and height.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="pixels"/> is null.</exception>
        public void SetPixels(int width, int height, byte[] pixels)
        {
            if (pixels.Length != width * height * 4)
            {
                throw new ArgumentException(
                    "Pixel array must have the length of width * height * 4.",
                    "pixels");
            }

            _pixelWidth  = width;
            _pixelHeight = height;
            _pixels = pixels;

            _isFilled = true;
        }

        #endregion
    }

	internal sealed partial class ExtendedImage : ImageBase
    {
        #region Constants

        /// <summary>
        /// The default density value (dots per inch) in x direction. The default value is 75 dots per inch.
        /// </summary>
        public const float DefaultDensityX = 75;
        /// <summary>
        /// The default density value (dots per inch) in y direction. The default value is 75 dots per inch.
        /// </summary>
        public const float DefaultDensityY = 75;

        #endregion

        #region Fields

       // private readonly object _lockObject = new object();

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the download is completed.
        /// </summary>
        /*public event OpenReadCompletedEventHandler DownloadCompleted;
        private void OnDownloadCompleted(OpenReadCompletedEventArgs e)
        {
            OpenReadCompletedEventHandler downloadCompletedHandler = DownloadCompleted;

            if (downloadCompletedHandler != null)
            {
                downloadCompletedHandler(this, e);
            }
        }*/

        /// <summary>
        /// Occurs when the download progress changed.
        /// </summary>
        /*public event DownloadProgressChangedEventHandler DownloadProgress;
        private void OnDownloadProgress(DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChangedEventHandler downloadProgressHandler = DownloadProgress;

            if (downloadProgressHandler != null)
            {
                downloadProgressHandler(this, e);
            }
        }*/

        /// <summary>
        /// Occurs when the loading is completed.
        /// </summary>
        public event EventHandler LoadingCompleted;
        private void OnLoadingCompleted(EventArgs e)
        {
            EventHandler loadingCompletedHandler = LoadingCompleted;

            if (loadingCompletedHandler != null)
            {
                loadingCompletedHandler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the loading of the image failed.
        /// </summary>
		public delegate void EventHandlerCustom(object o, Exception e);
        public event EventHandlerCustom LoadingFailed;
        private void OnLoadingFailed(Exception e)
        {
            var eventHandler = LoadingFailed;

            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this image is loading at the moment.
        /// </summary>
        /// <value>
        /// true if this instance is image is loading at the moment; otherwise, false.
        /// </value>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// Gets or sets the resolution of the image in x direction. It is defined as 
        /// number of dots per inch and should be an positive value.
        /// </summary>
        /// <value>The density of the image in x direction.</value>
        public float DensityX { get; set; }

        /// <summary>
        /// Gets or sets the resolution of the image in y direction. It is defined as 
        /// number of dots per inch and should be an positive value.
        /// </summary>
        /// <value>The density of the image in y direction.</value>
        public float DensityY { get; set; }

        /// <summary>
        /// Gets the width of the image in inches. It is calculated as the width of the image 
        /// in pixels multiplied with the density. When the density is equals or less than zero 
        /// the default value is used.
        /// </summary>
        /// <value>The width of the image in inches.</value>
        public float InchWidth
        {
            get
            {
                float densityX = DensityX;

                if (densityX <= 0)
                {
                    densityX = DefaultDensityX;
                }

                return PixelWidth / densityX;
            }
        }

        /// <summary>
        /// Gets the height of the image in inches. It is calculated as the height of the image 
        /// in pixels multiplied with the density. When the density is equals or less than zero 
        /// the default value is used.
        /// </summary>
        /// <value>The height of the image in inches.</value>
        public float InchHeight
        {
            get
            {
                float densityY = DensityY;

                if (densityY <= 0)
                {
                    densityY = DefaultDensityY;
                }

                return PixelHeight / densityY;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this image is animated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this image is animated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAnimated
        {
            get { return _frames.Count > 0; }
        }

        private ImageFrameCollection _frames = new ImageFrameCollection();
        /// <summary>
        /// Get the other frames for the animation.
        /// </summary>
        /// <value>The list of frame images.</value>
        public ImageFrameCollection Frames
        {
            get
            {
                return _frames;
            }
        }

        private ImagePropertyCollection _properties = new ImagePropertyCollection();
        /// <summary>
        /// Gets the list of properties for storing meta information about this image.
        /// </summary>
        /// <value>A list of image properties.</value>
        public ImagePropertyCollection Properties
        {
            get 
            {
                return _properties; 
            }
        }

        private Uri _uriSource;
        /// <summary>
        /// Gets or sets the <see cref="Uri"/> source of the <see cref="ExtendedImage"/>.
        /// </summary>
        /// <value>The <see cref="Uri"/> source of the <see cref="ExtendedImage"/>. The
        /// default value is null (Nothing in Visual Basic).</value>
        /// <remarks>If the stream source and the uri source are both set, 
        /// the stream source will be ignored.</remarks>
       /* public Uri UriSource
        {
            get { return _uriSource; }
            set
            {
                lock (_lockObject)
                {
                    _uriSource = value;

                    if (UriSource != null)
                    {
                        LoadAsync(UriSource);
                    }
                }
            }
        }*/

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedImage"/> class
        /// with the height and the width of the image.
        /// </summary>
        /// <param name="width">The width of the image in pixels.</param>
        /// <param name="height">The height of the image in pixels.</param>
        public ExtendedImage(int width, int height)
            : base(width, height)
        {
            DensityX = DefaultDensityX;
            DensityY = DefaultDensityY;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedImage"/> class
        /// by making a copy from another image.
        /// </summary>
        /// <param name="other">The other image, where the clone should be made from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null
        /// (Nothing in Visual Basic).</exception>
        public ExtendedImage(ExtendedImage other)
            : base(other)
        {
            foreach (ImageFrame frame in other.Frames)
            {
                if (frame != null)
                {
                    if (!frame.IsFilled)
                    {
                        throw new ArgumentException("The image contains a frame that has not been loaded yet.");
                    }

                    Frames.Add(new ImageFrame(frame));
                }
            }

            DensityX = DefaultDensityX;
            DensityY = DefaultDensityY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedImage"/> class.
        /// </summary>
        public ExtendedImage()
        {
            DensityX = DefaultDensityX;
            DensityY = DefaultDensityY;
        }

		#endregion Constructors 

        #region Methods

        /// <summary>
        /// Sets the source of the image to a specified stream.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that contains the data for 
        /// this <see cref="ExtendedImage"/>. Cannot be null.</param>
        /// <remarks>
        /// The stream will not be closed or disposed when the loading
        /// is finished, so always use a using block or manually dispose
        /// the stream, when using the method. 
        /// The <see cref="ExtendedImage"/> internal class does not support alpha
        /// transparency in bitmaps. To enable alpha transparency, use
        /// PNG images with 32 bits per pixel.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>
        /// is null (Nothing in Visual Basic).</exception>
        /// <exception cref="ImageFormatException">The image has an invalid
        /// format.</exception>
        /// <exception cref="NotSupportedException">The image cannot be loaded
        /// because loading images of this type are not supported yet.</exception>
       /* public void SetSource(Stream stream)
        {
            if (_uriSource == null)
            {
                LoadAsync(stream);
            }
        }*/

        private void Load(Stream stream)
        {
            try
            {
                if (!stream.CanRead)
                {
                    throw new NotSupportedException("Cannot read from the stream.");
                }

                if (!stream.CanSeek)
                {
                    throw new NotSupportedException("The stream does not support seeking.");
                }

                var decoders = Decoders.GetAvailableDecoders();

                if (decoders.Count > 0)
                {
                    int maxHeaderSize = decoders.Max(x => x.HeaderSize);
                    if (maxHeaderSize > 0)
                    {
                        byte[] header = new byte[maxHeaderSize];

                        stream.Read(header, 0, maxHeaderSize);
                        stream.Position = 0;

                        var decoder = decoders.FirstOrDefault(x => x.IsSupportedFileFormat(header));
                        if (decoder != null)
                        {
                            decoder.Decode(this, stream);
                            IsLoading = false;
                        }
                    }
                }

                if (IsLoading)
                {
                    IsLoading = false;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("Image cannot be loaded. Available decoders:");

                    foreach (IImageDecoder decoder in decoders)
                    {
                        stringBuilder.AppendLine("-" + decoder);
                    }

                    throw new UnsupportedImageFormatException(stringBuilder.ToString());
                }
            }
            finally
            {
                stream.Dispose();
            }
        }

        /*private void LoadAsync(Stream stream)
        {
            IsLoading = true;

            ThreadPool.QueueUserWorkItem(objectState =>
                {
                    try
                    {
                        Load(stream);

                        OnLoadingCompleted(EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        OnLoadingFailed(e);
                    }
                });
        }*/

        /*private void LoadAsync(Uri uri)
        {
            try
            {
                bool isHandled = false;

                if (!uri.IsAbsoluteUri)
                {
                    string fixedUri = uri.ToString();

                    fixedUri = fixedUri.Replace("\\", "/");

                    if (fixedUri.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        fixedUri = fixedUri.Substring(1);
                    }

                    var resourceStream = Extensions.GetLocalResourceStream(new Uri(fixedUri, UriKind.Relative));
                    if (resourceStream != null)
                    {
                        LoadAsync(resourceStream);

                        isHandled = true;
                    }
                }

                if (!isHandled)
                {
                    IsLoading = true;

                    WebClient webClient = new WebClient();
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                    webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
                    webClient.OpenReadAsync(uri);
                }
            }
            catch (ArgumentException e)
            {
                OnLoadingFailed(new UnhandledExceptionEventArgs(e, false));
            }
            catch (InvalidOperationException e)
            {
                OnLoadingFailed(new UnhandledExceptionEventArgs(e, false));
            }
        }

        private void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    Stream remoteStream = e.Result;

                    if (remoteStream != null)
                    {
                        LoadAsync(remoteStream);
                    }
                }
                else
                {
                    OnLoadingFailed(new UnhandledExceptionEventArgs(e.Error, false));
                }

                OnDownloadCompleted(e);
            }
            catch (WebException ex)
            {
                OnLoadingFailed(new UnhandledExceptionEventArgs(ex, false));
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnDownloadProgress(e);
        }*/
        
        #endregion Methods 

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public ExtendedImage Clone()
        {
            return new ExtendedImage(this);
        }

        #endregion
    }

	/// <summary>
    /// A collection of simple helper extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Defines a constant rectangle where all values are zero.
        /// </summary>
        public static readonly Rect ZeroRect = new Rect(0, 0, 0, 0);

        /// <summary>
        /// Converts byte array to a new array where each value in the original array is represented 
        /// by a the specified number of bits.
        /// </summary>
        /// <param name="bytes">The bytes to convert from. Cannot be null.</param>
        /// <param name="bits">The number of bits per value.</param>
        /// <returns>The resulting byte array. Is never null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="bits"/> is greater or equals than zero.</exception>
        public static byte[] ToArrayByBitsLength(this byte[] bytes, int bits)
        {
            byte[] result = null;

            if (bits < 8)
            {
                result = new byte[bytes.Length * 8 / bits];

                int factor = (int)Math.Pow(2, bits) - 1;
                int mask = (0xFF >> (8 - bits));
                int resultOffset = 0;

                for (int i = 0; i < bytes.Length; i++)
                {
                    for (int shift = 0; shift < 8; shift += bits)
                    {
                        int colorIndex = (((bytes[i]) >> (8 - bits - shift)) & mask) * (255 / factor);

                        result[resultOffset] = (byte)colorIndex;

                        resultOffset++;
                    }

                }
            }
            else
            {
                result = bytes;
            }

            return result;
        }

        /// <summary>
        /// Gets the stream to a local resource
        /// </summary>
        /// <param name="uri">The path to the local stream. Cannot be null.</param>
        /// <returns>The stream to the local resource when such a resource exists or null otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null.</exception>
        public static Stream GetLocalResourceStream(Uri uri)
        {
			return null;
            /*StreamResourceInfo info = Application.GetResourceStream(uri);
            if (info == null)
            {
                Application app = Application.Current;

                if (app != null)
                {
                    Type appplicationType = app.GetType();

                    string assemblyName = appplicationType.Assembly.FullName.Split(new char[] { ',' })[0];

                    string uriString = uri.OriginalString;
                    uriString = string.Format(CultureInfo.CurrentCulture, "{0};component/{1}", assemblyName, uriString);
                    uriString = uriString.Replace("\\", "/");
                    uriString = uriString.Replace("//", "/");

                    Uri resourceUri = new Uri(uriString, UriKind.Relative);

                    info = Application.GetResourceStream(resourceUri);
                }
            }

            Stream stream = info != null ? info.Stream : null;
            return stream;*/
        }

        /// <summary>
        /// Multiplies the values of the specified rectangle by the factor.
        /// </summary>
        /// <param name="rectangle">The rectangle to multiply with the factor.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The new rectangle.</returns>
        public static Rectangle Multiply(Rectangle rectangle, float factor)
        {
            rectangle.X = (int)(rectangle.X * factor);
            rectangle.Y = (int)(rectangle.Y * factor);

            rectangle.Width  = (int)(rectangle.Width * factor);
            rectangle.Height = (int)(rectangle.Height * factor);

            return rectangle;
        }

        /// <summary>
        /// Multiplies the values of the specified rectangle by the factor.
        /// </summary>
        /// <param name="rectangle">The rectangle to multiply with the factor.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Multiply(Rect rectangle, float factor)
        {
            rectangle.x = rectangle.x * factor;
            rectangle.y = rectangle.y * factor;

            rectangle.width  = rectangle.width * factor;
            rectangle.height = rectangle.height * factor;

            return rectangle;
        }

        /// <summary>
        /// Determines whether the specified value is a valid number.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>A flag indicating whether the specified value is a number.</returns>
        public static bool IsNumber(this float value)
        {
            return !float.IsInfinity(value) && !float.IsNaN(value);
        }

        /// <summary>
        /// Invokes the specified foreach item in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
        /// <param name="items">The enumerable that is iterated through this method. Cannot be null.</param>
        /// <param name="action">The action to invoke foreach item. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="items"/> is null.</para>
        /// <para>- or -</para>
        /// <para><paramref name="action"/> is null.</para>
        /// </exception>
        public static void Foreach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                if (item != null)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Invokes the specified foreach item in the enumerable.
        /// </summary>
        /// <param name="items">The enumerable that is iterated through this method. Cannot be null.</param>
        /// <param name="action">The action to invoke foreach item. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="items"/> is null.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="action"/> is null.</para>
        /// </exception>
        public static void Foreach(this IEnumerable items, Action<object> action)
        {
            foreach (object item in items)
            {
                if (item != null)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Adds the the specified elements to the target collection object.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the source and target.</typeparam>
        /// <param name="target">The target, where the items should be inserted to. Cannot be null.</param>
        /// <param name="elements">The elements to add to the collection. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="target"/> is null.</para>
        /// <para>- or -</para>
        /// <para><paramref name="elements"/> is null.</para>
        /// </exception>
        public static void AddRange<TItem>(this List<TItem> target, IEnumerable<TItem> elements)
        {
            foreach (TItem item in elements)
            {
                target.Add(item);
            }
        }

        /// <summary>
        /// Adds the the specified elements to the target collection object.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the source and target.</typeparam>
        /// <param name="target">The target, where the items should be inserted to. Cannot be null.</param>
        /// <param name="elements">The elements to add to the collection. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="target"/> is null.</para>
        /// <para>- or -</para>
        /// <para><paramref name="elements"/> is null.</para>
        /// </exception>
        public static void AddRange<TItem>(this Collection<TItem> target, IEnumerable<TItem> elements)
        {
            foreach (TItem item in elements)
            {
                target.Add(item);
            }
        }

        /// <summary>
        /// Determines whether the specified value is between two other
        /// values.
        /// </summary>
        /// <typeparam name="TValue">The type of the values to check.
        /// Must implement <see cref="IComparable"/>.</typeparam>
        /// <param name="value">The value which should be between the other values.</param>
        /// <param name="low">The lower value.</param>
        /// <param name="high">The higher value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is between the other values; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBetween<TValue>(this TValue value, TValue low, TValue high) where TValue : IComparable
        {
            return (Comparer<TValue>.Default.Compare(low, value) <= 0
                 && Comparer<TValue>.Default.Compare(high, value) >= 0);
        }

        /// <summary>
        /// Arranges the value, so that it is not greater than the high value and
        /// not lower than the low value and returns the result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="low">The lower value.</param>
        /// <param name="high">The higher value.</param>
        /// <returns>The arranged value.</returns>
        /// <remarks>If the specified lower value is greater than the higher value. The low value
        /// will be returned.</remarks>
        public static TValue RemainBetween<TValue>(this TValue value, TValue low, TValue high) where TValue : IComparable
        {
            TValue result = value;

            if (Comparer<TValue>.Default.Compare(high, low) < 0)
            {
                result = low;
            }

            else if (Comparer<TValue>.Default.Compare(value, low) <= 0)
            {
                result = low;
            }

            else if (Comparer<TValue>.Default.Compare(value, high) >= 0)
            {
                result = high;
            }

            return result;
        }

        /// <summary>
        /// Swaps two references.
        /// </summary>
        /// <typeparam name="TRef">The type of the references to swap.</typeparam>
        /// <param name="lhs">The first reference.</param>
        /// <param name="rhs">The second reference.</param>
        public static void Swap<TRef>(ref TRef lhs, ref TRef rhs) where TRef : class
        {
            TRef tmp = lhs;

            lhs = rhs;
            rhs = tmp;
        }
    }

	internal interface IImageDecoder
    {
        /// <summary>
        /// Gets the size of the header for this image type.
        /// </summary>
        /// <value>The size of the header.</value>
        int HeaderSize { get; }

        /// <summary>
        /// Indicates if the image decoder supports the specified
        /// file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>
        /// <c>true</c>, if the decoder supports the specified
        /// extensions; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="extension"/>
        /// is null (Nothing in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException"><paramref name="extension"/> is a string
        /// of length zero or contains only blanks.</exception>
        bool IsSupportedFileExtension(string extension);

        /// <summary>
        /// Indicates if the image decoder supports the specified
        /// file header.
        /// </summary>
        /// <param name="header">The file header.</param>
        /// <returns>
        /// <c>true</c>, if the decoder supports the specified
        /// file header; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="header"/>
        /// is null (Nothing in Visual Basic).</exception>
        bool IsSupportedFileFormat(byte[] header);

        /// <summary>
        /// Decodes the image from the specified stream and sets
        /// the data to image.
        /// </summary>
        /// <param name="image">The image, where the data should be set to.
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <param name="stream">The stream, where the image should be
        /// decoded from. Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="image"/> is null (Nothing in Visual Basic).</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="stream"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        void Decode(ExtendedImage image, Stream stream);
    }

    /// <summary>
    /// Helper methods for decoders.
    /// </summary>
    internal static class Decoders
    {
        private static List<Type> _decoderTypes = new List<Type>();

        /// <summary>
        /// Adds the type of the decoder to the list of available image decoders.
        /// </summary>
        /// <typeparam name="TDecoder">The type of the decoder.</typeparam>
        public static void AddDecoder<TDecoder>() where TDecoder : IImageDecoder
        {
            if (!_decoderTypes.Contains(typeof(TDecoder)))
            {
                _decoderTypes.Add(typeof(TDecoder));
            }
        }

        /// <summary>
        /// Gets a list of all available decoders.
        /// </summary>
        /// <returns>A list of all available decoders.</returns>
        public static ReadOnlyCollection<IImageDecoder> GetAvailableDecoders()
        {
            List<IImageDecoder> decoders = new List<IImageDecoder>();

            foreach (Type decorderType in _decoderTypes)
            {
                if (decorderType != null)
                {
                    decoders.Add(Activator.CreateInstance(decorderType) as IImageDecoder);
                }
            }

            return new ReadOnlyCollection<IImageDecoder>(decoders);
        }
    }

	/// <summary>
    /// The Logical Screen Descriptor contains the parameters 
    /// necessary to define the area of the display device 
    /// within which the images will be rendered
    /// </summary>
    sealed internal class GifLogicalScreenDescriptor
    {
        /// <summary>
        /// Width, in pixels, of the Logical Screen where the images will 
        /// be rendered in the displaying device.
        /// </summary>
        public short Width;
        /// <summary>
        /// Height, in pixels, of the Logical Screen where the images will be 
        /// rendered in the displaying device.
        /// </summary>
        public short Height;
        /// <summary>
        /// Index into the Global Color Table for the Background Color. 
        /// The Background Color is the color used for those 
        /// pixels on the screen that are not covered by an image.
        /// </summary>
        public byte Background;
        /// <summary>
        /// Flag indicating the presence of a Global Color Table; 
        /// if the flag is set, the Global Color Table will immediately 
        /// follow the Logical Screen Descriptor.
        /// </summary>
        public bool GlobalColorTableFlag;
        /// <summary>
        /// If the Global Color Table Flag is set to 1, 
        /// the value in this field is used to calculate the number of 
        /// bytes contained in the Global Color Table.
        /// </summary>
        public int GlobalColorTableSize;
    }

	 /// <summary>
    /// Specifies, what to do with the last image 
    /// in an animation sequence.
    /// </summary>
    internal enum DisposalMethod : int
    {
        /// <summary>
        /// No disposal specified. The decoder is not 
        /// required to take any action. 
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// Do not dispose. The graphic is to be left in place. 
        /// </summary>
        NotDispose = 1,
        /// <summary>
        /// Restore to background color. 
        /// The area used by the graphic must be restored to
        /// the background color. 
        /// </summary>
        RestoreToBackground = 2,
        /// <summary>
        /// Restore to previous. The decoder is required to 
        /// restore the area overwritten by the 
        /// graphic with what was there prior to rendering the graphic. 
        /// </summary>
        RestoreToPrevious = 3
    }

	 /// <summary>
    /// The Graphic Control Extension contains parameters used when 
    /// processing a graphic rendering block.
    /// </summary>
    sealed internal class GifGraphicsControlExtension
    {        
        /// <summary>
        /// Indicates the way in which the graphic is to be treated after being displayed. 
        /// </summary>
        public DisposalMethod DisposalMethod;
        /// <summary>
        /// Indicates whether a transparency index is given in the Transparent Index field. 
        /// (This field is the least significant bit of the byte.) 
        /// </summary>
        public bool TransparencyFlag;
        /// <summary>
        /// The Transparency Index is such that when encountered, the corresponding pixel 
        /// of the display device is not modified and processing goes on to the next pixel.
        /// </summary>
        public int TransparencyIndex;
        /// <summary>
        /// If not 0, this field specifies the number of hundredths (1/100) of a second to 
        /// wait before continuing with the processing of the Data Stream. 
        /// The clock starts ticking immediately after the graphic is rendered. 
        /// This field may be used in conjunction with the User Input Flag field. 
        /// </summary>
        public int DelayTime;
    }

	/// <summary>
    /// A static internal class with a lot of helper methods, which guards 
    /// a method agains invalid parameters.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Verifies that the specified value is between a lower and a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is not between
        /// the lower and the upper value.</exception>
        public static void Between<TValue>(TValue target, TValue lower, TValue upper, string parameterName) where TValue : IComparable
        {
            if (!target.IsBetween(lower, upper))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be between {0} and {1}", lower, upper), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is between a lower and a upper value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is not between
        /// the lower and the upper value.</exception>
        public static void Between<TValue>(TValue target, TValue lower, TValue upper, string parameterName, string message) where TValue : IComparable
        {
            if (!target.IsBetween(lower, upper))
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater than a lower value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterThan<TValue>(TValue target, TValue lower, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(lower) <= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be greater than {0}", lower), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater than a lower value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterThan<TValue>(TValue target, TValue lower, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(lower) <= 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater or equals than a lower value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterEquals<TValue>(TValue target, TValue lower, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(lower) < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be greater than {0}", lower), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater or equals than a lower value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterEquals<TValue>(TValue target, TValue lower, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(lower) < 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less than a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessThan<TValue>(TValue target, TValue upper, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(upper) <= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be less than {0}", upper), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less than a upper value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessThan<TValue>(TValue target, TValue upper, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(upper) <= 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less or equals than a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessEquals<TValue>(TValue target, TValue upper, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(upper) > 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be less than {0}", upper), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less or equals than a upper value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessEquals<TValue>(TValue target, TValue upper, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(upper) > 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the collection method parameter with specified reference
        /// contains one or more elements and throws an exception
        /// if the object contains no elements.
        /// </summary>
        /// <typeparam name="TType">The type of the items in the collection.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentException"><paramref name="enumerable"/> is
        /// empty or contains only blanks.</exception>
        public static void NotEmpty<TType>(ICollection<TType> enumerable, string parameterName)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (enumerable.Count == 0)
            {
                throw new ArgumentException("Collection does not contain an item", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the collection method parameter with specified reference
        /// contains one or more elements and throws an exception with
        /// the passed message if the object contains no elements.
        /// </summary>
        /// <typeparam name="TType">The type of the items in the collection.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="enumerable"/> is
        /// empty or contains only blanks.</exception>
        public static void NotEmpty<TType>(ICollection<TType> enumerable, string parameterName, string message)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (enumerable.Count == 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the method parameter with specified object value and message  
        /// is not null and throws an exception if the object is null.
        /// </summary>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        public static void NotNull(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the method parameter with specified object value and message
        /// is not null and throws an exception with the passed message if the object is null.
        /// </summary>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <example>
        /// Use the following code to validate a parameter:
        /// <code>
        /// // A method with parameter 'name' which cannot be null.
        /// public void MyMethod(string name)
        /// {
        ///    Guard.NotNull(name, "name", "Name is null!");
        /// }
        /// </code>
        /// </example>
        public static void NotNull(object target, string parameterName, string message)
        {
            if (target == null)
            {
                throw new ArgumentNullException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the string method parameter with specified object value and message
        /// is not null, not empty and does not contain only blanls and throws an exception 
        /// if the object is null.
        /// </summary>
        /// <param name="target">The target string, which should be checked against being null or empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is
        /// empty or contains only blanks.</exception>
        public static void NotNullOrEmpty(string target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrEmpty(target) || target.Trim().Equals(string.Empty))
            {
                throw new ArgumentException("String parameter cannot be null or empty and cannot contain only blanks.", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the string method parameter with specified object value and message
        /// is not null, not empty and does not contain only blanls and throws an exception with 
        /// the passed message if the object is null.
        /// </summary>
        /// <param name="target">The target string, which should be checked against being null or empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is
        /// empty or contains only blanks.</exception>
        public static void NotNullOrEmpty(string target, string parameterName, string message)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrEmpty(target) || target.Trim().Equals(string.Empty))
            {
                throw new ArgumentException(message, parameterName);
            }
        }
    }

	 /// <summary>
    /// Each image in the Data Stream is composed of an Image Descriptor, 
    /// an optional Local Color Table, and the image data. 
    /// Each image must fit within the boundaries of the 
    /// Logical Screen, as defined in the Logical Screen Descriptor. 
    /// </summary>
    sealed internal class GifImageDescriptor
    {
        /// <summary>
        /// Column number, in pixels, of the left edge of the image, 
        /// with respect to the left edge of the Logical Screen. 
        /// Leftmost column of the Logical Screen is 0.
        /// </summary>
        public short Left;
        /// <summary>
        /// Row number, in pixels, of the top edge of the image with 
        /// respect to the top edge of the Logical Screen. 
        /// Top row of the Logical Screen is 0.
        /// </summary>
        public short Top;
        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        public short Width;
        /// <summary>
        /// Height of the image in pixels.
        /// </summary>
        public short Height;
        /// <summary>
        /// Indicates the presence of a Local Color Table immediately 
        /// following this Image Descriptor.
        /// </summary>
        public bool LocalColorTableFlag;
        /// <summary>
        /// If the Local Color Table Flag is set to 1, the value in this field 
        /// is used to calculate the number of bytes contained in the Local Color Table.
        /// </summary>
        public int LocalColorTableSize;
        /// <summary>
        /// Indicates if the image is interlaced. An image is interlaced 
        /// in a four-pass interlace pattern.
        /// </summary>
        public bool InterlaceFlag;
    }

	/// <summary>
    /// Uncrompress data using the LZW algorithmus.
    /// </summary>
    sealed internal class LZWDecoder
    {
        private const int StackSize = 4096;
        private const int NullCode = -1;

        private Stream _stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="LZWDecoder"/> class
        /// and sets the stream, where the compressed data should be read from.
        /// </summary>
        /// <param name="stream">The stream. where to read from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null
        /// (Nothing in Visual Basic).</exception>
        public LZWDecoder(Stream stream)
        {
            Guard.NotNull(stream, "stream");

            _stream = stream;
        }

        /// <summary>
        /// Decodes and uncompresses all pixel indices from the stream.
        /// </summary>
        /// <param name="width">The width of the pixel index array.</param>
        /// <param name="height">The height of the pixel index array.</param>
        /// <param name="dataSize">Size of the data.</param>
        /// <returns>The decoded and uncompressed array.</returns>
        public byte[] DecodePixels(int width, int height, int dataSize)
        {
            // The resulting index table.
            byte[] pixels = new byte[width * height];

            // Calculate the clear code. The value of the clear code is 2 ^ dataSize
            int clearCode = 1 << dataSize;

            if (dataSize == Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException("dataSize", "Must be less than Int32.MaxValue");
            }

            int codeSize = dataSize + 1;

            // Calculate the end code
            int endCode = clearCode + 1;

            // Calculate the available code.
            int availableCode = clearCode + 2;

            #region Jillzhangs Code (Not From Me) see: http://giflib.codeplex.com/ 

            int code = NullCode; //ÓÃÓÚ´æ´¢µ±Ç°µÄ±àÂëÖµ
            int old_code = NullCode;//ÓÃÓÚ´æ´¢ÉÏÒ»´ÎµÄ±àÂëÖµ
            int code_mask = (1 << codeSize) - 1;//±íÊ¾±àÂëµÄ×î´óÖµ£¬Èç¹ûcodeSize=5,Ôòcode_mask=31
            int bits = 0;//ÔÚ±àÂëÁ÷ÖÐÊý¾ÝµÄ±£´æÐÎÊ½Îªbyte£¬¶øÊµ¼Ê±àÂë¹ý³ÌÖÐÊÇÕÒÊµ¼Ê±àÂëÎ»À´´æ´¢µÄ£¬±ÈÈçµ±codeSize=5µÄÊ±ºò£¬ÄÇÃ´Êµ¼ÊÉÏ5bitµÄÊý¾Ý¾ÍÓ¦¸Ã¿ÉÒÔ±íÊ¾Ò»¸ö±àÂë£¬ÕâÑùÈ¡³öÀ´µÄ1¸ö×Ö½Ú¾Í¸»ÓàÁË3¸öbit£¬Õâ3¸öbitÓÃÓÚºÍµÚ¶þ¸ö×Ö½ÚµÄºóÁ½¸öbit½øÐÐ×éºÏ£¬ÔÙ´ÎÐÎ³É±àÂëÖµ£¬Èç´ËÀàÍÆ


            int[] prefix = new int[StackSize];//ÓÃÓÚ±£´æÇ°×ºµÄ¼¯ºÏ
            int[] suffix = new int[StackSize];//ÓÃÓÚ±£´æºó×º
            int[] pixelStatck = new int[StackSize + 1];//ÓÃÓÚÁÙÊ±±£´æÊý¾ÝÁ÷

            int top = 0;
            int count = 0;//ÔÚÏÂÃæµÄÑ­»·ÖÐ£¬Ã¿´Î»á»ñÈ¡Ò»¶¨Á¿µÄ±àÂëµÄ×Ö½ÚÊý×é£¬¶ø´¦ÀíÕâÐ©Êý×éµÄÊ±ºòÐèÒª1¸ö¸ö×Ö½ÚÀ´´¦Àí£¬count¾ÍÊÇ±íÊ¾»¹Òª´¦ÀíµÄ×Ö½ÚÊýÄ¿
            int bi = 0;//count±íÊ¾»¹Ê£¶àÉÙ×Ö½ÚÐèÒª´¦Àí£¬¶øbiÔò±íÊ¾±¾´ÎÒÑ¾­´¦ÀíµÄ¸öÊý
            int xyz = 0;//i´ú±íµ±Ç°´¦ÀíµÃµ½ÏñËØÊý

            int data = 0;//±íÊ¾µ±Ç°´¦ÀíµÄÊý¾ÝµÄÖµ
            int first = 0;//Ò»¸ö×Ö·û´®ÖØµÄµÚÒ»¸ö×Ö½Ú
            int inCode = NullCode; //ÔÚlzwÖÐ£¬Èç¹ûÈÏÊ¶ÁËÒ»¸ö±àÂëËù´ú±íµÄÊý¾Ýentry£¬Ôò½«±àÂë×÷ÎªÏÂÒ»´ÎµÄprefix£¬´Ë´¦inCode´ú±í´«µÝ¸øÏÂÒ»´Î×÷ÎªÇ°×ºµÄ±àÂëÖµ

            //ÏÈÉú³ÉÔªÊý¾ÝµÄÇ°×º¼¯ºÏºÍºó×º¼¯ºÏ£¬ÔªÊý¾ÝµÄÇ°×º¾ùÎª0£¬¶øºó×ºÓëÔªÊý¾ÝÏàµÈ£¬Í¬Ê±±àÂëÒ²ÓëÔªÊý¾ÝÏàµÈ
            for (code = 0; code < clearCode; code++)
            {
                //Ç°×º³õÊ¼Îª0
                prefix[code] = 0;
                //ºó×º=ÔªÊý¾Ý=±àÂë
                suffix[code] = (byte)code;
            }

            byte[] buffer = null;
            while (xyz < pixels.Length)
            {
                //×î´óÏñËØÊýÒÑ¾­È·¶¨ÎªpixelCount = width * width
                if (top == 0)
                {
                    if (bits < codeSize)
                    {
                        //Èç¹ûµ±Ç°µÄÒª´¦ÀíµÄbitÊýÐ¡ÓÚ±àÂëÎ»´óÐ¡£¬ÔòÐèÒª¼ÓÔØÊý¾Ý
                        if (count == 0)
                        {
                            //Èç¹ûcountÎª0£¬±íÊ¾Òª´Ó±àÂëÁ÷ÖÐ¶ÁÒ»¸öÊý¾Ý¶ÎÀ´½øÐÐ·ÖÎö
                            buffer = ReadBlock();
                            count = buffer.Length;
                            if (count == 0)
                            {
                                //ÔÙ´ÎÏë¶ÁÈ¡Êý¾Ý¶Î£¬È´Ã»ÓÐ¶Áµ½Êý¾Ý£¬´ËÊ±¾Í±íÃ÷ÒÑ¾­´¦ÀíÍêÁË
                                break;
                            }
                            //ÖØÐÂ¶ÁÈ¡Ò»¸öÊý¾Ý¶Îºó£¬Ó¦¸Ã½«ÒÑ¾­´¦ÀíµÄ¸öÊýÖÃ0
                            bi = 0;
                        }
                        //»ñÈ¡±¾´ÎÒª´¦ÀíµÄÊý¾ÝµÄÖµ
                        data += buffer[bi] << bits;//´Ë´¦ÎªºÎÒªÒÆÎ»ÄØ£¬±ÈÈçµÚÒ»´Î´¦ÀíÁË1¸ö×Ö½ÚÎª176£¬µÚÒ»´ÎÖ»Òª´¦Àí5bit¾Í¹»ÁË£¬Ê£ÏÂ3bitÁô¸øÏÂ¸ö×Ö½Ú½øÐÐ×éºÏ¡£Ò²¾ÍÊÇµÚ¶þ¸ö×Ö½ÚµÄºóÁ½Î»+µÚÒ»¸ö×Ö½ÚµÄÇ°ÈýÎ»×é³ÉµÚ¶þ´ÎÊä³öÖµ
                        bits += 8;//±¾´ÎÓÖ´¦ÀíÁËÒ»¸ö×Ö½Ú£¬ËùÒÔÐèÒª+8                    
                        bi++;//½«´¦ÀíÏÂÒ»¸ö×Ö½Ú
                        count--;//ÒÑ¾­´¦Àí¹ýµÄ×Ö½ÚÊý+1
                        continue;
                    }
                    //Èç¹ûÒÑ¾­ÓÐ×ã¹»µÄbitÊý¿É¹©´¦Àí£¬ÏÂÃæ¾ÍÊÇ´¦Àí¹ý³Ì
                    //»ñÈ¡±àÂë
                    code = data & code_mask;//»ñÈ¡dataÊý¾ÝµÄ±àÂëÎ»´óÐ¡bitµÄÊý¾Ý
                    data >>= codeSize;//½«±àÂëÊý¾Ý½ØÈ¡ºó£¬Ô­À´µÄÊý¾Ý¾ÍÊ£ÏÂ¼¸¸öbitÁË£¬´ËÊ±½«ÕâÐ©bitÓÒÒÆ£¬ÎªÏÂ´Î×÷×¼±¸
                    bits -= codeSize;//Í¬Ê±ÐèÒª½«µ±Ç°Êý¾ÝµÄbitÊý¼õÈ¥±àÂëÎ»³¤£¬ÒòÎªÒÑ¾­µÃµ½ÁË´¦Àí¡£

                    //ÏÂÃæ¸ù¾Ý»ñÈ¡µÄcodeÖµÀ´½øÐÐ´¦Àí

                    if (code > availableCode || code == endCode)
                    {
                        //µ±±àÂëÖµ´óÓÚ×î´ó±àÂëÖµ»òÕßÎª½áÊø±ê¼ÇµÄÊ±ºò£¬Í£Ö¹´¦Àí                     
                        break;
                    }
                    if (code == clearCode)
                    {
                        //Èç¹ûµ±Ç°ÊÇÇå³ý±ê¼Ç£¬ÔòÖØÐÂ³õÊ¼»¯±äÁ¿£¬ºÃÖØÐÂÔÙÀ´
                        codeSize = dataSize + 1;
                        //ÖØÐÂ³õÊ¼»¯×î´ó±àÂëÖµ
                        code_mask = (1 << codeSize) - 1;
                        //³õÊ¼»¯ÏÂÒ»²½Ó¦¸Ã´¦ÀíµÃ±àÂëÖµ
                        availableCode = clearCode + 2;
                        //½«±£´æµ½old_codeÖÐµÄÖµÇå³ý£¬ÒÔ±ãÖØÍ·ÔÙÀ´
                        old_code = NullCode;
                        continue;
                    }
                    //ÏÂÃæÊÇcodeÊôÓÚÄÜÑ¹ËõµÄ±àÂë·¶Î§ÄÚµÄµÄ´¦Àí¹ý³Ì
                    if (old_code == NullCode)
                    {
                        //Èç¹ûµ±Ç°±àÂëÖµÎª¿Õ,±íÊ¾ÊÇµÚÒ»´Î»ñÈ¡±àÂë
                        pixelStatck[top++] = suffix[code];//»ñÈ¡µ½1¸öÊý¾ÝÁ÷µÄÊý¾Ý
                        //±¾´Î±àÂë´¦ÀíÍê³É£¬½«±àÂëÖµ±£´æµ½old_codeÖÐ
                        old_code = code;
                        //µÚÒ»¸ö×Ö·ûÎªµ±Ç°±àÂë
                        first = code;
                        continue;
                    }
                    inCode = code;
                    if (code == availableCode)
                    {
                        //Èç¹ûµ±Ç°±àÂëºÍ±¾´ÎÓ¦¸ÃÉú³ÉµÄ±àÂëÏàÍ¬
                        pixelStatck[top++] = (byte)first;//ÄÇÃ´ÏÂÒ»¸öÊý¾Ý×Ö½Ú¾ÍµÈÓÚµ±Ç°´¦Àí×Ö·û´®µÄµÚÒ»¸ö×Ö½Ú
                        code = old_code; //»ØËÝµ½ÉÏÒ»¸ö±àÂë
                    }
                    while (code > clearCode)
                    {
                        //Èç¹ûµ±Ç°±àÂë´óÓÚÇå³ý±ê¼Ç£¬±íÊ¾±àÂëÖµÊÇÄÜÑ¹ËõÊý¾ÝµÄ
                        pixelStatck[top++] = suffix[code];
                        code = prefix[code];//»ØËÝµ½ÉÏÒ»¸ö±àÂë
                    }
                    first = suffix[code];

                    //»ñÈ¡ÏÂÒ»¸öÊý¾Ý
                    pixelStatck[top++] = suffix[code];

                    if (availableCode < StackSize) // Fix for Gifs that have "deferred clear code" as per here : https://bugzilla.mozilla.org/show_bug.cgi?id=55918
                    {
                        //ÉèÖÃµ±Ç°Ó¦¸Ã±àÂëÎ»ÖÃµÄÇ°×º
                        prefix[availableCode] = old_code;
                        //ÉèÖÃµ±Ç°Ó¦¸Ã±àÂëÎ»ÖÃµÄºó×º
                        suffix[availableCode] = first;
                        //ÏÂ´ÎÓ¦¸ÃµÃµ½µÄ±àÂëÖµ
                        availableCode++;
                        if (availableCode == code_mask + 1 && availableCode < StackSize)
                        {
                            //Ôö¼Ó±àÂëÎ»Êý
                            codeSize++;
                            //ÖØÉè×î´ó±àÂëÖµ
                            code_mask = (1 << codeSize) - 1;
                        }
                    }
                    //»¹Ô­old_code
                    old_code = inCode;
                }
                //»ØËÝµ½ÉÏÒ»¸ö´¦ÀíÎ»ÖÃ
                top--;
                //»ñÈ¡ÔªÊý¾Ý              
                pixels[xyz++] = (byte)pixelStatck[top];
            }

            #endregion

            return pixels;
        }

        private byte[] ReadBlock()
        {   
            // Reads the next data block from the stream. A data block begins with a byte,
            // which defines the size of the block, followed by the block itself.
            int blockSize = _stream.ReadByte();

            return ReadBytes(blockSize);
        }

        private byte[] ReadBytes(int length)
        {
            // Reads the specified number of bytes from the data stream.
            byte[] buffer = new byte[length];

            _stream.Read(buffer, 0, length);

            return buffer;
        }
    }

    /// <summary>
    /// Decodes GIF files from stream.
    /// </summary>
    internal class GifDecoder : IImageDecoder
    {
        #region Constants

        private const byte ExtensionIntroducer = 0x21;
        private const byte Terminator = 0;
        private const byte ImageLabel = 0x2C;
        private const byte EndIntroducer = 0x3B;
        private const byte ApplicationExtensionLabel = 0xFF;
        private const byte CommentLabel = 0xFE;
        private const byte ImageDescriptorLabel = 0x2C;
        private const byte PlainTextLabel = 0x01;
        private const byte GraphicControlLabel = 0xF9;

        #endregion

        #region Fields

        private ExtendedImage _image;
        private Stream _stream;
        private GifLogicalScreenDescriptor _logicalScreenDescriptor;
        private byte[] _globalColorTable;
        private byte[] _currentFrame;
        private GifGraphicsControlExtension _graphicsControl;

        #endregion

        #region IImageDecoder Members

        /// <summary>
        /// Gets the size of the header for this image type.
        /// </summary>
        /// <value>The size of the header.</value>
        public int HeaderSize
        {
            get { return 6; }
        }

        /// <summary>
        /// Indicates if the image decoder supports the specified
        /// file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>
        /// 	<c>true</c>, if the decoder supports the specified
        /// extensions; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="extension"/>
        /// is null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="extension"/> is a string
        /// of length zero or contains only blanks.</exception>
        public bool IsSupportedFileExtension(string extension)
        {
            Guard.NotNullOrEmpty(extension, "extension");

			#if UNITY_WINRT && !UNITY_EDITOR
			string extensionAsUpper = extension.ToUpper();
			#else
            string extensionAsUpper = extension.ToUpper(CultureInfo.CurrentCulture);
			#endif
            return extensionAsUpper == "GIF";
        }

        /// <summary>
        /// Indicates if the image decoder supports the specified
        /// file header.
        /// </summary>
        /// <param name="header">The file header.</param>
        /// <returns>
        /// <c>true</c>, if the decoder supports the specified
        /// file header; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="header"/>
        /// is null (Nothing in Visual Basic).</exception>
        public bool IsSupportedFileFormat(byte[] header)
        {
            bool isGif = false;

            if (header.Length >= 6)
            {
                isGif =
                    header[0] == 0x47 && // G
                    header[1] == 0x49 && // I
                    header[2] == 0x46 && // F
                    header[3] == 0x38 && // 8
                   (header[4] == 0x39 || header[4] == 0x37) && // 9 or 7
                    header[5] == 0x61;   // a
            }

            return isGif;
        }

        /// <summary>
        /// Decodes the image from the specified stream and sets
        /// the data to image.
        /// </summary>
        /// <param name="image">The image, where the data should be set to.
        /// Cannot be null (Nothing in Visual Basic).</param>
        /// <param name="stream">The stream, where the image should be
        /// decoded from. Cannot be null (Nothing in Visual Basic).</param>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="image"/> is null (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="stream"/> is null (Nothing in Visual Basic).</para>
        /// </exception>
        public void Decode(ExtendedImage image, Stream stream)
        {
            _image  = image;

            _stream = stream;
            _stream.Seek(6, SeekOrigin.Current);

            ReadLogicalScreenDescriptor();

            if (_logicalScreenDescriptor.GlobalColorTableFlag == true)
            {
                _globalColorTable = new byte[_logicalScreenDescriptor.GlobalColorTableSize * 3];

                // Read the global color table from the stream
                stream.Read(_globalColorTable, 0, _globalColorTable.Length);
            }

            int nextFlag = stream.ReadByte();
            while (nextFlag != 0)
            {
                if (nextFlag == ImageLabel)
                {
                    ReadFrame();
                }
                else if (nextFlag == ExtensionIntroducer)
                {
                    int gcl = stream.ReadByte();
                    switch (gcl)
                    {
                        case GraphicControlLabel:
                            ReadGraphicalControlExtension();
                            break;
                        case CommentLabel:
                            ReadComments();
                            break;
                        case ApplicationExtensionLabel:
                            Skip(12);
                            break;
                        case PlainTextLabel:
                            Skip(13);
                            break;
                    }
                }
                else if (nextFlag == EndIntroducer)
                {
                    break;
                }
                nextFlag = stream.ReadByte();
            }
        }

        private void ReadGraphicalControlExtension()
        {
            byte[] buffer = new byte[6];

            _stream.Read(buffer, 0, buffer.Length);

            byte packed = buffer[1];

            _graphicsControl = new GifGraphicsControlExtension();
            _graphicsControl.DelayTime = BitConverter.ToInt16(buffer, 2);
            _graphicsControl.TransparencyIndex = buffer[4];
            _graphicsControl.TransparencyFlag = (packed & 0x01) == 1;
            _graphicsControl.DisposalMethod = (DisposalMethod)((packed & 0x1C) >> 2);
        }

        private GifImageDescriptor ReadImageDescriptor()
        {
            byte[] buffer = new byte[9];

            _stream.Read(buffer, 0, buffer.Length);

            byte packed = buffer[8];

            GifImageDescriptor imageDescriptor = new GifImageDescriptor();
            imageDescriptor.Left = BitConverter.ToInt16(buffer, 0);
            imageDescriptor.Top = BitConverter.ToInt16(buffer, 2);
            imageDescriptor.Width = BitConverter.ToInt16(buffer, 4);
            imageDescriptor.Height = BitConverter.ToInt16(buffer, 6);
            imageDescriptor.LocalColorTableFlag = ((packed & 0x80) >> 7) == 1;
            imageDescriptor.LocalColorTableSize = 2 << (packed & 0x07);
            imageDescriptor.InterlaceFlag = ((packed & 0x40) >> 6) == 1;

            return imageDescriptor;
        }

        private void ReadLogicalScreenDescriptor()
        {
            byte[] buffer = new byte[7];

            _stream.Read(buffer, 0, buffer.Length);

            byte packed = buffer[4];

            _logicalScreenDescriptor = new GifLogicalScreenDescriptor();
            _logicalScreenDescriptor.Width = BitConverter.ToInt16(buffer, 0);
            _logicalScreenDescriptor.Height = BitConverter.ToInt16(buffer, 2);
            _logicalScreenDescriptor.Background = buffer[5];
            _logicalScreenDescriptor.GlobalColorTableFlag = ((packed & 0x80) >> 7) == 1;
            _logicalScreenDescriptor.GlobalColorTableSize = 2 << (packed & 0x07);
        }

        private void Skip(int length)
        {
            _stream.Seek(length, SeekOrigin.Current);

            int flag = 0;

            while ((flag = _stream.ReadByte()) != 0)
            {
                _stream.Seek(flag, SeekOrigin.Current);
            }
        }

        private void ReadComments()
        {
            int flag = 0;

            while ((flag = _stream.ReadByte()) != 0)
            {
                byte[] buffer = new byte[flag]; 
                
                _stream.Read(buffer, 0, flag);

                _image.Properties.Add(new ImageProperty("Comments", BitConverter.ToString(buffer)));
            }
        }

        private void ReadFrame()
        {
            GifImageDescriptor imageDescriptor = ReadImageDescriptor();

            byte[] localColorTable = ReadFrameLocalColorTable(imageDescriptor);

            byte[] indices = ReadFrameIndices(imageDescriptor);

            // Determine the color table for this frame. If there is a local one, use it
            // otherwise use the global color table.
            byte[] colorTable = localColorTable != null ? localColorTable : _globalColorTable;

            ReadFrameColors(indices, colorTable, imageDescriptor);

            Skip(0); // skip any remaining blocks
        }

        private byte[] ReadFrameIndices(GifImageDescriptor imageDescriptor)
        {
            int dataSize = _stream.ReadByte();

            LZWDecoder lzwDecoder = new LZWDecoder(_stream);

            byte[] indices = lzwDecoder.DecodePixels(imageDescriptor.Width, imageDescriptor.Height, dataSize);
            return indices;
        }

        private byte[] ReadFrameLocalColorTable(GifImageDescriptor imageDescriptor)
        {
            byte[] localColorTable = null;

            if (imageDescriptor.LocalColorTableFlag == true)
            {
                localColorTable = new byte[imageDescriptor.LocalColorTableSize * 3];

                _stream.Read(localColorTable, 0, localColorTable.Length);
            }

            return localColorTable;
        }

        private void ReadFrameColors(byte[] indices, byte[] colorTable, GifImageDescriptor descriptor)
        {
            int imageWidth  = _logicalScreenDescriptor.Width;
            int imageHeight = _logicalScreenDescriptor.Height;

            if (_currentFrame == null)
            {
                _currentFrame = new byte[imageWidth * imageHeight * 4];
            }

            byte[] lastFrame = null;

            if (_graphicsControl != null &&
                _graphicsControl.DisposalMethod == DisposalMethod.RestoreToPrevious)
            {
                lastFrame = new byte[imageWidth * imageHeight * 4];

                Array.Copy(_currentFrame, lastFrame, lastFrame.Length);
            }

            int offset = 0, i = 0, index = -1;

            int iPass = 0; // the interlace pass
            int iInc = 8; // the interlacing line increment
            int iY = 0; // the current interlaced line
            int writeY = 0; // the target y offset to write to

            for (int y = descriptor.Top; y < descriptor.Top + descriptor.Height; y++)
            {
                // Check if this image is interlaced.
                if (descriptor.InterlaceFlag)
                {
                    // If so then we read lines at predetermined offsets.
                    // When an entire image height worth of offset lines has been read we consider this a pass.
                    // With each pass the number of offset lines changes and the starting line changes.
                    if (iY >= descriptor.Height)
                    {
                        iPass++;
                        switch (iPass)
                        {
                            case 1:
                                iY = 4;
                                break;
                            case 2:
                                iY = 2;
                                iInc = 4;
                                break;
                            case 3:
                                iY = 1;
                                iInc = 2;
                                break;
                        }
                    }

                    writeY = iY + descriptor.Top;

                    iY += iInc;
                }
                else
                {
                    writeY = y;
                }

                for (int x = descriptor.Left; x < descriptor.Left + descriptor.Width; x++)
                {
                    offset = writeY * imageWidth + x;

                    index = indices[i];

                    if (_graphicsControl == null ||
                        _graphicsControl.TransparencyFlag == false ||
                        _graphicsControl.TransparencyIndex != index)
                    {
                        _currentFrame[offset * 4 + 0] = colorTable[index * 3 + 0];
                        _currentFrame[offset * 4 + 1] = colorTable[index * 3 + 1];
                        _currentFrame[offset * 4 + 2] = colorTable[index * 3 + 2];
                        _currentFrame[offset * 4 + 3] = (byte)255;
                    }

                    i++;
                }
            }

            byte[] pixels = new byte[imageWidth * imageHeight * 4];

            Array.Copy(_currentFrame, pixels, pixels.Length);

            ImageBase currentImage = null;

            if (_image.Pixels == null)
            {
                currentImage = _image;
                currentImage.SetPixels(imageWidth, imageHeight, pixels);
            }
            else
            {
                ImageFrame frame = new ImageFrame();

                currentImage = frame;
                currentImage.SetPixels(imageWidth, imageHeight, pixels);

                _image.Frames.Add(frame);
            }

            if (_graphicsControl != null)
            {
                if (_graphicsControl.DelayTime > 0)
                {
                    currentImage.DelayTime = _graphicsControl.DelayTime;
                }

                if (_graphicsControl.DisposalMethod == DisposalMethod.RestoreToBackground)
                {
                    for (int y = descriptor.Top; y < descriptor.Top + descriptor.Height; y++)
                    {
                        for (int x = descriptor.Left; x < descriptor.Left + descriptor.Width; x++)
                        {
                            offset = y * imageWidth + x;

                            _currentFrame[offset * 4 + 0] = 0;
                            _currentFrame[offset * 4 + 1] = 0;
                            _currentFrame[offset * 4 + 2] = 0;
                            _currentFrame[offset * 4 + 3] = 0;
                        }
                    }
                }
                else if (_graphicsControl.DisposalMethod == DisposalMethod.RestoreToPrevious)
                {
                    _currentFrame = lastFrame;
                }
            }
        }

        #endregion
    }
}