using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amirhome.CustomHelpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString turnToPersianNumber(this HtmlHelper helper, string value)
        {
            if (string.IsNullOrEmpty(value))
                return new MvcHtmlString(String.Empty);
            char[] englishNumbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            char[] persianNumbers = { '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹', '۰' };
            for (int i = 0; i < 10; i++)
            {
                value = value.Replace(englishNumbers[i], persianNumbers[i]);
            }
            return new MvcHtmlString(value);
        }
        public static MvcHtmlString SplitInParts(this HtmlHelper helper, string text, int size)
        {
            if (string.IsNullOrEmpty(text))
                return new MvcHtmlString(String.Empty);
            List<String> ret = new List<String>(((text.Length + size - 1) / size) + 1);
            if ((text.Length + size) % size != 0)
                ret.Add(text.Substring(0, (text.Length + size) % size));

            for (int start = (text.Length + size) % size; start < text.Length; start += size)
            {
                if ((start + size) <= text.Length)
                    ret.Add(text.Substring(start, size));
                else
                    ret.Add(text.Substring(start, (text.Length + size) % size));
            }
            string result = String.Join(",", ret) + " تومان";
            return turnToPersianNumber(helper, result);
        }
        public static MvcHtmlString gregorianToJalali(this HtmlHelper helper, string date)
        {
            if (string.IsNullOrEmpty(date))
                return new MvcHtmlString(String.Empty);
            string[] splited = date.Split('/');
            int m = int.Parse(splited[0]),
                d = int.Parse(splited[1]),
                y = int.Parse(splited[2]);
            int[] g_days_in_month = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int[] j_days_in_month = { 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29 };
            int gy = y - 1600;
            int gm = m - 1;
            int gd = d - 1;
            int g_day_number = (365 * gy) + ((gy + 3) / 4) - ((gy + 99) / 100) + ((gy + 399) / 400);
            for (int i = 0; i < gm; i++)
                g_day_number += g_days_in_month[i];
            if (gm > 1 && (((gy % 4 == 0) && (gy % 100 != 0)) || (gy % 400 != 0)))
                ++g_day_number;
            g_day_number += gd;

            int j_day_number = g_day_number - 79;
            int j_np = j_day_number / 12053;
            j_day_number %= 12053;
            int jy = 979 + (33 * j_np) + 4 * (j_day_number / 1461);
            j_day_number %= 1461;
            if (j_day_number >= 366)
            {
                jy += (j_day_number - 1) / 365;
                j_day_number = (j_day_number - 1) % 365;
            }
            int index;
            for (index = 0; index < 11 && j_day_number >= j_days_in_month[index]; index++)
            {
                j_day_number -= j_days_in_month[index];
            }
            int jm = index + 1;
            int jd = j_day_number + 1;
            string result = jy.ToString() + "/" + jm.ToString() + "/" + jd.ToString();

            return turnToPersianNumber(helper, result);
        }
    }
}