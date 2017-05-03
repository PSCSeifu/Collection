// Decompiled with JetBrains decompiler
// Type: Wyvern.Base.SFStrings
// Assembly: Wyvern.Base, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C96879FF-8A02-4EC1-BB96-AE29A19B1BA7
// Assembly location: D:\WYVERN\AssembliesX\Wyvern.Base.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Wyvern.Base
{
  public class SFStrings
  {
    public static string[] SFSplit(string source, string value)
    {
      return source.Split(new string[1]{ value }, StringSplitOptions.None);
    }

    public static string Capitalize(string value)
    {
      if (value == null || value.Length == 0)
        return string.Empty;
      if (value.Length == 1)
        return value.ToUpper();
      return value.Substring(0, 1).ToUpper() + value.Substring(1);
    }

    public static string CapitalizeEachWord(string value, char separator = ' ')
    {
      string str = string.Empty;
      if (value == null || value.Length == 0)
        return string.Empty;
      if (value.IndexOf(separator) > -1)
      {
        string[] strArray = value.Split(separator);
        for (int index = 0; index <= strArray.Length - 1; ++index)
          strArray[index] = strArray[index].Length != 1 ? strArray[index].Substring(0, 1).ToUpper() + strArray[index].Substring(1) : strArray[index].ToUpper();
        str = string.Join(separator.ToString(), strArray);
      }
      return str;
    }

    public static char CharFromRight(string value, int index)
    {
      if (value == null || value.Length == 0 || (index < 1 || value.Length - index - 1 < 0))
        return Convert.ToChar(string.Empty);
      string str = value;
      int startIndex = str.Length - index - 1;
      int length = 1;
      return Convert.ToChar(str.Substring(startIndex, length));
    }

    public static bool CompareDates(DateTime leftDate, DateTime rightDate)
    {
      return SFStrings.CompareDates(leftDate, rightDate, false);
    }

    public static bool CompareDates(DateTime leftDate, DateTime rightDate, bool fullCompare)
    {
      if (fullCompare)
        return leftDate == rightDate;
      return leftDate.Year == rightDate.Year && leftDate.Month == rightDate.Month && leftDate.Day == rightDate.Day;
    }

    public static void CompareStrings(ref List<TextCompareItem> left, ref List<TextCompareItem> right, string valueLeft, string valueRight)
    {
      left = new List<TextCompareItem>();
      right = new List<TextCompareItem>();
      if (string.IsNullOrEmpty(valueLeft) && string.IsNullOrEmpty(valueRight))
        return;
      string[] strArray1 = valueLeft.Split('\n');
      string[] strArray2 = valueRight.Split('\n');
      int index1 = 0;
      for (int index2 = 0; index2 < strArray1.Length; ++index2)
      {
        if (index1 < strArray2.Length)
        {
          if (strArray1[index2] == strArray2[index1])
          {
            List<TextCompareItem> textCompareItemList1 = left;
            TextCompareItem textCompareItem1 = new TextCompareItem();
            textCompareItem1.IsNotMatch = false;
            string str1 = strArray1[index2];
            textCompareItem1.Text = str1;
            textCompareItemList1.Add(textCompareItem1);
            List<TextCompareItem> textCompareItemList2 = right;
            TextCompareItem textCompareItem2 = new TextCompareItem();
            textCompareItem2.IsNotMatch = false;
            string str2 = strArray2[index1];
            textCompareItem2.Text = str2;
            textCompareItemList2.Add(textCompareItem2);
          }
          else if (index1 < strArray2.Length - 1)
          {
            if (strArray1[index2] == strArray2[index1 + 1])
            {
              left.Add(new TextCompareItem()
              {
                IsNotMatch = true,
                Text = ""
              });
              List<TextCompareItem> textCompareItemList1 = right;
              TextCompareItem textCompareItem1 = new TextCompareItem();
              textCompareItem1.IsNotMatch = true;
              string str1 = strArray2[index1];
              textCompareItem1.Text = str1;
              textCompareItemList1.Add(textCompareItem1);
              ++index1;
              List<TextCompareItem> textCompareItemList2 = left;
              TextCompareItem textCompareItem2 = new TextCompareItem();
              textCompareItem2.IsNotMatch = false;
              string str2 = strArray1[index2];
              textCompareItem2.Text = str2;
              textCompareItemList2.Add(textCompareItem2);
              List<TextCompareItem> textCompareItemList3 = right;
              TextCompareItem textCompareItem3 = new TextCompareItem();
              textCompareItem3.IsNotMatch = false;
              string str3 = strArray2[index1];
              textCompareItem3.Text = str3;
              textCompareItemList3.Add(textCompareItem3);
            }
            else if (index2 > 0)
            {
              if (index2 + 1 < strArray1.Length)
              {
                if (strArray1[index2 + 1] == strArray2[index1])
                {
                  List<TextCompareItem> textCompareItemList1 = left;
                  TextCompareItem textCompareItem1 = new TextCompareItem();
                  textCompareItem1.IsNotMatch = true;
                  string str1 = strArray1[index2];
                  textCompareItem1.Text = str1;
                  textCompareItemList1.Add(textCompareItem1);
                  right.Add(new TextCompareItem()
                  {
                    IsNotMatch = true,
                    Text = ""
                  });
                  List<TextCompareItem> textCompareItemList2 = left;
                  TextCompareItem textCompareItem2 = new TextCompareItem();
                  textCompareItem2.IsNotMatch = false;
                  string str2 = strArray1[index2 + 1];
                  textCompareItem2.Text = str2;
                  textCompareItemList2.Add(textCompareItem2);
                  List<TextCompareItem> textCompareItemList3 = right;
                  TextCompareItem textCompareItem3 = new TextCompareItem();
                  textCompareItem3.IsNotMatch = false;
                  string str3 = strArray2[index1];
                  textCompareItem3.Text = str3;
                  textCompareItemList3.Add(textCompareItem3);
                  ++index2;
                }
                else
                {
                  List<TextCompareItem> textCompareItemList1 = left;
                  TextCompareItem textCompareItem1 = new TextCompareItem();
                  textCompareItem1.IsNotMatch = true;
                  string str1 = strArray1[index2];
                  textCompareItem1.Text = str1;
                  textCompareItemList1.Add(textCompareItem1);
                  List<TextCompareItem> textCompareItemList2 = right;
                  TextCompareItem textCompareItem2 = new TextCompareItem();
                  textCompareItem2.IsNotMatch = true;
                  string str2 = strArray2[index1];
                  textCompareItem2.Text = str2;
                  textCompareItemList2.Add(textCompareItem2);
                }
              }
              else
              {
                List<TextCompareItem> textCompareItemList1 = left;
                TextCompareItem textCompareItem1 = new TextCompareItem();
                textCompareItem1.IsNotMatch = true;
                string str1 = strArray1[index2];
                textCompareItem1.Text = str1;
                textCompareItemList1.Add(textCompareItem1);
                List<TextCompareItem> textCompareItemList2 = right;
                TextCompareItem textCompareItem2 = new TextCompareItem();
                textCompareItem2.IsNotMatch = true;
                string str2 = strArray2[index1];
                textCompareItem2.Text = str2;
                textCompareItemList2.Add(textCompareItem2);
              }
            }
            else
            {
              List<TextCompareItem> textCompareItemList1 = left;
              TextCompareItem textCompareItem1 = new TextCompareItem();
              textCompareItem1.IsNotMatch = true;
              string str1 = strArray1[index2];
              textCompareItem1.Text = str1;
              textCompareItemList1.Add(textCompareItem1);
              List<TextCompareItem> textCompareItemList2 = right;
              TextCompareItem textCompareItem2 = new TextCompareItem();
              textCompareItem2.IsNotMatch = true;
              string str2 = strArray2[index1];
              textCompareItem2.Text = str2;
              textCompareItemList2.Add(textCompareItem2);
            }
          }
          else
          {
            List<TextCompareItem> textCompareItemList1 = left;
            TextCompareItem textCompareItem1 = new TextCompareItem();
            textCompareItem1.IsNotMatch = true;
            string str1 = strArray1[index2];
            textCompareItem1.Text = str1;
            textCompareItemList1.Add(textCompareItem1);
            List<TextCompareItem> textCompareItemList2 = right;
            TextCompareItem textCompareItem2 = new TextCompareItem();
            textCompareItem2.IsNotMatch = true;
            string str2 = strArray2[index1];
            textCompareItem2.Text = str2;
            textCompareItemList2.Add(textCompareItem2);
          }
          ++index1;
        }
        else
        {
          List<TextCompareItem> textCompareItemList = left;
          TextCompareItem textCompareItem = new TextCompareItem();
          textCompareItem.IsNotMatch = true;
          string str = strArray1[index2];
          textCompareItem.Text = str;
          textCompareItemList.Add(textCompareItem);
          right.Add(new TextCompareItem()
          {
            IsNotMatch = true,
            Text = ""
          });
        }
      }
      if (index1 >= strArray2.Length - 1)
        return;
      for (int index2 = index1; index2 < strArray2.Length; ++index2)
      {
        left.Add(new TextCompareItem()
        {
          IsNotMatch = true,
          Text = ""
        });
        List<TextCompareItem> textCompareItemList = right;
        TextCompareItem textCompareItem = new TextCompareItem();
        textCompareItem.IsNotMatch = true;
        string str = strArray2[index2];
        textCompareItem.Text = str;
        textCompareItemList.Add(textCompareItem);
      }
    }

    public static string CreateSummary(string value, int summaryLength)
    {
      if (string.IsNullOrEmpty(value) || summaryLength < 1)
        return string.Empty;
      if (summaryLength >= value.Length)
        return value;
      int num = 0;
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = value.Split(' ');
      stringBuilder.Append(strArray[0]);
      if (strArray.Length != 0)
      {
        for (int index = 1; index <= strArray.Length - 1; ++index)
        {
          num += strArray[index].Length;
          if (num < summaryLength)
          {
            stringBuilder.Append(" ");
            stringBuilder.Append(strArray[index]);
          }
          else
            break;
        }
      }
      stringBuilder.Append("...");
      return stringBuilder.ToString();
    }

    public static string FirstCharToLower(string value)
    {
      if (value == null || value.Length == 0)
        return string.Empty;
      if (value.Length == 1)
        return value.ToLower();
      return value.Substring(0, 1).ToLower() + value.Substring(1);
    }

    public static string FormatBytesToString(double value)
    {
      string[] strArray = new string[9]
      {
        "Bytes",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB",
        "EB",
        "ZB",
        "YB"
      };
      value = Convert.ToDouble(value);
      int length = strArray.Length;
      while (length >= 0)
      {
        if (value >= Math.Pow(1024.0, (double) length))
          return SFStrings.ThreeNonZeroDigits(value / Math.Pow(1024.0, (double) length)) + " " + strArray[length];
        length += -1;
      }
      return string.Empty;
    }

    public static string FromBytes(byte[] bytes)
    {
      char[] chArray = new char[bytes.Length / 2];
      Buffer.BlockCopy((Array) bytes, 0, (Array) chArray, 0, bytes.Length);
      return new string(chArray);
    }

    public static DateTime FromSFCString(string newDate)
    {
      if (string.IsNullOrEmpty(newDate))
        return new DateTime(1900, 1, 1);
      string[] strArray = newDate.Split(':');
      if (strArray.Length != 6)
        return new DateTime(1900, 1, 1);
      try
      {
        return new DateTime(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), Convert.ToInt32(strArray[3]), Convert.ToInt32(strArray[4]), Convert.ToInt32(strArray[5]));
      }
      catch
      {
      }
      return new DateTime(1900, 1, 1);
    }

    public static int GetAgeFromDate(DateTime value)
    {
      if (value.Day == DateTime.Now.Day && value.Month == DateTime.Now.Month && value.Year == DateTime.Now.Year || value > DateTime.Now)
        return 0;
      int int32 = Convert.ToInt32(DateTime.Now.Year - value.Year);
      if (value.Month > DateTime.Now.Month)
        --int32;
      else if (value.Month == DateTime.Now.Month & value.Day > DateTime.Now.Day)
        --int32;
      return int32;
    }

    public static string GetDayAsString(int day, bool fullName = false)
    {
      switch (day)
      {
        case 0:
          return fullName ? "Sunday" : "Sun";
        case 1:
          return fullName ? "Monday" : "Mon";
        case 2:
          return fullName ? "Tuesday" : "Tue";
        case 3:
          return fullName ? "Wednesday" : "Wed";
        case 4:
          return fullName ? "Thursday" : "Thr";
        case 5:
          return fullName ? "Friday" : "Fri";
        case 6:
          return fullName ? "Saturday" : "Sat";
        default:
          return "";
      }
    }

    public static string GetInitials(string value, bool makeUpperCase, bool useReturnPeriod, string returnSeparator = "")
    {
      string[] strArray = value.Split(' ');
      if (value == null || value.Length == 0)
        return string.Empty;
      for (int index = 0; index <= strArray.Length - 1; ++index)
      {
        strArray[index] = strArray[index].Substring(0, 1);
        if (makeUpperCase)
          strArray[index] = strArray[index].ToUpper();
        if (useReturnPeriod)
          strArray[index] = strArray[index] + ".";
      }
      return string.Join(returnSeparator, strArray);
    }

    public static string GetIpsum(int words, int paragraphs, bool isHtml = false)
    {
      string[] strArray = new string[542]
      {
        "consetetur",
        "sadipscing",
        "elitr",
        "sed",
        "diam",
        "nonumy",
        "eirmod",
        "tempor",
        "invidunt",
        "ut",
        "labore",
        "et",
        "dolore",
        "magna",
        "aliquyam",
        "erat",
        "sed",
        "diam",
        "voluptua",
        "at",
        "vero",
        "eos",
        "et",
        "accusam",
        "et",
        "justo",
        "duo",
        "dolores",
        "et",
        "ea",
        "rebum",
        "stet",
        "clita",
        "kasd",
        "gubergren",
        "no",
        "sea",
        "takimata",
        "sanctus",
        "est",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consetetur",
        "sadipscing",
        "elitr",
        "sed",
        "diam",
        "nonumy",
        "eirmod",
        "tempor",
        "invidunt",
        "ut",
        "labore",
        "et",
        "dolore",
        "magna",
        "aliquyam",
        "erat",
        "sed",
        "diam",
        "voluptua",
        "at",
        "vero",
        "eos",
        "et",
        "accusam",
        "et",
        "justo",
        "duo",
        "dolores",
        "et",
        "ea",
        "rebum",
        "stet",
        "clita",
        "kasd",
        "gubergren",
        "no",
        "sea",
        "takimata",
        "sanctus",
        "est",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consetetur",
        "sadipscing",
        "elitr",
        "sed",
        "diam",
        "nonumy",
        "eirmod",
        "tempor",
        "invidunt",
        "ut",
        "labore",
        "et",
        "dolore",
        "magna",
        "aliquyam",
        "erat",
        "sed",
        "diam",
        "voluptua",
        "at",
        "vero",
        "eos",
        "et",
        "accusam",
        "et",
        "justo",
        "duo",
        "dolores",
        "et",
        "ea",
        "rebum",
        "stet",
        "clita",
        "kasd",
        "gubergren",
        "no",
        "sea",
        "takimata",
        "sanctus",
        "est",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "duis",
        "autem",
        "vel",
        "eum",
        "iriure",
        "dolor",
        "in",
        "hendrerit",
        "in",
        "vulputate",
        "velit",
        "esse",
        "molestie",
        "consequat",
        "vel",
        "illum",
        "dolore",
        "eu",
        "feugiat",
        "nulla",
        "facilisis",
        "at",
        "vero",
        "eros",
        "et",
        "accumsan",
        "et",
        "iusto",
        "odio",
        "dignissim",
        "qui",
        "blandit",
        "praesent",
        "luptatum",
        "zzril",
        "delenit",
        "augue",
        "duis",
        "dolore",
        "te",
        "feugait",
        "nulla",
        "facilisi",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consectetuer",
        "adipiscing",
        "elit",
        "sed",
        "diam",
        "nonummy",
        "nibh",
        "euismod",
        "tincidunt",
        "ut",
        "laoreet",
        "dolore",
        "magna",
        "aliquam",
        "erat",
        "volutpat",
        "ut",
        "wisi",
        "enim",
        "ad",
        "minim",
        "veniam",
        "quis",
        "nostrud",
        "exerci",
        "tation",
        "ullamcorper",
        "suscipit",
        "lobortis",
        "nisl",
        "ut",
        "aliquip",
        "ex",
        "ea",
        "commodo",
        "consequat",
        "duis",
        "autem",
        "vel",
        "eum",
        "iriure",
        "dolor",
        "in",
        "hendrerit",
        "in",
        "vulputate",
        "velit",
        "esse",
        "molestie",
        "consequat",
        "vel",
        "illum",
        "dolore",
        "eu",
        "feugiat",
        "nulla",
        "facilisis",
        "at",
        "vero",
        "eros",
        "et",
        "accumsan",
        "et",
        "iusto",
        "odio",
        "dignissim",
        "qui",
        "blandit",
        "praesent",
        "luptatum",
        "zzril",
        "delenit",
        "augue",
        "duis",
        "dolore",
        "te",
        "feugait",
        "nulla",
        "facilisi",
        "nam",
        "liber",
        "tempor",
        "cum",
        "soluta",
        "nobis",
        "eleifend",
        "option",
        "congue",
        "nihil",
        "imperdiet",
        "doming",
        "id",
        "quod",
        "mazim",
        "placerat",
        "facer",
        "possim",
        "assum",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consectetuer",
        "adipiscing",
        "elit",
        "sed",
        "diam",
        "nonummy",
        "nibh",
        "euismod",
        "tincidunt",
        "ut",
        "laoreet",
        "dolore",
        "magna",
        "aliquam",
        "erat",
        "volutpat",
        "ut",
        "wisi",
        "enim",
        "ad",
        "minim",
        "veniam",
        "quis",
        "nostrud",
        "exerci",
        "tation",
        "ullamcorper",
        "suscipit",
        "lobortis",
        "nisl",
        "ut",
        "aliquip",
        "ex",
        "ea",
        "commodo",
        "consequat",
        "duis",
        "autem",
        "vel",
        "eum",
        "iriure",
        "dolor",
        "in",
        "hendrerit",
        "in",
        "vulputate",
        "velit",
        "esse",
        "molestie",
        "consequat",
        "vel",
        "illum",
        "dolore",
        "eu",
        "feugiat",
        "nulla",
        "facilisis",
        "at",
        "vero",
        "eos",
        "et",
        "accusam",
        "et",
        "justo",
        "duo",
        "dolores",
        "et",
        "ea",
        "rebum",
        "stet",
        "clita",
        "kasd",
        "gubergren",
        "no",
        "sea",
        "takimata",
        "sanctus",
        "est",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consetetur",
        "sadipscing",
        "elitr",
        "sed",
        "diam",
        "nonumy",
        "eirmod",
        "tempor",
        "invidunt",
        "ut",
        "labore",
        "et",
        "dolore",
        "magna",
        "aliquyam",
        "erat",
        "sed",
        "diam",
        "voluptua",
        "at",
        "vero",
        "eos",
        "et",
        "accusam",
        "et",
        "justo",
        "duo",
        "dolores",
        "et",
        "ea",
        "rebum",
        "stet",
        "clita",
        "kasd",
        "gubergren",
        "no",
        "sea",
        "takimata",
        "sanctus",
        "est",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consetetur",
        "sadipscing",
        "elitr",
        "at",
        "accusam",
        "aliquyam",
        "diam",
        "diam",
        "dolore",
        "dolores",
        "duo",
        "eirmod",
        "eos",
        "erat",
        "et",
        "nonumy",
        "sed",
        "tempor",
        "et",
        "et",
        "invidunt",
        "justo",
        "labore",
        "stet",
        "clita",
        "ea",
        "et",
        "gubergren",
        "kasd",
        "magna",
        "no",
        "rebum",
        "sanctus",
        "sea",
        "sed",
        "takimata",
        "ut",
        "vero",
        "voluptua",
        "est",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "lorem",
        "ipsum",
        "dolor",
        "sit",
        "amet",
        "consetetur",
        "sadipscing",
        "elitr",
        "sed",
        "diam",
        "nonumy",
        "eirmod",
        "tempor",
        "invidunt",
        "ut",
        "labore",
        "et",
        "dolore",
        "magna",
        "aliquyam",
        "erat",
        "consetetur",
        "sadipscing",
        "elitr",
        "sed",
        "diam",
        "nonumy",
        "eirmod",
        "tempor",
        "invidunt",
        "ut",
        "labore",
        "et",
        "dolore",
        "magna",
        "aliquyam",
        "erat",
        "sed",
        "diam",
        "voluptua",
        "at",
        "vero",
        "eos",
        "et",
        "accusam",
        "et",
        "justo",
        "duo",
        "dolores",
        "et",
        "ea",
        "rebum",
        "stet",
        "clita",
        "kasd",
        "gubergren",
        "no",
        "sea",
        "takimata",
        "sanctus",
        "est",
        "lorem",
        "ipsum"
      };
      StringBuilder stringBuilder = new StringBuilder();
      Random random = new Random();
      int num1 = 0;
      if (isHtml)
        stringBuilder.Append("<p>");
      stringBuilder.Append("lorem");
      if (words > 1)
      {
        int num2 = words <= paragraphs ? words : words / paragraphs;
        for (int index = 0; index <= words; ++index)
        {
          stringBuilder.Append(" " + strArray[random.Next(strArray.Length - 1)]);
          if (num1 > num2)
          {
            num1 = 0;
            if (isHtml)
              stringBuilder.Append("</p><p>");
            else
              stringBuilder.Append(Environment.NewLine).Append(Environment.NewLine).Append(Environment.NewLine);
          }
          ++num1;
        }
      }
      if (isHtml)
        stringBuilder.Append("</p>");
      stringBuilder.Append(".");
      return stringBuilder.ToString();
    }

    public static string GetMonthAsString(int month, bool fullName = true)
    {
      switch (month)
      {
        case 1:
          return fullName ? "January" : "Jan";
        case 2:
          return fullName ? "Febuary" : "Feb";
        case 3:
          return fullName ? "March" : "Mar";
        case 4:
          return fullName ? "April" : "Apr";
        case 5:
          return "May";
        case 6:
          return fullName ? "June" : "Jun";
        case 7:
          return fullName ? "July" : "Jul";
        case 8:
          return fullName ? "August" : "Aug";
        case 9:
          return fullName ? "September" : "Sep";
        case 10:
          return fullName ? "October" : "Oct";
        case 11:
          return fullName ? "November" : "Nov";
        case 12:
          return fullName ? "December" : "Dec";
        default:
          return "";
      }
    }

    public static int GetNumeric(string value)
    {
      int result;
      if (string.IsNullOrEmpty(value) || !int.TryParse(value, out result))
        return 0;
      return result;
    }

    public static int GetStarSignFromDate(DateTime value)
    {
      if (value.Month == 3)
        return value.Day < 21 ? 12 : 1;
      if (value.Month == 4)
        return value.Day < 20 ? 1 : 2;
      if (value.Month == 5)
        return value.Day < 21 ? 2 : 3;
      if (value.Month == 6)
        return value.Day < 21 ? 3 : 4;
      if (value.Month == 7)
        return value.Day < 23 ? 4 : 5;
      if (value.Month == 8)
        return value.Day < 23 ? 5 : 6;
      if (value.Month == 9)
        return value.Day < 23 ? 6 : 7;
      if (value.Month == 10)
        return value.Day < 23 ? 7 : 8;
      if (value.Month == 11)
        return value.Day < 22 ? 8 : 9;
      if (value.Month == 12)
        return value.Day < 22 ? 9 : 10;
      if (value.Month == 1)
        return value.Day < 20 ? 10 : 11;
      if (value.Month != 2)
        return 0;
      return value.Day < 19 ? 11 : 12;
    }

    public static bool HasNumbers(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      return Regex.IsMatch(value, "\\d+");
    }

    public static bool HasVowels(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if (!Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "a", true)) || !Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "e", true)) || (!Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "i", true)) || !Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "o", true))) || !Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "u", true)))
          return true;
      }
      return false;
    }

    public static int[] IndexesOf(string value, string valueToMatch)
    {
      if (value == null || valueToMatch == null || (value.Length == 0 || valueToMatch.Length == 0))
        return (int[]) null;
      List<int> intList = new List<int>();
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if (value.Substring(startIndex, 1) == valueToMatch)
          intList.Add(startIndex);
      }
      int[] array = new int[intList.Count];
      intList.CopyTo(array);
      intList.Clear();
      return array;
    }

    public static string InsertSeparatorAt(string value, string separator, int index)
    {
      if (value == null || value.Length == 0 || (separator == null || separator.Length == 0) || (index > value.Length - 1 || index < 0))
        return string.Empty;
      return value.Substring(0, index) + separator + value.Substring(index);
    }

    public static string InsertSeparatorEvery(string value, string separator, int spacing)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (spacing < 1)
        spacing = 1;
      if (value == null || value.Length == 0 || (separator == null || separator.Length == 0) || spacing - 1 > value.Length)
        return string.Empty;
      int startIndex = 0;
      while (startIndex <= value.Length - 1)
      {
        if (startIndex + spacing <= value.Length)
          stringBuilder.Append(value.Substring(startIndex, spacing)).Append(separator);
        else
          stringBuilder.Append(value.Substring(startIndex)).Append(separator);
        startIndex += spacing;
      }
      return stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
    }

    public static bool IsAlphaNumeric(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if (!SFStrings.IsNumeric(value.Substring(startIndex, 1)) && ((int) Convert.ToChar(value.Substring(startIndex, 1)) < 65 || (int) Convert.ToChar(value.Substring(startIndex, 1)) > 90) && ((int) Convert.ToChar(value.Substring(startIndex, 1)) < 97 || (int) Convert.ToChar(value.Substring(startIndex, 1)) > 122))
          return false;
      }
      return true;
    }

    public static bool IsCapitalized(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      return (int) Convert.ToChar(value.Substring(0, 1)) == (int) Convert.ToChar(value.Substring(0, 1).ToUpper());
    }

    public static bool IsDate(object obj)
    {
      string s = obj.ToString();
      try
      {
        DateTime dateTime = DateTime.Parse(s);
        return dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue;
      }
      catch
      {
        return false;
      }
    }

    public static bool IsEachWordCapital(string value, char separator = ' ')
    {
      if (value == null || value.Length == 0)
        return false;
      if (value.IndexOf(separator) > -1)
      {
        string[] strArray = value.Split(separator);
        for (int index = 0; index <= strArray.Length - 1; ++index)
        {
          if ((int) Convert.ToChar(strArray[index].Substring(0, 1)) != (int) Convert.ToChar(strArray[index].Substring(0, 1).ToUpper()))
            return false;
        }
      }
      return true;
    }

    public static bool IsLettersOnly(string value)
    {
      return SFStrings.IsAlphaNumeric(value) && SFStrings.IsNumeric(value);
    }

    public static bool IsLower(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if ((int) Convert.ToChar(value.Substring(startIndex, 1)) != (int) Convert.ToChar(value.Substring(startIndex, 1).ToLower()))
          return false;
      }
      return true;
    }

    public static bool IsMixedCase(string value)
    {
      bool flag = false;
      if (value == null || value.Length == 0)
        return false;
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if (SFStrings.IsLettersOnly(value.Substring(startIndex, 1)))
          flag = SFStrings.IsUpper(value.Substring(startIndex, 1));
      }
      if (value.Length == 1)
        return false;
      for (int startIndex = 1; startIndex <= value.Length - 1; ++startIndex)
      {
        if (SFStrings.IsUpper(value.Substring(startIndex, 1)) != flag && SFStrings.IsLettersOnly(value.Substring(startIndex, 1)))
          return true;
      }
      return false;
    }

    public static bool IsNumeric(string value)
    {
      double result;
      return !string.IsNullOrEmpty(value) && double.TryParse(value, out result);
    }

    public static bool IsNumericOnly(string value)
    {
      int result;
      return !string.IsNullOrEmpty(value) && int.TryParse(value, out result);
    }

    public static bool IsNumericWithDecimal(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      if (value.Length > 2)
      {
        value = value.ToLower().Replace("am", string.Empty);
        value = value.ToLower().Replace("pm", string.Empty);
      }
      return SFStrings.IsNumeric(value) && value.IndexOf(".") > -1;
    }

    public static bool IsSpacesOnly(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if ((int) Convert.ToChar(value.Substring(startIndex, 1)) != 32)
          return false;
      }
      return true;
    }

    public static bool IsUpper(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if ((int) Convert.ToChar(value.Substring(startIndex, 1)) != (int) Convert.ToChar(value.Substring(startIndex, 1).ToUpper()))
          return false;
      }
      return true;
    }

    public static bool IsValidPostcode(string value)
    {
      if (string.IsNullOrEmpty(value))
        return false;
      return Regex.IsMatch(value, "(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX]][0-9][A-HJKPSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY])))) [0-9][A-Z-[CIKMOV]]{2})");
    }

    public static bool IsValidUrl(string value)
    {
      if (string.IsNullOrEmpty(value))
        return false;
      return Regex.IsMatch(value, "(http|https)://([\\w-]+\\.)+(/[\\w- ./?%&=]*)?");
    }

    public static string PadString(string value, int charLength)
    {
      if (value == null)
        value = "";
      if (value.Length < charLength)
      {
        for (int index = 0; index < charLength; ++index)
          value += " ";
      }
      return value;
    }

    public static int PasswordStrength(string value)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (value == null || value.Length == 0)
        return 0;
      int num = value.Length * 3;
      for (int startIndex = 1; startIndex <= value.Length - 1; ++startIndex)
      {
        if ((int) Convert.ToChar(value.Substring(startIndex, 1)) >= 65 && (int) Convert.ToChar(value.Substring(startIndex, 1)) <= 92)
          flag1 = true;
      }
      for (int startIndex = 1; startIndex <= value.Length - 1; ++startIndex)
      {
        if ((int) Convert.ToChar(value.Substring(startIndex, 1)) >= 97 & (int) Convert.ToChar(value.Substring(startIndex, 1)) <= 122)
          flag2 = true;
      }
      if (flag1 & flag2)
        num = Convert.ToInt32((double) num * 1.2);
      for (int startIndex = 1; startIndex <= value.Length - 1; ++startIndex)
      {
        if ((int) Convert.ToChar(value.Substring(startIndex, 1)) >= 48 & (int) Convert.ToChar(value.Substring(startIndex, 1)) <= 57)
        {
          if (flag1 | flag2)
          {
            num = Convert.ToInt32((double) num * 1.4);
            break;
          }
          break;
        }
      }
      for (int startIndex = 1; startIndex <= value.Length - 1; ++startIndex)
      {
        if ((int) Convert.ToChar(value.Substring(startIndex, 1)) <= 47 | (int) Convert.ToChar(value.Substring(startIndex, 1)) >= 123 | (int) Convert.ToChar(value.Substring(startIndex, 1)) >= 58 & (int) Convert.ToChar(value.Substring(startIndex, 1)) <= 64)
        {
          num = Convert.ToInt32((double) num * 1.5);
          break;
        }
      }
      if (num > 100)
        num = 100;
      return num;
    }

    public static string RemoveVowels(string value)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (value == null || value.Length == 0)
        return string.Empty;
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if (Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "a", true)) && Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "e", true)) && (Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "i", true)) && Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "o", true))) && Convert.ToBoolean(string.Compare(value.Substring(startIndex, 1), "u", true)))
          stringBuilder.Append(value.Substring(startIndex, 1));
      }
      return stringBuilder.ToString();
    }

    public static string Reverse(string value)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (value == null || value.Length == 0)
        return string.Empty;
      int startIndex = value.Length - 1;
      while (startIndex >= 0)
      {
        stringBuilder.Append(value.Substring(startIndex, 1));
        startIndex += -1;
      }
      return stringBuilder.ToString();
    }

    public static string[] SplitKeepingQuotes(string value, char separator = ' ')
    {
      string[] strArray = value.Split(separator);
      List<string> stringList1 = new List<string>();
      if (value == null || value.Length == 0)
        return (string[]) null;
      for (int index1 = 0; index1 <= strArray.Length - 1; ++index1)
      {
        if (strArray[index1].StartsWith("\""))
        {
          List<string> stringList2 = new List<string>();
          for (int index2 = index1; index2 <= strArray.Length - 1; ++index2)
          {
            if (strArray[index2].EndsWith("\""))
            {
              stringList2.Add(strArray[index2].Substring(0, strArray[index2].Length - 1));
              index1 = index2;
              break;
            }
            if (strArray[index2].StartsWith("\""))
              strArray[index2] = strArray[index2].Substring(1);
            stringList2.Add(strArray[index2]);
          }
          string[] array = new string[stringList2.Count];
          stringList2.CopyTo(array);
          if (stringList2.Count != 0)
            stringList1.Add(string.Join(separator.ToString(), array));
          stringList2.Clear();
        }
        else
          stringList1.Add(strArray[index1]);
      }
      string[] array1 = new string[stringList1.Count];
      stringList1.CopyTo(array1);
      stringList1.Clear();
      if (array1 == null)
        array1 = new string[1];
      return array1;
    }

    public static string[] SplitLines(string value)
    {
      char[] separator = new char[2]{ '\r', '\n' };
      return value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] SplitString(string source, string value)
    {
      return source.Split(new string[1]{ value }, StringSplitOptions.None);
    }

    public static string[] SplitWords(string value)
    {
      return Regex.Split(value, "\\W+");
    }

    public static string InsertSeparator(string value, string separator)
    {
      if (value == null || value.Length == 0 || (separator == null || separator.Length == 0))
        return string.Empty;
      return SFStrings.InsertSeparatorEvery(value, separator, 1);
    }

    public static bool IsEmailAddress(string value)
    {
      if (value == null || value.Length == 0)
        return false;
      string pattern = "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
      return Regex.Match(value, pattern).Success;
    }

    public static string StripHtmlTags(string value)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      value = Regex.Replace(value, "<(.|\\n)*?>", string.Empty);
      return value;
    }

    public static string StripNonAlphaChars(string value)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      char[] charArray = value.ToCharArray();
      for (int index = 0; index <= charArray.Length - 1; ++index)
      {
        if ((int) charArray[index] > 96 && (int) charArray[index] < 122)
          stringBuilder.Append(charArray[index]);
      }
      return stringBuilder.ToString();
    }

    public static string StripNonAlphaNumericChars(string value)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      char[] charArray = value.ToCharArray();
      for (int startIndex = 0; startIndex <= charArray.Length - 1; ++startIndex)
      {
        if (SFStrings.IsNumeric(value.Substring(startIndex, 1)))
          stringBuilder.Append(charArray[startIndex]);
        else if ((int) Convert.ToChar(value.Substring(startIndex, 1)) >= 65 && (int) Convert.ToChar(value.Substring(startIndex, 1)) <= 90 || (int) Convert.ToChar(value.Substring(startIndex, 1)) >= 97 && (int) Convert.ToChar(value.Substring(startIndex, 1)) <= 122)
          stringBuilder.Append(charArray[startIndex]);
      }
      return stringBuilder.ToString();
    }

    public static string StripNonControlChars(string value)
    {
      if (value == null || value == "")
        return "";
      StringBuilder stringBuilder = new StringBuilder();
      char[] charArray = value.ToCharArray();
      for (int index = 0; index < charArray.Length; ++index)
      {
        if (!char.IsControl(charArray[index]))
          stringBuilder.Append(charArray[index]);
      }
      return stringBuilder.ToString();
    }

    public static string SubStringFromTo(string value, int startIndex, int endIndex)
    {
      if (value == null || value.Length == 0 || (startIndex < 0 || endIndex > value.Length - 1) || startIndex > endIndex)
        return value;
      return value.Substring(startIndex, endIndex - startIndex);
    }

    public static string SwapCase(string value)
    {
      if (value == null || value.Length == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      for (int startIndex = 0; startIndex <= value.Length - 1; ++startIndex)
      {
        if (SFStrings.IsUpper(value.Substring(startIndex, 1)))
          stringBuilder.Append(value.Substring(startIndex, 1).ToLower());
        else
          stringBuilder.Append(value.Substring(startIndex, 1).ToUpper());
      }
      return stringBuilder.ToString();
    }

    public static byte[] ToBytes(string str)
    {
      byte[] numArray = new byte[str.Length * 2];
      Buffer.BlockCopy((Array) str.ToCharArray(), 0, (Array) numArray, 0, numArray.Length);
      return numArray;
    }

    public static DateTime ToDate(string value)
    {
      DateTime defaultValue = new DateTime(1900, 1, 1);
      return SFStrings.ToDate(value, defaultValue);
    }

    public static DateTime ToDate(string value, DateTime defaultValue)
    {
      return SFStrings.ToDate(value, defaultValue, false);
    }

    public static DateTime ToDate(string value, DateTime defaultValue, bool isUs)
    {
      int day = 0;
      int year = 0;
      int minute = 0;
      int hour = 0;
      int second = 0;
      bool flag = false;
      if (string.IsNullOrEmpty(value))
        return defaultValue;
      value = value.ToLower();
      string[] strArray1;
      if (value.Contains("t"))
      {
        string[] strArray2 = value.Split('t');
        strArray1 = strArray2[0].Split('-');
        flag = true;
        if (strArray2.Length == 2)
        {
          string[] strArray3 = strArray2[1].Split(':');
          if (strArray3.Length == 3)
          {
            hour = Convert.ToInt32(strArray3[0]);
            minute = Convert.ToInt32(strArray3[1]);
            second = Convert.ToInt32(Convert.ToDecimal(strArray3[2]));
          }
        }
      }
      else if (value.Contains(":"))
      {
        value = value.Replace("pm", "");
        value = value.Replace("am", "");
        value = value.Trim();
        string[] strArray2 = value.Split(' ');
        if (value.Contains("/"))
          strArray1 = strArray2[0].Split('/');
        else
          strArray1 = strArray2[0].Split('-');
        if (strArray2.Length == 2)
        {
          string[] strArray3 = strArray2[1].Split(':');
          if (strArray3.Length == 3)
          {
            hour = Convert.ToInt32(strArray3[0]);
            minute = Convert.ToInt32(strArray3[1]);
            second = Convert.ToInt32(Convert.ToDecimal(strArray3[2]));
          }
        }
      }
      else if (value.Contains("/"))
        strArray1 = value.Split('/');
      else if (value.Contains("-"))
        strArray1 = value.Split('-');
      else
        strArray1 = value.Split(' ');
      if (strArray1.Length < 3 || strArray1.Length > 4)
        return defaultValue;
      int num = strArray1.Length != 4 ? 0 : 1;
      if (strArray1[0].Length == 4 && strArray1[1].Length == 2 && strArray1[2].Length == 2)
        flag = true;
      else if (strArray1[0].Length == 2 && strArray1[1].Length == 2 && strArray1[2].Length == 4)
        flag = false;
      int month;
      if (flag)
      {
        if (SFStrings.IsNumeric(strArray1[2 + num]))
          day = Convert.ToInt32(strArray1[2 + num]);
        month = !SFStrings.IsNumeric(strArray1[1 + num]) ? SFStrings.GetMonthFromString(strArray1[1 + num]) : Convert.ToInt32(strArray1[1 + num]);
        if (SFStrings.IsNumeric(strArray1[0 + num]))
          year = Convert.ToInt32(strArray1[0 + num]);
      }
      else if (isUs)
      {
        if (SFStrings.IsNumeric(strArray1[1 + num]))
          day = Convert.ToInt32(strArray1[1 + num]);
        month = !SFStrings.IsNumeric(strArray1[0 + num]) ? SFStrings.GetMonthFromString(strArray1[1 + num]) : Convert.ToInt32(strArray1[0 + num]);
        if (SFStrings.IsNumeric(strArray1[2 + num]))
          year = Convert.ToInt32(strArray1[2 + num]);
      }
      else
      {
        if (SFStrings.IsNumeric(strArray1[0 + num]))
          day = Convert.ToInt32(strArray1[0 + num]);
        month = !SFStrings.IsNumeric(strArray1[1 + num]) ? SFStrings.GetMonthFromString(strArray1[1 + num]) : Convert.ToInt32(strArray1[1 + num]);
        if (SFStrings.IsNumeric(strArray1[2 + num]))
          year = Convert.ToInt32(strArray1[2 + num]);
      }
      if (day > 0 && month > 0)
      {
        if (year > 0)
        {
          try
          {
            return new DateTime(year, month, day, hour, minute, second);
          }
          catch
          {
            try
            {
              return Convert.ToDateTime(value);
            }
            catch
            {
            }
          }
        }
      }
      return SFStrings.ToDateVerbose(value, defaultValue);
    }

    public static DateTime ToDateVerbose(string value, string separator = "/", SFStrings.DateFormat format = SFStrings.DateFormat.UK)
    {
      return SFStrings.ToDateVerbose(value, new DateTime(1900, 1, 1), separator, format);
    }

    public static DateTime ToDateVerbose(string value, DateTime defaultValue, string separator = "/", SFStrings.DateFormat format = SFStrings.DateFormat.UK)
    {
      string[] strArray1 = new string[14]
      {
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sun",
        "Mon",
        "Tue",
        "Wed",
        "Thu",
        "Fri",
        "Sat"
      };
      int day = 0;
      int year = 0;
      int hour = 0;
      int minute = 0;
      int second = 0;
      if (string.IsNullOrEmpty(value))
        return defaultValue;
      string[] strArray2;
      if (value.Contains("/"))
        strArray2 = value.Split('/');
      else if (value.Contains("-"))
        strArray2 = value.Split('-');
      else if (value.Contains("|"))
        strArray2 = value.Split('|');
      else if (value.Contains(separator))
        strArray2 = value.Split(separator.ToCharArray());
      else
        strArray2 = value.Split(' ');
      if (strArray2.Length < 3)
        return defaultValue;
      if (strArray2.Length > 3)
      {
        string[] strArray3 = strArray2[4].Trim().Split(':');
        if (strArray3.Length == 3)
        {
          try
          {
            hour = Convert.ToInt32(strArray3[0]);
            minute = Convert.ToInt32(strArray3[1]);
            second = Convert.ToInt32(strArray3[2]);
          }
          catch
          {
          }
        }
      }
      int month;
      if (format == SFStrings.DateFormat.UK)
      {
        int num = strArray2.Length != 4 ? 0 : 1;
        if (SFStrings.IsNumeric(strArray2[0 + num]))
          day = Convert.ToInt32(strArray2[0 + num]);
        month = !SFStrings.IsNumeric(strArray2[1 + num]) ? SFStrings.GetMonthFromString(strArray2[1 + num]) : Convert.ToInt32(strArray2[1 + num]);
        if (SFStrings.IsNumeric(strArray2[2 + num]))
          year = Convert.ToInt32(strArray2[2 + num]);
      }
      else if (format == SFStrings.DateFormat.US)
      {
        if (SFStrings.IsNumeric(strArray2[1]))
          day = Convert.ToInt32(strArray2[1]);
        month = !SFStrings.IsNumeric(strArray2[0]) ? SFStrings.GetMonthFromString(strArray2[0]) : Convert.ToInt32(strArray2[0]);
        if (SFStrings.IsNumeric(strArray2[2]))
          year = Convert.ToInt32(strArray2[2]);
      }
      else
      {
        if (SFStrings.IsNumeric(strArray2[2]))
          day = Convert.ToInt32(strArray2[2]);
        month = !SFStrings.IsNumeric(strArray2[1]) ? SFStrings.GetMonthFromString(strArray2[1]) : Convert.ToInt32(strArray2[1]);
        if (SFStrings.IsNumeric(strArray2[0]))
          year = Convert.ToInt32(strArray2[0]);
      }
      if (day > 0 && month > 0)
      {
        if (year > 0)
        {
          try
          {
            return new DateTime(year, month, day, hour, minute, second);
          }
          catch
          {
          }
        }
      }
      return defaultValue;
    }

    public static DateTime ToDateVerbose(string value, DateTime defaultValue)
    {
      string[] strArray1 = new string[14]
      {
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sun",
        "Mon",
        "Tue",
        "Wed",
        "Thu",
        "Fri",
        "Sat"
      };
      int day = 0;
      int year = 0;
      if (string.IsNullOrEmpty(value))
        return defaultValue;
      string[] strArray2;
      if (value.Contains("/"))
        strArray2 = value.Split('/');
      else
        strArray2 = value.Split(' ');
      if (strArray2.Length < 3 || strArray2.Length > 4)
        return defaultValue;
      int num = strArray2.Length != 4 ? 0 : 1;
      if (SFStrings.IsNumeric(strArray2[0 + num]))
        day = Convert.ToInt32(strArray2[0 + num]);
      int month = !SFStrings.IsNumeric(strArray2[1 + num]) ? SFStrings.GetMonthFromString(strArray2[1 + num]) : Convert.ToInt32(strArray2[1 + num]);
      if (SFStrings.IsNumeric(strArray2[2 + num]))
        year = Convert.ToInt32(strArray2[2 + num]);
      if (day > 0 && month > 0)
      {
        if (year > 0)
        {
          try
          {
            return new DateTime(year, month, day);
          }
          catch
          {
          }
        }
      }
      return defaultValue;
    }

    public static string ToSFCString(DateTime value)
    {
      return value.Year.ToString() + ":" + (object) value.Month + ":" + (object) value.Day + ":" + (object) value.Hour + ":" + (object) value.Minute + ":" + (object) value.Second;
    }

    public static int TotalCharsInString(string value, string charsToFind)
    {
      int num = 0;
      if (value == null || charsToFind == null || (value.Length == 0 || charsToFind.Length == 0))
        return 0;
      int startIndex = 0;
      while (startIndex <= value.Length - 1)
      {
        if (startIndex + charsToFind.Length <= value.Length && !Convert.ToBoolean(string.Compare(value.Substring(startIndex, charsToFind.Length), charsToFind, true)))
          ++num;
        startIndex += charsToFind.Length;
      }
      return num;
    }

    public string GetStarSignFromDate(DateTime sourceValue, ref int starSignID)
    {
      string[] strArray = new string[12]
      {
        "Aries",
        "Taurus",
        "Gemini",
        "Cancer",
        "Leo",
        "Virgo",
        "Libra",
        "Scorpio",
        "Sagittarius",
        "Capricorn",
        "Aquarius",
        "Pisces"
      };
      starSignID = this.GetStarSignIDFromDate(sourceValue);
      int index = starSignID - 1;
      return strArray[index];
    }

    public int GetStarSignIDFromDate(DateTime sourceValue)
    {
      int num = 0;
      switch (sourceValue.Month)
      {
        case 1:
          num = sourceValue.Day <= 20 ? 10 : 11;
          break;
        case 2:
          num = sourceValue.Day <= 19 ? 11 : 12;
          break;
        case 3:
          num = sourceValue.Day <= 20 ? 12 : 1;
          break;
        case 4:
          num = sourceValue.Day <= 20 ? 1 : 2;
          break;
        case 5:
          num = sourceValue.Day <= 21 ? 2 : 3;
          break;
        case 6:
          num = sourceValue.Day <= 21 ? 3 : 4;
          break;
        case 7:
          num = sourceValue.Day <= 23 ? 4 : 5;
          break;
        case 8:
          num = sourceValue.Day <= 23 ? 5 : 6;
          break;
        case 9:
          num = sourceValue.Day <= 22 ? 6 : 7;
          break;
        case 10:
          num = sourceValue.Day <= 23 ? 7 : 8;
          break;
        case 11:
          num = sourceValue.Day <= 22 ? 8 : 9;
          break;
        case 12:
          num = sourceValue.Day <= 21 ? 9 : 10;
          break;
      }
      return num;
    }

    public bool ValidateString(string value)
    {
      return new Regex("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$").IsMatch(value);
    }

    private static string GetMonthFromInteger(int value, bool UseShortMonth = false)
    {
      string[] strArray = new string[24]
      {
        "January",
        "Febuary",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December",
        "Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec"
      };
      if (value < 1 | value > 12)
        return "";
      if (UseShortMonth)
        value += 11;
      else
        --value;
      try
      {
        return strArray[value];
      }
      catch
      {
        return "";
      }
    }

    private static int GetMonthFromString(string value)
    {
      string[] strArray = new string[24]
      {
        "january",
        "febuary",
        "march",
        "april",
        "may",
        "june",
        "july",
        "august",
        "september",
        "october",
        "november",
        "december",
        "jan",
        "feb",
        "mar",
        "apr",
        "may",
        "jun",
        "jul",
        "aug",
        "sep",
        "oct",
        "nov",
        "dec"
      };
      if (string.IsNullOrEmpty(value))
        return 0;
      value = SFStrings.StripNonAlphaChars(value.ToLower());
      for (int index = 0; index <= strArray.Length - 1; ++index)
      {
        if (strArray[index].ToLower() == value)
        {
          if (index > 11)
            return index - 11;
          return index + 1;
        }
      }
      return 0;
    }

    private static string ThreeNonZeroDigits(double value)
    {
      if (value >= 100.0)
        return Convert.ToInt32(value).ToString();
      if (value >= 10.0)
        return value.ToString("0.0");
      return value.ToString("0.00");
    }

    public enum DateFormat
    {
      UK,
      US,
      ISO,
    }
  }
}
