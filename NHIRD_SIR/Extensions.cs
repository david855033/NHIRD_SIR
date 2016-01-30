using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHIRD.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// string -> datetime格式轉換, 可接受"yyyy-MM-dd"或"yyyyMMdd"或"yyyyMM"(自動加上01), 錯誤的話回傳dt.MinValue
        /// </summary>
        /// <param name="str_date"></param>
        /// <returns></returns>
        public static DateTime StringToDatetime(this string str_date)
        {
            try
            {
                if (str_date.Trim().Length <= 6)
                {
                    str_date = str_date.Substring(0, 4) + "-" + str_date.Substring(4, 2) + "-01";
                }
                else if (str_date.Length <= 8)
                {
                    str_date = str_date.Substring(0, 4) + "-" + str_date.Substring(4, 2) + "-" + str_date.Substring(6, 2);
                }
                return Convert.ToDateTime(str_date);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// datetime -> string 格式轉換, "yyyy-MM-dd", 如果為dt.Minvalue, 則轉換成null
        /// </summary>
        /// <param name="dt_date"></param>
        /// <returns></returns>
        public static string DatetimeToFormatString(this DateTime dt_date)
        {
            try
            {
                if (dt_date == DateTime.MinValue) return null;
                return dt_date.ToString("yyyy-MM-dd");
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 轉換成字串，化成%顯示
        /// </summary>
        /// <param name="db"></param>
        /// <param name="digit">小數點位數</param>
        /// <returns></returns>
        public static string PercentFormat(this double db, int digit)
        {
            if (db == 0) return "";
            return Math.Round(db * 100, digit) + "%";
        }
        /// <summary>
        /// 轉換成字串，若數值小於等於0則傳回空字串
        /// </summary>
        /// <param name="_int"></param>
        /// <returns></returns>
        public static string NullforZero(this int _int)
        {
            return _int <= 0 ? "" : _int.ToString();
        }
        /// <summary>
        /// 轉換成字串，若數值小於等於0則傳回空字串
        /// </summary>
        /// <param name="_int"></param>
        /// <returns></returns>
        public static string NullforZero(this double _db)
        {
            return _db <= 0 ? "" : _db.ToString();
        }
        /// <summary>
        ///轉換成字串，若數值小於0則傳回空字串
        /// </summary>
        /// <param name="_int"></param>
        /// <returns></returns>
        public static string NullforNeg(this double _db)
        {
            return _db < 0 ? "" : _db.ToString();
        }
        /// <summary>
        /// 四捨五入
        /// </summary>
        /// <param name="_int"></param>
        /// <returns></returns>
        public static double Round(this double _db, int digit)
        {
            return Math.Round(_db, digit);
        }
    }
}
