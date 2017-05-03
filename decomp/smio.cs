// Decompiled with JetBrains decompiler
// Type: Wyvern.Base.SFIO
// Assembly: Wyvern.Base, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C96879FF-8A02-4EC1-BB96-AE29A19B1BA7
// Assembly location: D:\WYVERN\AssembliesX\Wyvern.Base.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wyvern.Base
{
  public class SFIO
  {
    private static Dictionary<byte[], Func<BinaryReader, Size>> imageFormatDecoders = new Dictionary<byte[], Func<BinaryReader, Size>>()
    {
      {
        new byte[2]{ (byte) 66, (byte) 77 },
        new Func<BinaryReader, Size>(SFIO.DecodeBitmap)
      },
      {
        new byte[6]
        {
          (byte) 71,
          (byte) 73,
          (byte) 70,
          (byte) 56,
          (byte) 55,
          (byte) 97
        },
        new Func<BinaryReader, Size>(SFIO.DecodeGif)
      },
      {
        new byte[6]
        {
          (byte) 71,
          (byte) 73,
          (byte) 70,
          (byte) 56,
          (byte) 57,
          (byte) 97
        },
        new Func<BinaryReader, Size>(SFIO.DecodeGif)
      },
      {
        new byte[8]
        {
          (byte) 137,
          (byte) 80,
          (byte) 78,
          (byte) 71,
          (byte) 13,
          (byte) 10,
          (byte) 26,
          (byte) 10
        },
        new Func<BinaryReader, Size>(SFIO.DecodePng)
      },
      {
        new byte[2]{ byte.MaxValue, (byte) 216 },
        new Func<BinaryReader, Size>(SFIO.DecodeJfif)
      }
    };
    private const string errorMessage = "Could not recognize image format.";

    public static string ConvertTextEncoding(string source, Encoding encoding, Encoding convertToEncoding)
    {
      byte[] bytes1 = encoding.GetBytes(source);
      byte[] bytes2 = Encoding.Convert(encoding, convertToEncoding, bytes1);
      source = convertToEncoding.GetString(bytes2);
      return source;
    }

    public static string ConvertTextToUTF8(string source, Encoding encoding)
    {
      byte[] bytes = encoding.GetBytes(source);
      source = Encoding.UTF8.GetString(Encoding.Convert(encoding, Encoding.UTF8, bytes));
      return source;
    }

    public static List<string> CsvSplit(string source)
    {
      return SFIO.CsvSplit(source, true, true, ',', '"');
    }

    public static List<string> CsvSplit(string source, char separator, char quote)
    {
      return SFIO.CsvSplit(source, true, true, separator, quote);
    }

    public static List<string> CsvSplit(string source, bool stripQuotes, bool trim)
    {
      return SFIO.CsvSplit(source, stripQuotes, trim, ',', '"');
    }

    public static List<string> CsvSplit(string source, bool stripQuotes, bool trim, char separator, char quote)
    {
      string str1 = "\"\"";
      int startIndex = -1;
      int num = -1;
      if ((int) separator == 0)
        separator = ',';
      if ((int) quote == 0)
        quote = '"';
      else
        str1 = quote.ToString() + quote.ToString();
      List<string> stringList = new List<string>();
      if (string.IsNullOrEmpty(source))
        return stringList;
      source.Trim();
      if (source.Length < 3)
        return stringList;
      for (int length = 0; length < source.Length; ++length)
      {
        if ((int) source[length] == (int) separator && num == -1)
        {
          if (startIndex == -1)
          {
            stringList.Add(source.Substring(0, length));
            startIndex = length + 1;
          }
          else
          {
            if (stripQuotes)
            {
              string str2 = source.Substring(startIndex, length - startIndex);
              if (str2 == str1)
              {
                stringList.Add("");
              }
              else
              {
                if (str2.Length > 2 && (int) str2[0] == (int) quote)
                {
                  string str3 = str2;
                  int index = str3.Length - 1;
                  if ((int) str3[index] == (int) quote)
                    str2 = str2.Substring(1, str2.Length - 2);
                }
                if (trim)
                  str2 = str2.Trim();
                stringList.Add(str2);
              }
            }
            else if (trim)
              stringList.Add(source.Substring(startIndex, length - startIndex).Trim());
            else
              stringList.Add(source.Substring(startIndex, length - startIndex));
            startIndex = length + 1;
          }
        }
        if ((int) source[length] == (int) quote)
        {
          if (num == -1)
          {
            num = length;
            if (startIndex == -1)
              startIndex = length;
          }
          else
            num = -1;
        }
      }
      string str4 = source.Substring(startIndex, source.Length - startIndex);
      if (stripQuotes)
      {
        if (str4 == str1)
        {
          stringList.Add("");
        }
        else
        {
          if (str4.Length > 2 && (int) str4[0] == (int) quote)
          {
            string str2 = str4;
            int index = str2.Length - 1;
            if ((int) str2[index] == (int) quote)
              str4 = str4.Substring(1, str4.Length - 2);
          }
          if (trim)
            str4 = str4.Trim();
          stringList.Add(str4);
        }
      }
      else if (trim)
        stringList.Add(str4.Trim());
      else
        stringList.Add(str4);
      return stringList;
    }

    public static void DeleteFolder(string folder, bool includeSubFolders)
    {
      foreach (SFFile sfFile in SFIO.GetSFFiles(folder, includeSubFolders))
      {
        sfFile.FileInfo.IsReadOnly = false;
        sfFile.FileInfo.Delete();
      }
      Directory.Delete(folder, includeSubFolders);
    }

    public static FileLine FileLineParse(string fileName, int lineNumber)
    {
      return SFIO.FileLineParse(fileName, lineNumber, true, false, ',', '"', false);
    }

    public static FileLine FileLineParse(string fileName, int lineNumber, bool stripQuotes, bool trim, char separator, char quote, bool csvSplitOn)
    {
      FileLine fileLine = new FileLine();
      if (fileName == "" || !File.Exists(fileName))
        return fileLine;
      int num = File.ReadLines(fileName).Count<string>();
      if (num == 0 || lineNumber > num || lineNumber < 1)
        return fileLine;
      string source = File.ReadLines(fileName).Skip<string>(lineNumber - 1).Take<string>(1).First<string>();
      List<string> stringList = new List<string>();
      if (csvSplitOn)
        stringList = SFIO.CsvSplit(source, stripQuotes, trim, separator, quote);
      return new FileLine()
      {
        Data = source,
        Index = lineNumber,
        ColumnData = stringList
      };
    }

    public static FilePaging FilePagingParse(FilePaging filePaging)
    {
      List<FileLine> fileLineList = new List<FileLine>();
      filePaging.Message = "";
      if (filePaging.FileName == "" || !File.Exists(filePaging.FileName))
        filePaging.Message = "Error: Cannot open file";
      if (filePaging.PageSize < 1)
        filePaging.PageSize = 1;
      if (filePaging.PageIndex < 1)
        filePaging.PageIndex = 1;
      filePaging.Lines = new List<FileLine>();
      filePaging.PagingTotal = File.ReadLines(filePaging.FileName).Count<string>();
      if (filePaging.PagingTotal > 0)
      {
        int count1 = filePaging.PageSize;
        int count2;
        if (filePaging.PagingTotal <= filePaging.PageSize)
        {
          count2 = 1;
          filePaging.PagingPages = 1;
        }
        else
        {
          count2 = (filePaging.PageIndex - 1) * filePaging.PageSize;
          if (count2 + count1 > filePaging.PagingTotal)
            count1 = filePaging.PagingTotal - count2;
          FilePaging filePaging1 = filePaging;
          int num = filePaging1.PagingTotal / filePaging.PageSize;
          filePaging1.PagingPages = num;
        }
        List<string> list = File.ReadLines(filePaging.FileName).Skip<string>(count2).Take<string>(count1).ToList<string>();
        for (int index = 0; index < list.Count; ++index)
        {
          List<string> stringList = new List<string>();
          if (filePaging.CsvSplitOn)
            stringList = SFIO.CsvSplit(list[index], filePaging.StripQuotes, filePaging.Trim, filePaging.Separator, filePaging.Quote);
          filePaging.Lines.Add(new FileLine()
          {
            Data = list[index],
            Index = count2 + (index + 1),
            ColumnData = stringList
          });
        }
      }
      else
        filePaging.PagingPages = 0;
      return filePaging;
    }

    public static List<FileInfo> FilterFiles(List<FileInfo> source, FileFilterOptions options)
    {
      return SFIO.FilterFiles(source, options, (FileSortOptions) null);
    }

    public static List<FileInfo> FilterFiles(List<FileInfo> source, FileFilterOptions options, FileSortOptions sortOptions)
    {
      List<FileInfo> listOfFiles = new List<FileInfo>();
      List<FileInfo> fileInfoList = new List<FileInfo>();
      GenericComparer<FileInfo> genericComparer = new GenericComparer<FileInfo>();
      if (string.IsNullOrEmpty(options.FilterValue) || source == null || source.Count == 0)
        return source;
      if (options.SystemFilter == SFIO.FilterSystemType.Hidden)
        source = source.Select<FileInfo, FileInfo>((Func<FileInfo, FileInfo>) (f => f)).Where<FileInfo>((Func<FileInfo, bool>) (f => (f.Attributes & FileAttributes.Hidden) == (FileAttributes) 0)).ToList<FileInfo>();
      else if (options.SystemFilter == SFIO.FilterSystemType.System)
      {
        source = source.Select<FileInfo, FileInfo>((Func<FileInfo, FileInfo>) (f => f)).Where<FileInfo>((Func<FileInfo, bool>) (f => (f.Attributes & FileAttributes.System) == (FileAttributes) 0)).ToList<FileInfo>();
      }
      else
      {
        source = source.Select<FileInfo, FileInfo>((Func<FileInfo, FileInfo>) (f => f)).Where<FileInfo>((Func<FileInfo, bool>) (f => (f.Attributes & FileAttributes.System) == (FileAttributes) 0)).ToList<FileInfo>();
        source = source.Select<FileInfo, FileInfo>((Func<FileInfo, FileInfo>) (f => f)).Where<FileInfo>((Func<FileInfo, bool>) (f => (f.Attributes & FileAttributes.Hidden) == (FileAttributes) 0)).ToList<FileInfo>();
      }
      List<string> list = ((IEnumerable<string>) options.FilterValue.Split('|')).ToList<string>();
      if (options.MatchCase)
      {
        for (int index = 0; index <= list.Count - 1; ++index)
        {
          string value = list[index];
          List<FileInfo> source1 = (List<FileInfo>) null;
          switch (options.Filter)
          {
            case SFIO.FilterFilesType.Match:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName) == value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName == value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName) == value)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.Contains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.EndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).EndsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.StartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).StartsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotStartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileNameWithoutExtension(n.FullName).StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !n.FullName.StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileName(n.FullName).StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetExtension(n.FullName).StartsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotEndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileNameWithoutExtension(n.FullName).EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !n.FullName.EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileName(n.FullName).EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetExtension(n.FullName).EndsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotContains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotMatch:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName) != value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName != value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName) != value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName) != value)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.Pattern:
              string[] patternValues = value.Split('*');
              if (value.StartsWith("*"))
              {
                value = value.Replace("*", "");
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).EndsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.EndsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).EndsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).EndsWith(value))).ToList<FileInfo>();
                    break;
                }
              }
              else if (value.EndsWith("*"))
              {
                value = value.Replace("*", "");
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).StartsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.StartsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).StartsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).StartsWith(value))).ToList<FileInfo>();
                    break;
                }
              }
              if (patternValues.Length > 1)
              {
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (Path.GetFileNameWithoutExtension(n.FullName).StartsWith(patternValues[0]))
                        return Path.GetFileNameWithoutExtension(n.FullName).EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (n.FullName.ToLower().StartsWith(patternValues[0]))
                        return n.FullName.EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (Path.GetFileName(n.FullName).StartsWith(patternValues[0]))
                        return Path.GetFileName(n.FullName).EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (n.Extension.ToLower().StartsWith(patternValues[0]))
                        return n.Extension.EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                }
              }
              else
                break;
          }
          if (source1 != null && source1.Count > 0)
            listOfFiles.AddRange((IEnumerable<FileInfo>) source1.Where<FileInfo>((Func<FileInfo, bool>) (p => !listOfFiles.Any<FileInfo>((Func<FileInfo, bool>) (p2 => p2.FullName == p.FullName)))).ToList<FileInfo>());
        }
      }
      else
      {
        for (int index = 0; index <= list.Count - 1; ++index)
        {
          string value = list[index].ToLower();
          List<FileInfo> source1 = (List<FileInfo>) null;
          switch (options.Filter)
          {
            case SFIO.FilterFilesType.Match:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower() == value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower() == value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower() == value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower() == value)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.Contains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower().IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower().IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower().IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower().IndexOf(value) > -1)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.EndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.StartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotStartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileNameWithoutExtension(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !n.FullName.ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileName(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetExtension(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotEndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileNameWithoutExtension(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !n.FullName.ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetFileName(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => !Path.GetExtension(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotContains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower().IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower().IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower().IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower().IndexOf(value) == -1)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.NotMatch:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower() != value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower() != value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower() != value)).ToList<FileInfo>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower() != value)).ToList<FileInfo>();
                  break;
              }
            case SFIO.FilterFilesType.Pattern:
              string[] patternValues = value.Split('*');
              if (value.StartsWith("*"))
              {
                value = value.Replace("*", "");
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower().EndsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower().EndsWith(value))).ToList<FileInfo>();
                    break;
                }
              }
              else if (value.EndsWith("*"))
              {
                value = value.Replace("*", "");
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileNameWithoutExtension(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => n.FullName.ToLower().StartsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetFileName(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n => Path.GetExtension(n.FullName).ToLower().StartsWith(value))).ToList<FileInfo>();
                    break;
                }
              }
              if (patternValues.Length > 1)
              {
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (Path.GetFileNameWithoutExtension(n.FullName).ToLower().StartsWith(patternValues[0]))
                        return Path.GetFileNameWithoutExtension(n.FullName).ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (n.FullName.ToLower().StartsWith(patternValues[0]))
                        return n.FullName.ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (Path.GetFileName(n.FullName).ToLower().StartsWith(patternValues[0]))
                        return Path.GetFileName(n.FullName).ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<FileInfo>((Func<FileInfo, bool>) (n =>
                    {
                      if (n.Extension.ToLower().StartsWith(patternValues[0]))
                        return n.Extension.ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<FileInfo>();
                    break;
                }
              }
              else
                break;
          }
          if (source1 != null && source1.Count > 0)
            listOfFiles.AddRange((IEnumerable<FileInfo>) source1.Where<FileInfo>((Func<FileInfo, bool>) (p => !listOfFiles.Any<FileInfo>((Func<FileInfo, bool>) (p2 => p2.FullName == p.FullName)))).ToList<FileInfo>());
        }
      }
      if (options.SystemFilter == SFIO.FilterSystemType.Hidden)
        listOfFiles = source.Select<FileInfo, FileInfo>((Func<FileInfo, FileInfo>) (f => f)).Where<FileInfo>((Func<FileInfo, bool>) (f => (f.Attributes & FileAttributes.Hidden) == (FileAttributes) 0)).ToList<FileInfo>();
      else if (options.SystemFilter == SFIO.FilterSystemType.System)
        listOfFiles = source.Select<FileInfo, FileInfo>((Func<FileInfo, FileInfo>) (f => f)).Where<FileInfo>((Func<FileInfo, bool>) (f => (f.Attributes & FileAttributes.System) == (FileAttributes) 0)).ToList<FileInfo>();
      if (listOfFiles.Count > 0 && sortOptions != null && sortOptions.Sort != SFIO.SortType.None)
      {
        if (string.IsNullOrEmpty(sortOptions.PropertyName))
          sortOptions.PropertyName = "Name";
        if (sortOptions.Sort == SFIO.SortType.Ascending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Ascending));
        }
        else if (sortOptions.Sort == SFIO.SortType.Descending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Descending));
        }
        listOfFiles.Sort((IComparer<FileInfo>) genericComparer);
      }
      return listOfFiles;
    }

    public static List<SFFile> FilterFiles(List<string> source, FileFilterOptions options)
    {
      if (source == null || source.Count <= 0)
        return new List<SFFile>();
      List<SFFile> source1 = new List<SFFile>();
      for (int index = 0; index < source.Count; ++index)
        source1.Add(new SFFile(source[index]));
      return SFIO.FilterFiles(source1, options);
    }

    public static List<SFFile> FilterFiles(List<SFFile> source, FileFilterOptions options)
    {
      return SFIO.FilterFiles(source, options, (FileSortOptions) null);
    }

    public static List<SFFile> FilterFiles(List<SFFile> source, FileFilterOptions options, FileSortOptions sortOptions)
    {
      List<SFFile> listOfFiles = new List<SFFile>();
      List<SFFile> sfFileList = new List<SFFile>();
      GenericComparer<SFFile> genericComparer = new GenericComparer<SFFile>();
      if (source == null)
        return new List<SFFile>();
      if (options == null || source.Count == 0 || string.IsNullOrEmpty(options.FilterValue))
        return source;
      if (source[0].FileInfo != null)
      {
        if (options.SystemFilter == SFIO.FilterSystemType.Hidden)
          source = source.Select<SFFile, SFFile>((Func<SFFile, SFFile>) (f => f)).Where<SFFile>((Func<SFFile, bool>) (f => (f.FileInfo.Attributes & FileAttributes.Hidden) == (FileAttributes) 0)).ToList<SFFile>();
        else if (options.SystemFilter == SFIO.FilterSystemType.System)
        {
          source = source.Select<SFFile, SFFile>((Func<SFFile, SFFile>) (f => f)).Where<SFFile>((Func<SFFile, bool>) (f => (f.FileInfo.Attributes & FileAttributes.System) == (FileAttributes) 0)).ToList<SFFile>();
        }
        else
        {
          source = source.Select<SFFile, SFFile>((Func<SFFile, SFFile>) (f => f)).Where<SFFile>((Func<SFFile, bool>) (f => (f.FileInfo.Attributes & FileAttributes.System) == (FileAttributes) 0)).ToList<SFFile>();
          source = source.Select<SFFile, SFFile>((Func<SFFile, SFFile>) (f => f)).Where<SFFile>((Func<SFFile, bool>) (f => (f.FileInfo.Attributes & FileAttributes.Hidden) == (FileAttributes) 0)).ToList<SFFile>();
        }
      }
      List<string> list = ((IEnumerable<string>) options.FilterValue.Split('|')).ToList<string>();
      if (options.MatchCase)
      {
        for (int index = 0; index <= list.Count - 1; ++index)
        {
          string value = list[index];
          if (value.IndexOf("*") > -1)
          {
            if (options.Filter == SFIO.FilterFilesType.Match)
              options.Filter = SFIO.FilterFilesType.Pattern;
          }
          else if (options.Filter == SFIO.FilterFilesType.Pattern)
            options.Filter = SFIO.FilterFilesType.Match;
          if (Path.GetExtension(value) != "" && options.FilterOn == SFIO.FilterFilesOnType.Name)
            options.FilterOn = SFIO.FilterFilesOnType.FullName;
          List<SFFile> source1 = (List<SFFile>) null;
          switch (options.Filter)
          {
            case SFIO.FilterFilesType.Match:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension == value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName == value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name == value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension == value)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.Contains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.IndexOf(value) > -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.IndexOf(value) > -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.IndexOf(value) > -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.IndexOf(value) > -1)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.EndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.EndsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.StartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.StartsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotStartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.NameNoExtension.StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.FullName.StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Name.StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Extension.StartsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotEndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.NameNoExtension.EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.FullName.EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Name.EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Extension.EndsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotContains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.IndexOf(value) == -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.IndexOf(value) == -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.IndexOf(value) == -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.IndexOf(value) == -1)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotMatch:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension != value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName != value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name != value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension != value)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.Pattern:
              string[] patternValues = value.Split('*');
              if (value.StartsWith("*"))
              {
                value = value.Replace("*", "");
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.EndsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.EndsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.EndsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.EndsWith(value))).ToList<SFFile>();
                    break;
                }
              }
              else if (value.EndsWith("*"))
              {
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.StartsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.StartsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.StartsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.StartsWith(value))).ToList<SFFile>();
                    break;
                }
              }
              if (patternValues.Length > 1)
              {
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.NameNoExtension.StartsWith(patternValues[0]))
                        return n.NameNoExtension.EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.FullName.StartsWith(patternValues[0]))
                        return n.FullName.EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.Name.StartsWith(patternValues[0]))
                        return n.Name.EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.Extension.StartsWith(patternValues[0]))
                        return n.Extension.EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                }
              }
              else
                break;
          }
          if (source1 != null && source1.Count > 0)
          {
            source1.All<SFFile>((Func<SFFile, bool>) (xn =>
            {
              xn.Pattern = value;
              return true;
            }));
            try
            {
              listOfFiles.AddRange((IEnumerable<SFFile>) source1.Where<SFFile>((Func<SFFile, bool>) (p => !listOfFiles.Any<SFFile>((Func<SFFile, bool>) (p2 => p2.FullName.ToLower() == p.FullName.ToLower())))).ToList<SFFile>());
            }
            catch
            {
            }
          }
        }
      }
      else
      {
        for (int index = 0; index <= list.Count - 1; ++index)
        {
          string value = list[index].ToLower();
          List<SFFile> source1 = (List<SFFile>) null;
          if (value.IndexOf("*") > -1)
          {
            if (options.Filter == SFIO.FilterFilesType.Match)
              options.Filter = SFIO.FilterFilesType.Pattern;
          }
          else if (options.Filter == SFIO.FilterFilesType.Pattern)
            options.Filter = SFIO.FilterFilesType.Match;
          if (Path.GetExtension(value) != "" && options.FilterOn == SFIO.FilterFilesOnType.Name)
            options.FilterOn = SFIO.FilterFilesOnType.FullName;
          switch (options.Filter)
          {
            case SFIO.FilterFilesType.Match:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower() == value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower() == value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower() == value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower() == value)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.Contains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower().IndexOf(value) > -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower().IndexOf(value) > -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower().IndexOf(value) > -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower().IndexOf(value) > -1)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.EndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.StartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotStartsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.NameNoExtension.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.FullName.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Name.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Extension.ToLower().StartsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotEndsWith:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.NameNoExtension.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.FullName.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Name.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => !n.Extension.ToLower().EndsWith(value))).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotContains:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower().IndexOf(value) == -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower().IndexOf(value) == -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower().IndexOf(value) == -1)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower().IndexOf(value) == -1)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.NotMatch:
              switch (options.FilterOn)
              {
                case SFIO.FilterFilesOnType.Name:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower() != value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullPath:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower() != value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.FullName:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower() != value)).ToList<SFFile>();
                  break;
                case SFIO.FilterFilesOnType.Extension:
                  source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower() != value)).ToList<SFFile>();
                  break;
              }
            case SFIO.FilterFilesType.Pattern:
              string[] patternValues = value.Split('*');
              if (value.StartsWith("*"))
              {
                value = value.Replace("*", "");
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower().EndsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower().EndsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower().EndsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower().EndsWith(value))).ToList<SFFile>();
                    break;
                }
              }
              else if (value.EndsWith("*"))
              {
                value = value.Replace("*", "");
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.NameNoExtension.ToLower().StartsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.FullName.ToLower().StartsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Name.ToLower().StartsWith(value))).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n => n.Extension.ToLower().StartsWith(value))).ToList<SFFile>();
                    break;
                }
              }
              if (patternValues.Length > 1)
              {
                switch (options.FilterOn)
                {
                  case SFIO.FilterFilesOnType.Name:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.NameNoExtension.ToLower().StartsWith(patternValues[0]))
                        return n.NameNoExtension.ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullPath:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.FullName.ToLower().StartsWith(patternValues[0]))
                        return n.FullName.ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.FullName:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.Name.ToLower().StartsWith(patternValues[0]))
                        return n.Name.ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                  case SFIO.FilterFilesOnType.Extension:
                    source1 = source.Where<SFFile>((Func<SFFile, bool>) (n =>
                    {
                      if (n.Extension.ToLower().StartsWith(patternValues[0]))
                        return n.Extension.ToLower().EndsWith(patternValues[1]);
                      return false;
                    })).ToList<SFFile>();
                    break;
                }
              }
              else
                break;
          }
          if (source1 != null && source1.Count > 0)
          {
            source1.All<SFFile>((Func<SFFile, bool>) (xn =>
            {
              xn.Pattern = value;
              return true;
            }));
            try
            {
              listOfFiles.AddRange((IEnumerable<SFFile>) source1.Where<SFFile>((Func<SFFile, bool>) (p => !listOfFiles.Any<SFFile>((Func<SFFile, bool>) (p2 => p2.FullName.ToLower() == p.FullName.ToLower())))).ToList<SFFile>());
            }
            catch
            {
            }
          }
        }
      }
      if (listOfFiles != null && listOfFiles.Count > 0 && (sortOptions != null && sortOptions.Sort != SFIO.SortType.None))
      {
        if (string.IsNullOrEmpty(sortOptions.PropertyName))
          sortOptions.PropertyName = "Name";
        if (sortOptions.Sort == SFIO.SortType.Ascending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Ascending));
        }
        else if (sortOptions.Sort == SFIO.SortType.Descending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Descending));
        }
        listOfFiles.Sort((IComparer<SFFile>) genericComparer);
      }
      return listOfFiles;
    }

    public static Encoding GetFileEncoding(string fileName)
    {
      byte[] buffer = new byte[4];
      using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        fileStream.Read(buffer, 0, 4);
      if ((int) buffer[0] == 43 && (int) buffer[1] == 47 && (int) buffer[2] == 118)
        return Encoding.UTF7;
      if ((int) buffer[0] == 239 && (int) buffer[1] == 187 && (int) buffer[2] == 191)
        return Encoding.UTF8;
      if ((int) buffer[0] == (int) byte.MaxValue && (int) buffer[1] == 254)
        return Encoding.Unicode;
      if ((int) buffer[0] == 254 && (int) buffer[1] == (int) byte.MaxValue)
        return Encoding.BigEndianUnicode;
      if ((int) buffer[0] == 0 && (int) buffer[1] == 0 && ((int) buffer[2] == 254 && (int) buffer[3] == (int) byte.MaxValue))
        return Encoding.UTF32;
      return Encoding.Default;
    }

    public static List<FileInfo> GetFiles(string folder)
    {
      return SFIO.GetFiles(folder, (FileFilterOptions) null, (FileSortOptions) null, false, "");
    }

    public static List<FileInfo> GetFiles(string folder, bool includeSubFolders)
    {
      return SFIO.GetFiles(folder, (FileFilterOptions) null, (FileSortOptions) null, includeSubFolders, "");
    }

    public static List<FileInfo> GetFiles(string folder, FileFilterOptions options)
    {
      return SFIO.GetFiles(folder, options, (FileSortOptions) null, false, "");
    }

    public static List<FileInfo> GetFiles(string folder, FileSortOptions sortOptions)
    {
      return SFIO.GetFiles(folder, (FileFilterOptions) null, sortOptions, false, "");
    }

    public static List<FileInfo> GetFiles(string folder, FileSortOptions sortOptions, bool includeSubFolders)
    {
      return SFIO.GetFiles(folder, (FileFilterOptions) null, sortOptions, includeSubFolders, "");
    }

    public static List<FileInfo> GetFiles(string folder, FileFilterOptions options, bool includeSubFolders)
    {
      return SFIO.GetFiles(folder, options, (FileSortOptions) null, includeSubFolders, "");
    }

    public static List<FileInfo> GetFiles(string folder, FileFilterOptions options, FileSortOptions sortOptions)
    {
      return SFIO.GetFiles(folder, options, sortOptions, false, "");
    }

    public static List<FileInfo> GetFiles(string folder, FileFilterOptions options, FileSortOptions sortOptions, bool includeSubFolders)
    {
      return SFIO.GetFiles(folder, options, sortOptions, includeSubFolders, "");
    }

    public static List<SFFile> GetSFFiles(string folder)
    {
      return SFIO.GetSFFiles(folder, (FileFilterOptions) null, (FileSortOptions) null, false, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, bool includeSubFolders)
    {
      return SFIO.GetSFFiles(folder, (FileFilterOptions) null, (FileSortOptions) null, includeSubFolders, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, bool includeSubFolders, bool imageDimensionsOn)
    {
      return SFIO.GetSFFiles(folder, (FileFilterOptions) null, (FileSortOptions) null, includeSubFolders, "", imageDimensionsOn);
    }

    public static List<SFFile> GetSFFiles(string folder, FileFilterOptions options)
    {
      return SFIO.GetSFFiles(folder, options, (FileSortOptions) null, false, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, FileSortOptions sortOptions)
    {
      return SFIO.GetSFFiles(folder, (FileFilterOptions) null, sortOptions, false, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, FileSortOptions sortOptions, bool includeSubFolders)
    {
      return SFIO.GetSFFiles(folder, (FileFilterOptions) null, sortOptions, includeSubFolders, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, FileSortOptions sortOptions, bool includeSubFolders, bool imageDimensionsOn)
    {
      return SFIO.GetSFFiles(folder, (FileFilterOptions) null, sortOptions, includeSubFolders, "", imageDimensionsOn);
    }

    public static List<SFFile> GetSFFiles(string folder, FileFilterOptions options, bool includeSubFolders)
    {
      return SFIO.GetSFFiles(folder, options, (FileSortOptions) null, includeSubFolders, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, FileFilterOptions options, bool includeSubFolders, bool imageDimensionsOn)
    {
      return SFIO.GetSFFiles(folder, options, (FileSortOptions) null, includeSubFolders, "", imageDimensionsOn);
    }

    public static List<SFFile> GetSFFiles(string folder, FileFilterOptions options, FileSortOptions sortOptions)
    {
      return SFIO.GetSFFiles(folder, options, sortOptions, false, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, FileFilterOptions options, FileSortOptions sortOptions, bool includeSubFolders)
    {
      return SFIO.GetSFFiles(folder, options, sortOptions, includeSubFolders, "", false);
    }

    public static List<SFFile> GetSFFiles(string folder, FileFilterOptions options, FileSortOptions sortOptions, bool includeSubFolders, bool imageDimensionsOn)
    {
      return SFIO.GetSFFiles(folder, options, sortOptions, includeSubFolders, "", imageDimensionsOn);
    }

    public static bool IsImage(string file)
    {
      if (string.IsNullOrEmpty(file))
        return false;
      file = file.ToLower();
      return file.EndsWith(".png") || file.EndsWith(".bmp") || (file.EndsWith(".jpg") || file.EndsWith(".jpeg")) || (file.EndsWith(".gif") || file.EndsWith(".tif") || (file.EndsWith(".tag") || file.EndsWith(".psd")));
    }

    public static void RenameFiles(string folder, string filter, string valueToRename, string renameTo, bool makeCopy, FileFilterOptions options)
    {
      List<FileInfo> fileInfoList1 = new List<FileInfo>();
      List<FileInfo> files = SFIO.GetFiles(folder, options);
      if (files == null || files.Count <= 0)
        return;
      FileFilterOptions options1 = new FileFilterOptions()
      {
        FilterOn = SFIO.FilterFilesOnType.Name,
        Filter = SFIO.FilterFilesType.Contains,
        FilterValue = valueToRename
      };
      List<FileInfo> fileInfoList2 = SFIO.FilterFiles(files, options1);
      if (fileInfoList2.Count <= 0)
        return;
      for (int index = 0; index <= fileInfoList2.Count - 1; ++index)
      {
        string fullName = fileInfoList2[index].FullName;
        string newValue = Path.GetFileName(fullName).Replace(valueToRename, renameTo);
        string destFileName = fullName.Replace(Path.GetFileName(fullName), newValue);
        try
        {
          File.Copy(fullName, destFileName);
        }
        catch
        {
        }
        if (!makeCopy)
        {
          try
          {
            File.Delete(fullName);
          }
          catch
          {
          }
        }
      }
    }

    public static bool SaveBinary(string fileName, byte[] data)
    {
      bool flag = false;
      if (string.IsNullOrEmpty(fileName))
        return false;
      if (data == null)
        return false;
      try
      {
        FileStream fileStream = File.Create(fileName, 2048, FileOptions.None);
        BinaryWriter binaryWriter = new BinaryWriter((Stream) fileStream);
        byte[] buffer = data;
        binaryWriter.Write(buffer);
        binaryWriter.Close();
        fileStream.Close();
        return true;
      }
      catch (Exception ex)
      {
        flag = false;
        throw new Exception("Binary File Save Failed: " + fileName, ex);
      }
    }

    public static void SetFilesReadOnly(string folder, bool setTrue)
    {
      SFIO.SetFilesReadOnly(folder, setTrue, (FileFilterOptions) null);
    }

    public static void SetFilesReadOnly(string folder, bool setTrue, FileFilterOptions options)
    {
      SFIO.SetFilesReadOnly(SFIO.GetSFFiles(folder, options, true), setTrue);
    }

    public static void SetFilesReadOnly(List<SFFile> sourceFiles, bool setTrue)
    {
      foreach (SFFile sourceFile in sourceFiles)
        sourceFile.FileInfo.IsReadOnly = setTrue;
    }

    public static void WireupCSV<T>(ref T sourceObject, List<string> fields, List<string> data)
    {
      if (fields == null || fields.Count == 0 || (data == null || data.Count == 0) || data.Count < fields.Count)
        return;
      foreach (PropertyInfo property in sourceObject.GetType().GetProperties())
      {
        if (property.CanRead && property.CanWrite)
        {
          FieldNameAttribute[] customAttributes = (FieldNameAttribute[]) property.GetCustomAttributes(typeof (FieldNameAttribute), false);
          int index1 = -1;
          for (int index2 = 0; index2 <= fields.Count - 1; ++index2)
          {
            if (fields[index2].Trim().ToLower() == property.Name.Trim().ToLower())
              index1 = index2;
            else if (customAttributes != null && customAttributes.Length != 0 && fields[index2].Trim().ToLower() == customAttributes[0].AttributeValue.Trim().ToLower())
              index1 = index2;
          }
          if (index1 > -1)
          {
            if (index1 < data.Count)
            {
              try
              {
                object obj = (object) null;
                Type type = property.GetValue((object) sourceObject, (object[]) null).GetType();
                if ((object) type == (object) typeof (string))
                {
                  try
                  {
                    obj = (object) data[index1].Trim();
                  }
                  catch
                  {
                    obj = (object) "";
                  }
                }
                else if ((object) type == (object) typeof (int) || (object) type == (object) typeof (int))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    if (SFStrings.IsNumeric(data[index1]))
                    {
                      try
                      {
                        obj = (object) Convert.ToInt32(data[index1].Trim());
                        goto label_62;
                      }
                      catch
                      {
                        obj = (object) Convert.ToInt32(0);
                        goto label_62;
                      }
                    }
                  }
                  obj = (object) Convert.ToInt32(0);
                }
                else if ((object) type == (object) typeof (short))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    if (SFStrings.IsNumeric(data[index1]))
                    {
                      try
                      {
                        obj = (object) Convert.ToInt16(data[index1].Trim());
                        goto label_62;
                      }
                      catch
                      {
                        obj = (object) Convert.ToInt16(0);
                        goto label_62;
                      }
                    }
                  }
                  obj = (object) Convert.ToInt16(0);
                }
                else if ((object) type == (object) typeof (long))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    if (SFStrings.IsNumeric(data[index1]))
                    {
                      try
                      {
                        obj = (object) Convert.ToInt64(data[index1].Trim());
                        goto label_62;
                      }
                      catch
                      {
                        obj = (object) Convert.ToInt64(0);
                        goto label_62;
                      }
                    }
                  }
                  obj = (object) Convert.ToInt64(0);
                }
                else if ((object) type == (object) typeof (float))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    if (SFStrings.IsNumeric(data[index1]))
                    {
                      try
                      {
                        obj = (object) Convert.ToSingle(data[index1]);
                        goto label_62;
                      }
                      catch
                      {
                        obj = (object) Convert.ToSingle(0);
                        goto label_62;
                      }
                    }
                  }
                  obj = (object) Convert.ToSingle(0);
                }
                else if ((object) type == (object) typeof (double))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    if (SFStrings.IsNumeric(data[index1]))
                    {
                      try
                      {
                        obj = (object) Convert.ToDouble(data[index1].Trim());
                        goto label_62;
                      }
                      catch
                      {
                        obj = (object) Convert.ToDouble(0);
                        goto label_62;
                      }
                    }
                  }
                  obj = (object) Convert.ToDouble(0);
                }
                else if ((object) type == (object) typeof (Decimal))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    if (SFStrings.IsNumeric(data[index1]))
                    {
                      try
                      {
                        obj = (object) Convert.ToDecimal(data[index1].Trim());
                        goto label_62;
                      }
                      catch
                      {
                        obj = (object) Convert.ToDecimal(0);
                        goto label_62;
                      }
                    }
                  }
                  obj = (object) Convert.ToDecimal(0);
                }
                else if ((object) type == (object) typeof (bool))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    try
                    {
                      obj = (object) Convert.ToBoolean(data[index1].Trim());
                    }
                    catch
                    {
                      obj = (object) false;
                    }
                  }
                  else
                    obj = (object) Convert.ToDouble(0);
                }
                else if ((object) type == (object) typeof (DateTime))
                {
                  if (!string.IsNullOrEmpty(data[index1]))
                  {
                    try
                    {
                      obj = (object) SFStrings.ToDate(data[index1]);
                    }
                    catch
                    {
                      obj = (object) new DateTime(1900, 1, 1);
                    }
                  }
                  else
                    obj = (object) new DateTime(1900, 1, 1);
                }
label_62:
                if (obj != null)
                  property.SetValue((object) sourceObject, obj, (object[]) null);
              }
              catch
              {
              }
            }
          }
        }
      }
    }

    public static void CopyFolders(List<string> folders, string newRoot)
    {
      SFIO.CopyFolders(folders, "", newRoot);
    }

    public static void CopyFolders(List<string> folders, string root, string newRoot)
    {
      if (!Directory.Exists(newRoot) || folders == null || folders.Count <= 0)
        return;
      for (int index = 0; index < folders.Count; ++index)
      {
        string path = !(root == "") ? folders[index].Replace(root, newRoot) : newRoot + "\\" + folders[index];
        try
        {
          if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        }
        catch
        {
        }
      }
    }

    public static List<string> GetFolders(string folder, bool includeSubFolders)
    {
      return SFIO.GetFolders(folder, includeSubFolders, "");
    }

    public static List<string> GetFolders(string folder, bool includeSubFolders, string root)
    {
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = ((IEnumerable<string>) Directory.GetDirectories(folder)).ToList<string>();
      if (stringList2 != null && stringList2.Count > 0 && includeSubFolders)
      {
        for (int index = 0; index < stringList2.Count; ++index)
        {
          List<string> folders = SFIO.GetFolders(stringList2[index], true);
          if (folders != null && folders.Count > 0)
            stringList2.AddRange((IEnumerable<string>) folders);
        }
      }
      if (stringList2 == null)
        stringList2 = new List<string>();
      else if (root != "")
      {
        for (int index = 0; index < stringList2.Count; ++index)
          stringList2[index] = stringList2[index].Replace(root, "");
      }
      return stringList2;
    }

    private static Size DecodeBitmap(BinaryReader binaryReader)
    {
      binaryReader.ReadBytes(16);
      return new Size(binaryReader.ReadInt32(), binaryReader.ReadInt32());
    }

    private static Size DecodeGif(BinaryReader binaryReader)
    {
      return new Size((int) binaryReader.ReadInt16(), (int) binaryReader.ReadInt16());
    }

    private static Size DecodeJfif(BinaryReader binaryReader)
    {
      while ((int) binaryReader.ReadByte() == (int) byte.MaxValue)
      {
        int num1 = (int) binaryReader.ReadByte();
        short num2 = SFIO.ReadLittleEndianInt16(binaryReader);
        int num3 = 192;
        if (num1 == num3)
        {
          int num4 = (int) binaryReader.ReadByte();
          int height = (int) SFIO.ReadLittleEndianInt16(binaryReader);
          return new Size((int) SFIO.ReadLittleEndianInt16(binaryReader), height);
        }
        binaryReader.ReadBytes((int) num2 - 2);
      }
      throw new ArgumentException("Could not recognize image format.");
    }

    private static Size DecodePng(BinaryReader binaryReader)
    {
      binaryReader.ReadBytes(8);
      return new Size(SFIO.ReadLittleEndianInt32(binaryReader), SFIO.ReadLittleEndianInt32(binaryReader));
    }

    private static List<FileInfo> GetFiles(string folder, FileFilterOptions options, FileSortOptions sortOptions, bool includeSubFolders, string subFolder)
    {
      List<FileInfo> fileInfoList = new List<FileInfo>();
      GenericComparer<FileInfo> genericComparer = new GenericComparer<FileInfo>();
      bool flag = false;
      if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
        return new List<FileInfo>();
      List<FileInfo> source = ((IEnumerable<FileInfo>) new DirectoryInfo(folder).GetFiles("*.*")).ToList<FileInfo>();
      if (options != null)
        source = SFIO.FilterFiles(source, options);
      if (subFolder == "")
      {
        flag = true;
        subFolder = folder;
      }
      if (includeSubFolders)
      {
        string[] directories = Directory.GetDirectories(folder);
        if (directories != null && directories.Length != 0)
        {
          for (int index = 0; index < directories.Length; ++index)
          {
            List<FileInfo> files = SFIO.GetFiles(directories[index], options, sortOptions, true, subFolder);
            if (files != null && files.Count > 0)
              source.AddRange((IEnumerable<FileInfo>) files);
          }
        }
      }
      if (flag && source.Count > 0 && (sortOptions != null && sortOptions.Sort != SFIO.SortType.None))
      {
        if (string.IsNullOrEmpty(sortOptions.PropertyName))
          sortOptions.PropertyName = "Name";
        if (sortOptions.Sort == SFIO.SortType.Ascending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Ascending));
        }
        else if (sortOptions.Sort == SFIO.SortType.Descending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Descending));
        }
        source.Sort((IComparer<FileInfo>) genericComparer);
      }
      return source;
    }

    private static Size GetImageDimensions(string path)
    {
      if (!File.Exists(path))
        return new Size(0, 0);
      using (BinaryReader binaryReader = new BinaryReader((Stream) File.OpenRead(path)))
      {
        try
        {
          return SFIO.GetImageDimensions(binaryReader);
        }
        catch
        {
          return new Size(0, 0);
        }
      }
    }

    private static Size GetImageDimensions(BinaryReader binaryReader)
    {
      int length = SFIO.imageFormatDecoders.Keys.OrderByDescending<byte[], int>((Func<byte[], int>) (x => x.Length)).First<byte[]>().Length;
      byte[] thisBytes = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        thisBytes[index] = binaryReader.ReadByte();
        foreach (KeyValuePair<byte[], Func<BinaryReader, Size>> imageFormatDecoder in SFIO.imageFormatDecoders)
        {
          if (SFIO.StartsWith(thisBytes, imageFormatDecoder.Key))
            return imageFormatDecoder.Value(binaryReader);
        }
      }
      throw new ArgumentException("Could not recognize image format.", "binaryReader");
    }

    private static List<SFFile> GetSFFiles(string folder, FileFilterOptions options, FileSortOptions sortOptions, bool includeSubFolders, string subFolder, bool imageDimensionsOn)
    {
      List<FileInfo> fileInfoList = new List<FileInfo>();
      GenericComparer<SFFile> genericComparer = new GenericComparer<SFFile>();
      bool flag = false;
      if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
        return new List<SFFile>();
      List<FileInfo> list = ((IEnumerable<FileInfo>) new DirectoryInfo(folder).GetFiles("*.*")).ToList<FileInfo>();
      List<SFFile> source = new List<SFFile>();
      for (int index = 0; index < list.Count; ++index)
      {
        SFFile sfFile = new SFFile(list[index], subFolder);
        if (imageDimensionsOn && SFIO.IsImage(sfFile.FullName))
        {
          Size imageDimensions = SFIO.GetImageDimensions(sfFile.FullName);
          sfFile.Width = imageDimensions.Width;
          sfFile.Height = imageDimensions.Height;
        }
        source.Add(sfFile);
      }
      if (options != null)
        source = SFIO.FilterFiles(source, options);
      if (subFolder == "")
      {
        flag = true;
        subFolder = folder;
      }
      if (includeSubFolders)
      {
        string[] directories = Directory.GetDirectories(folder);
        if (directories != null && directories.Length != 0)
        {
          for (int index = 0; index < directories.Length; ++index)
          {
            List<SFFile> sfFiles = SFIO.GetSFFiles(directories[index], options, sortOptions, true, subFolder, imageDimensionsOn);
            if (sfFiles != null && sfFiles.Count > 0)
              source.AddRange((IEnumerable<SFFile>) sfFiles);
          }
        }
      }
      if (flag && source.Count > 0 && (sortOptions != null && sortOptions.Sort != SFIO.SortType.None))
      {
        if (string.IsNullOrEmpty(sortOptions.PropertyName))
          sortOptions.PropertyName = "Name";
        if (sortOptions.Sort == SFIO.SortType.Ascending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Ascending));
        }
        else if (sortOptions.Sort == SFIO.SortType.Descending)
        {
          string propertyName = sortOptions.PropertyName;
          char[] chArray = new char[1]{ '|' };
          foreach (string fieldName in propertyName.Split(chArray))
            genericComparer.SortFields.Add(new SortFieldItem(fieldName, SortFieldItem.SortOrderType.Descending));
        }
        source.Sort((IComparer<SFFile>) genericComparer);
      }
      return source;
    }

    private static short ReadLittleEndianInt16(BinaryReader binaryReader)
    {
      byte[] numArray = new byte[2];
      for (int index = 0; index < 2; ++index)
        numArray[1 - index] = binaryReader.ReadByte();
      return BitConverter.ToInt16(numArray, 0);
    }

    private static int ReadLittleEndianInt32(BinaryReader binaryReader)
    {
      byte[] numArray = new byte[4];
      for (int index = 0; index < 4; ++index)
        numArray[3 - index] = binaryReader.ReadByte();
      return BitConverter.ToInt32(numArray, 0);
    }

    private static bool StartsWith(byte[] thisBytes, byte[] thatBytes)
    {
      for (int index = 0; index < thatBytes.Length; ++index)
      {
        if ((int) thisBytes[index] != (int) thatBytes[index])
          return false;
      }
      return true;
    }

    public enum FilterFilesOnType
    {
      Name,
      FullPath,
      FullName,
      Extension,
      None,
    }

    public enum FilterFilesType
    {
      Match,
      Contains,
      EndsWith,
      StartsWith,
      NotStartsWith,
      NotEndsWith,
      NotContains,
      NotMatch,
      Pattern,
    }

    public enum FilterSystemType
    {
      Hidden,
      System,
      Both,
      None,
    }

    public enum SortType
    {
      None,
      Ascending,
      Descending,
    }
  }
}
