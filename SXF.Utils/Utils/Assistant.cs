using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SXF.Utils
{
    public class Assistant
    {
        /// <summary>
        /// 从字符串里随机得到，规定个数的字符串.
        /// </summary>
        public static string GetRandomCode(string allChar, int CodeCount)
        {
            string[] allCharArray = allChar.Split(',');
            string RandomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < CodeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
                }

                int t = rand.Next(allCharArray.Length - 1);

                while (temp == t)
                {
                    t = rand.Next(allCharArray.Length - 1);
                }

                temp = t;
                RandomCode += allCharArray[t];
            }
            return RandomCode;
        }
        /// <summary>
        /// 生成验证码
        /// </summary>
        public static string GetRandomCode(int CodeCount)
        {
            string allChar = "2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
            return GetRandomCode(allChar, CodeCount);
        }
    }
}
