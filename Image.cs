using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace RayTracer
{
    struct Pixel
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Pixel(Vector3 color)
        {
            R = (byte)(Math.Clamp(color.X, 0.0f, 0.999f) * 255.99f);
            G = (byte)(Math.Clamp(color.Y, 0.0f, 0.999f) * 255.99f);
            B = (byte)(Math.Clamp(color.Z, 0.0f, 0.999f) * 255.99f);
        }
    }

    class Image
    {
        public const int bytesPerPixel = 3;

        public int Width { get; }
        public int Height { get; }
        public int Stride { get;  }

        private byte[] _pixels { get; }

        public Image(int width, int height)
        {
            Width = width;
            Height = height;
            Stride = width * bytesPerPixel;
            _pixels = new byte[Height * Stride];
        }

        public void SetPixel(Pixel pixel, int x, int y)
        {
            int index = IndexFromXY(x, y);
            _pixels[index] = pixel.B;
            _pixels[index + 1] = pixel.G;
            _pixels[index + 2] = pixel.R;
        }

        public void WriteToTGA(string path)
        {
            using Stream outStream = File.Open(path, FileMode.Create);

            // WriteByte the TGA header data.
            outStream.WriteByte(0);
            outStream.WriteByte(0);
            outStream.WriteByte(2);                                 // Uncompressed RGB
            outStream.WriteByte(0); outStream.WriteByte(0);
            outStream.WriteByte(0); outStream.WriteByte(0);
            outStream.WriteByte(0);
            outStream.WriteByte(0); outStream.WriteByte(0);         // X origin
            outStream.WriteByte(0); outStream.WriteByte(0);         // y origin
            outStream.WriteByte((byte)(Width & 0x00FF));
            outStream.WriteByte((byte)((Width & 0xFF00) / 256));
            outStream.WriteByte((byte)(Height & 0x00FF));
            outStream.WriteByte((byte)((Height & 0xFF00) / 256));
            outStream.WriteByte(24);                                // 24 bit bitmap
            outStream.WriteByte(0);

            // WriteByte the pixel data.
            foreach (var i in _pixels)
            {
                outStream.WriteByte(i);
            }
        }

        private int IndexFromXY(int x, int y)
        {
            return bytesPerPixel * (Width * y + x);
        }
    }
}
