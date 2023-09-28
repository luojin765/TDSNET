using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TDSNET.Engine.Utils
{
    class SpellCN
    {
        public static string GetSpellCodeWithBuffer(ReadOnlySpan<char> CnStr, ConcurrentDictionary<char, char> SpellDict)
        {
            StringBuilder strTemp = new StringBuilder(256);

            strTemp.Clear();
            int iLen = CnStr.Length;

            int i = 0;

            for (i = 0; i <= iLen - 1; i++)
            {
                char c = char.ToUpper(CnStr[i]);

                if (!SpellDict.ContainsKey(c))
                {
                    SpellDict.TryAdd(c, GetCharSpellCode(c));
                }

                if (SpellDict.TryGetValue(c, out char value))
                {
                    strTemp.Append(value);
                }
            }
            return strTemp.ToString();
        }

        public static string GetSpellCode(ReadOnlySpan<char> CnStr, ConcurrentDictionary<char, char> SpellDict = null)
        {
            if (SpellDict == null)
            {
                return GetSpellCodeWithOutBuffer(CnStr);
            }
            else
            {
                return GetSpellCodeWithBuffer(CnStr, SpellDict);
            }
        }


        public static string GetSpellCodeWithOutBuffer(ReadOnlySpan<char> CnStr)
        {
            var strTemp = new char[CnStr.Length];

            int iLen = CnStr.Length;

            int i = 0;

            for (i = 0; i < iLen; i++)
            {

                strTemp[i]=GetCharSpellCode(char.ToUpper(CnStr[i]));
            }
            return new string(strTemp);
        }

        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name="CnChar">单个汉字</param>
        /// <returns>单个大写字母</returns>

        private static char GetCharSpellCode(char CnChar)
        {
            long iCnChar;
            Encoding gb2312 = Encoding.GetEncoding("gb2312");
            byte[] ZW = gb2312.GetBytes(CnChar.ToString());


            //如果是字母，则直接返回

            if (ZW.Length == 1)
            {

                return CnChar;

            }

            else
            {

                // get the array of byte from the single char

                int i1 = ZW[0];

                int i2 = ZW[1];

                iCnChar = i1 * 256 + i2;

            }

            // iCnChar match the constant

            return  iCnChar switch
            {
                >= 45217 and <= 45252 => 'A',
                >= 45253 and <= 45760 => 'B',
                >= 45761 and <= 46317 => 'C',
                >= 46318 and <= 46825 => 'D',
                >= 46826 and <= 47009 => 'E',
                >= 47010 and <= 47296 => 'F',
                >= 47297 and <= 47613 => 'G',
                >= 47614 and <= 48118 => 'H',
                >= 48119 and <= 49061 => 'J',
                >= 49062 and <= 49323 => 'K',
                >= 49324 and <= 49895 => 'L',
                >= 49896 and <= 50370 => 'M',
                >= 50371 and <= 50613 => 'N',
                >= 50614 and <= 50621 => 'O',
                >= 50622 and <= 50905 => 'P',
                >= 50906 and <= 51386 => 'Q',
                >= 51387 and <= 51445 => 'R',
                >= 51446 and <= 52217 => 'S',
                >= 52218 and <= 52697 => 'T',
                >= 52698 and <= 52979 => 'W',
                >= 52980 and <= 53688 => 'X',
                >= 53689 and <= 54480 => 'Y',
                >= 54481 and <= 65289 => 'Z', 
                _=> CnChar
            };           
        }
    }
}
