using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace BuyUnion.Models
{
    public class ImageResizer
    {
        public ImageResizer(string name, int width, int height, string image = null, int pWidth = 0, int pHeight = 0)
        {
            Name = name;
            Width = width;
            Height = height;
            ImageUrl = image;
            PreviewWidth = pWidth > 0 ? pWidth : width;
            PreviewHeight = pHeight > 0 ? pHeight : height;
        }

        public string ImageUrl { get; set; }

        private string pPreviewUrl;

        public string PreviewUrl
        {
            get
            {
                if (pPreviewUrl == null)
                {
                    pPreviewUrl = string.IsNullOrWhiteSpace(ImageUrl)
                        ? $"~/Content/Images/view/avatar_m@3x.png?w={PreviewWidth}&h={PreviewHeight}&scale=canvas&Bgcolor=f6f6f6"
                        : Comm.ResizeImage(ImageUrl, PreviewWidth, PreviewHeight, image: Enums.DummyImage.Avatar);
                }
                return pPreviewUrl;
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Name { get; set; }

        public int PreviewWidth { get; set; }

        public int PreviewHeight { get; set; }

        public bool AutoInit { get; set; } = true;
    }
}