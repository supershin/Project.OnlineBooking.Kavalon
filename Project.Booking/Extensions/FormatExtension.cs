using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Extensions
{
    public static class FormatExtension
    {
        private static Random random = new Random();        
        public static string StandardNumberFormat = ConfigurationManager.AppSettings["UI.StandardNumberFormat"];
        public static string StandardDateFormat = ConfigurationManager.AppSettings["UI.StandardDateFormat"];
        public static string StandardDateFormatCalendar = ConfigurationManager.AppSettings["UI.StandardDateFormatCalendar"];
        public static string StandardDateTimeFormat = ConfigurationManager.AppSettings["UI.StandardDateTimeFormat"];
        public static string StandardCulture = ConfigurationManager.AppSettings["UI.PageCulture"];
        public static string StandardDateStampFormat = "yyyyMMddHHmmssffff";
        public static string StandardTimeFormat = ConfigurationManager.AppSettings["UI.TimeFormat"];

        public static int AsInt(this int? value1)
        {
            return value1.HasValue ? value1.Value : 0;
        }
        public static Guid AsGuid(this Guid? value1)
        {
            return value1.HasValue ? value1.Value : Guid.Empty;
        }
        public static DateTime AsDate(this DateTime? value1)
        {
            return value1.HasValue ? value1.Value : DateTime.Now;
        }
        public static bool AsBool(this bool? value1)
        {
            return value1.HasValue ? value1.Value : false;
        }
        public static decimal AsDecimal(this decimal? value1)
        {
            return value1.HasValue ? value1.Value : 0;
        }

        public static int? ToInt(this string str)
        {
            int result;
            if (int.TryParse(str, out result))
                return result;

            return null;
        }
        public static DateTime? ToDate(this string str)
        {
            DateTime result;
            if (!string.IsNullOrEmpty(str.ToStringNullable()))
            {
                if (DateTime.TryParse(str, out result))
                    return result;
            }
            return null;
        }
        public static string ToStringNullable(this string param)
        {
            return string.IsNullOrEmpty(param) ? null : param.Trim();
        }
        public static string ToStringEmpty(this string param)
        {
            return string.IsNullOrEmpty(param) ? string.Empty : param.Trim();
        }

        public static string ToStringNumber(this int number, bool returnZeroEmpty = false, string Formater = "###,###,##0")
        {
            if (returnZeroEmpty && number == 0)
                return string.Empty;
            return number.ToString(Formater);
        }
        public static string ToStringNumber(this int number, string returnIfZero, string Formater = "###,###,##0")
        {
            if (number == 0)
                return returnIfZero;
            return number.ToString(Formater);
        }
        public static string ToStringNumber(this float number, bool returnZeroEmpty = false)
        {
            if (returnZeroEmpty && number == 0)
                return string.Empty;
            return string.Format(StandardNumberFormat, number);
        }
        public static string ToStringNumber(this decimal number, bool returnZeroEmpty = false)
        {
            if (returnZeroEmpty && number == 0)
                return string.Empty;
            return string.Format(StandardNumberFormat, number);
        }
        public static string ToStringNumber(this decimal number, string returnIfZero)
        {
            if (number == 0)
                return returnIfZero;
            return string.Format(StandardNumberFormat, number);
        }
        public static string ToStringNumber(this int? number, bool returnZeroEmpty = false)
        {
            if (number.HasValue)
            {
                if (returnZeroEmpty && number.Value == 0)
                    return string.Empty;
            }
            else
            {
                if (returnZeroEmpty)
                    return string.Empty;
            }

            return number.HasValue ? number.ToString() : "0";
        }
        public static string ToStringNumber(this float? number, bool returnZeroEmpty = false)
        {
            if (number.HasValue)
            {
                if (returnZeroEmpty && number.Value == 0)
                    return string.Empty;
            }
            else
            {
                if (returnZeroEmpty)
                    return string.Empty;
            }

            return number.HasValue ? string.Format(StandardNumberFormat, number.Value) : "0.00";

        }
        public static string ToStringNumber(this decimal? number, bool returnZeroEmpty = false)
        {
            if (number.HasValue)
            {
                if (returnZeroEmpty && number.Value == 0)
                    return string.Empty;
            }
            else
            {
                if (returnZeroEmpty)
                    return string.Empty;
            }

            return number.HasValue ? string.Format(StandardNumberFormat, number.Value) : "0.00";
        }
        public static string ToStringNumber(this double? number, bool returnZeroEmpty = false)
        {
            if (number.HasValue)
            {
                if (returnZeroEmpty && number.Value == 0)
                    return string.Empty;
            }
            else
            {
                if (returnZeroEmpty)
                    return string.Empty;
            }

            return number.HasValue ? string.Format(StandardNumberFormat, number.Value) : "0.00";
        }
        public static string ToStringNumber(this double number)
        {
            return string.Format(StandardNumberFormat, number);
        }

        public static string ToStringDateTime(this DateTime dateValue)
        {
            var culture = CultureInfo.CreateSpecificCulture(StandardCulture);
            return dateValue.ToString(StandardDateTimeFormat, culture);
        }
        public static string ToStringDate(this DateTime? dateValue)
        {

            var culture = CultureInfo.CreateSpecificCulture(StandardCulture);
            return dateValue.HasValue ? dateValue.Value.ToString(StandardDateFormat, culture) : string.Empty;
        }

        public static string Right(this string sValue, int iMaxLength)
        {
            //Check if the value is valid
            if (string.IsNullOrEmpty(sValue))
            {
                //Set valid empty string as string could be null
                sValue = string.Empty;
            }
            else if (sValue.Length > iMaxLength)
            {
                //Make the string no longer than the max length
                sValue = sValue.Substring(sValue.Length - iMaxLength, iMaxLength);
            }

            //Return the string
            return sValue;
        }

        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }

        public static List<T> ConvertDataTable<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        var value = dr[column.ColumnName];
                        if (dr.IsNull(column.ColumnName))
                        {
                            value = null;
                        }
                        pro.SetValue(obj, value, null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
