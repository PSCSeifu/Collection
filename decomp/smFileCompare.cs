// Decompiled with JetBrains decompiler
// Type: Wyvern.Base.SFFileCompare
// Assembly: Wyvern.Base, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C96879FF-8A02-4EC1-BB96-AE29A19B1BA7
// Assembly location: D:\WYVERN\AssembliesX\Wyvern.Base.dll

using System.Collections.Generic;

namespace Wyvern.Base
{
  public class SFFileCompare
  {
    public SFFile FileA { get; set; }

    public SFFile FileB { get; set; }

    public string FileNameA
    {
      get
      {
        if (this.FileA != null)
          return this.FileA.Name;
        return "";
      }
    }

    public string FileNameB
    {
      get
      {
        if (this.FileB != null)
          return this.FileB.Name;
        return "";
      }
    }

    public bool MatchByteSize
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.FileA.ByteSize > 0L && this.FileA.ByteSize == this.FileB.ByteSize);
      }
    }

    public bool MatchContains
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.FileA.NameNoExtension.ToLower().IndexOf(this.FileB.NameNoExtension.ToLower()) > -1 || this.FileB.NameNoExtension.ToLower().IndexOf(this.FileA.NameNoExtension.ToLower()) > -1);
      }
    }

    public bool MatchDate
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && this.FileA.DateModified == this.FileB.DateModified;
      }
    }

    public bool MatchDirectory
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && this.FileA.Directory.ToLower() == this.FileB.Directory.ToLower();
      }
    }

    public bool MatchEndsWith
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.FileA.NameNoExtension.ToLower().EndsWith(this.FileB.NameNoExtension.ToLower()) || this.FileB.NameNoExtension.ToLower().EndsWith(this.FileA.NameNoExtension.ToLower()));
      }
    }

    public bool MatchExtension
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && this.FileA.Extension.ToLower() == this.FileB.Extension.ToLower();
      }
    }

    public bool MatchFullName
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && this.FileA.FullName.ToLower() == this.FileB.FullName.ToLower();
      }
    }

    public bool MatchHeight
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.FileA.Height > 0 && this.FileA.Height == this.FileB.Height);
      }
    }

    public bool MatchName
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && this.FileA.Name.ToLower() == this.FileB.Name.ToLower();
      }
    }

    public bool MatchNameAndBytes
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.MatchName && this.MatchByteSize);
      }
    }

    public bool MatchNameAndSize
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.MatchName && this.MatchWidthAndHeight);
      }
    }

    public bool MatchNameSizeAndBytes
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.MatchHeight && this.MatchWidth && (this.MatchName && this.MatchByteSize));
      }
    }

    public bool MatchNameTypes
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.MatchStartsWith || this.MatchEndsWith || this.MatchContains);
      }
    }

    public bool MatchNameTypesAndExtension
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && this.MatchExtension && (this.MatchStartsWith || this.MatchEndsWith || this.MatchContains);
      }
    }

    public bool MatchNameWithoutExt
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && this.FileA.NameNoExtension.ToLower() == this.FileB.NameNoExtension.ToLower();
      }
    }

    public bool MatchStartsWith
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.FileA.NameNoExtension.ToLower().StartsWith(this.FileB.NameNoExtension.ToLower()) || this.FileB.NameNoExtension.ToLower().StartsWith(this.FileA.NameNoExtension.ToLower()));
      }
    }

    public bool MatchWidth
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.FileA.Width > 0 && this.FileA.Width == this.FileB.Width);
      }
    }

    public bool MatchWidthAndHeight
    {
      get
      {
        return this.FileA != null && this.FileB != null && (this.FileA.Name != "" && this.FileB.Name != "") && (this.MatchHeight && this.MatchWidth);
      }
    }

    public SFFileCompare()
    {
      this.FileA = new SFFile();
      this.FileB = new SFFile();
    }

    public List<string> GetProperties()
    {
      List<string> stringList = new List<string>();
      if (this.MatchNameTypes)
        stringList.Add("Name");
      if (this.MatchByteSize)
        stringList.Add("ByteSize");
      if (this.MatchDate)
        stringList.Add("DateModified");
      if (this.MatchDirectory)
        stringList.Add("Directory");
      if (this.MatchHeight)
        stringList.Add("Height");
      if (this.MatchWidth)
        stringList.Add("Width");
      return stringList;
    }

    public string GetSummary()
    {
      string str = this.FileA.Name + " - " + this.FileB.Name;
      if (this.MatchByteSize)
        str = str + " B:" + (object) this.FileA.ByteSize;
      if (this.MatchDate)
        str = str + " DT:" + this.FileA.DateModified.ToString("dd/MM/yyyy hh:mm:ss");
      if (this.MatchDirectory)
        str = str + " D:" + this.FileA.Directory;
      if (this.MatchHeight)
        str = str + " H:" + (object) this.FileA.Height;
      if (this.MatchWidth)
        str = str + " W:" + (object) this.FileA.Width;
      if (this.MatchStartsWith)
        str = str + " SW:" + this.FileA.NameNoExtension;
      return str;
    }
  }
}
