using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using zgcwkj.Util;
using zgcwkj.Util.Common;

namespace ZFramework.Comm.Common
{
    /// <summary>
    /// 验证码工具
    /// </summary>
    public class ValidateCode
    {
        private string validateCode = "";
        private string validateColor = "FFFFFF";

        /// <summary>
        /// 实例验证码
        /// </summary>
        /// <param name="length">验证码长度</param>
        public ValidateCode(int length = 4)
        {
            validateCode = RandomTool.GetRandomStr(length);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public string GetValidate()
        {
            return validateCode;
        }

        /// <summary>
        /// 背景颜色 不传参数时为随机
        /// </summary>
        /// <param name="color"></param>
        public void GetColor(string color = "")
        {
            if (color == "") validateColor = RandomTool.GetRandomInt(6).ToStr(); else validateColor = color;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        public byte[] GetImage()
        {
            Bitmap image = new Bitmap(validateCode.Length * 20, 35);
            Graphics g = Graphics.FromImage(image);

            try
            {
                ColorConverter ww = new ColorConverter();
                g.Clear((Color)ww.ConvertFromString("#" + validateColor));
                Random random = new Random();

                //画图片的噪音线
                for (int i = 0; i < 12; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
                }

                Font font = new Font("Georgia", 20, FontStyle.Bold | FontStyle.Italic);
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Gray, 1.2f, true);
                g.DrawString(validateCode, font, brush, 0, 0);

                //画图片的噪音点
                for (int i = 0; i < 12; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.White);
                }
                //画图片的噪音线
                for (int i = 0; i < 12; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.FromArgb(random.Next())), x1, y1, x2, y2);
                }

                //画图片的边框线
                //g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                MemoryStream ms = new MemoryStream();

                image.Save(ms, ImageFormat.Gif);

                return ms.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
