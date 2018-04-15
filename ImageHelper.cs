﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SpritesheetMaker {

    public static class ImageHelper {

        /// <summary>
        /// If the <see cref="Rectangle"/> has negative width or height, transform it so that it hasn't anymore.
        /// </summary>
        private static Rectangle Correct(this Rectangle rect) {
            if (rect.Width < 0) {
                rect.Width = -rect.Width;
                rect.X -= rect.Width;
            }

            if (rect.Height >= 0) return rect;

            rect.Height = -rect.Height;
            rect.Y -= rect.Height;
            return rect;
        }

        /// <summary>
        /// If the <see cref="Rectangle"/> doesn't contain the point, enlarge it so that it does.
        /// </summary>
        /// <param name="rect">The rectangle to modify.</param>
        /// <param name="x">X-coordinate of the point.</param>
        /// <param name="y">Y-coordinate of the point.</param>
        private static Rectangle IncludePoint(this Rectangle rect, int x, int y) {
            return Rectangle.Union(rect,
                new Rectangle(rect.Left, rect.Top, x - rect.Left, y - rect.Top).Correct());
        }

        public static Bitmap Crop(Bitmap bitmap, Rectangle rect) {
            return bitmap.Clone(rect, bitmap.PixelFormat);
        }

        //https://codereview.stackexchange.com/questions/178660/determine-if-an-image-is-opaque-or-transparent

        public static Rectangle FindMinRect(ref Bitmap[] images) {
            var rect = new Rectangle();

            foreach (var img in images) {
                var crop = FindMinRect(img);

                if (rect.IsEmpty) {
                    rect = crop;
                    continue;
                }

                rect = Rectangle.Union(rect, crop);
            }

            return rect;
        }

        /// <summary>
        /// Finds the rectangle which contains the image and the least transparency.
        /// </summary>
        private static Rectangle FindMinRect(Image image) {
            var bitmap = new Bitmap(image);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            var rect = new Rectangle();

            unsafe {
                var p = (byte*) bitmapData.Scan0;

                if (p == null) throw new NullReferenceException();

                var off = 3; // NB apply Roland's observation about endianness here
                var gap = bitmapData.Stride - bitmap.Width * 4;

                for (var y = 0; y < bitmap.Height; y++, off += gap) {
                    for (var x = 0; x < bitmap.Width; x++, off += 4) {
                        if (p[off] == 0) continue;

                        if (rect.IsEmpty) {
                            rect = new Rectangle {
                                X = x,
                                Y = y,
                                Width = 1,
                                Height = 1
                            };
                            continue;
                        }

                        rect = rect.IncludePoint(x, y);
                    }
                }
            }

            bitmap.UnlockBits(bitmapData);

            if (!rect.IsEmpty) {
                rect.Inflate(1, 1);
            }

            return rect;
        }
    }
}