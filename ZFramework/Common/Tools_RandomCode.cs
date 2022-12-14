using System;

namespace ZFramework.Common
{
    /// <summary>
    /// 生成随机码
    /// </summary>
    public class Tools_RandomCode
    {
        private string strType = "";//字符类型

        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="digital">数字</param>
        /// <param name="character">字符</param>
        /// <param name="symbol">符号</param>
        public Tools_RandomCode(bool digital = false, bool character = false, bool symbol = false)
        {
            //数字
            if (digital)
            {
                strType = "1|2|3|4|5|6|7|8|9|0";
            }
            //字符
            if (character)
            {
                if (strType != "") strType += "|";//避免前面不选
                strType += "q|w|e|r|t|y|u|i|o|p|a|s|d|f|g|h|j|k|l|z|x|c|v|b|n|m";
            }
            //符号
            if (symbol)
            {
                if (strType != "") strType += "|";//避免前面不选
                strType += ",|.|/|;|'|[|]|{|}|;|<|>|?|!|@|#|$|%|^|&|*|(|)|_|-|+|=|~";
            }
        }

        /// <summary>
        /// 获取随机码
        /// </summary>
        /// <param name="Length">长度</param>
        /// <param name="excludeStrs">排除字符</param>
        /// <returns></returns>
        public string GoRandom(int Length, params string[] excludeStrs)
        {
            //准备返回的随机码
            string strRandom = "";
            //排除对应的字符
            foreach (var str in excludeStrs) strType = strType.Replace(str + "|", "").Replace(str, "");
            //获取对应的长度
            string[] zf = strType.Split('|');
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                int sjs = rd.Next(zf.Length);
                strRandom += zf[sjs];
            }
            return strRandom;
        }
    }
}