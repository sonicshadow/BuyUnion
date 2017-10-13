using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.WeChatPay
{
    public class QRCodeImage
    {
        public QRCodeImage(string data)
        {

            ThoughtWorks.QRCode.Codec.QRCodeEncoder qrCodeEncoder = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.M;
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeScale = 4;

            //将字符串生成二维码图片
            System.Drawing.Bitmap image = qrCodeEncoder.Encode(data, System.Text.Encoding.Default);

            //保存为PNG到内存流  
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            ////输出二维码图片
            //Response.BinaryWrite(ms.GetBuffer());
            //Response.End();
            file = new System.Web.Mvc.FileContentResult(ms.GetBuffer(), "image/png");
        }

        private System.Web.Mvc.FileContentResult file;

        public System.Web.Mvc.FileContentResult File { get { return file; } }
    }
}