// Decompiled with JetBrains decompiler
// Type: Wyvern.Base.SFFile
// Assembly: Wyvern.Base, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C96879FF-8A02-4EC1-BB96-AE29A19B1BA7
// Assembly location: D:\WYVERN\AssembliesX\Wyvern.Base.dll

using System;
using System.ComponentModel;
using System.IO;

namespace Wyvern.Base
{
  [Serializable]
  public class SFFile
  {
    private string c_name = "";
    private string c_subDirectory = "";
    private FileInfo c_baseFile;

    [Category("Size")]
    public long ByteSize
    {
      get
      {
        if (this.c_baseFile != null)
          return this.c_baseFile.Length;
        return 0;
      }
    }

    [Category("Custom")]
    public bool Checked { get; set; }

    [Category("File")]
    public DateTime DateModified
    {
      get
      {
        if (this.c_baseFile == null)
          return new DateTime(1900, 1, 1);
        if (string.IsNullOrEmpty(this.c_baseFile.FullName))
          return new DateTime(1900, 1, 1);
        try
        {
          return System.IO.File.GetLastWriteTime(this.c_baseFile.FullName);
        }
        catch
        {
          return new DateTime(1900, 1, 1);
        }
      }
    }

    [Category("Directory")]
    public string Directory
    {
      get
      {
        if (string.IsNullOrEmpty(this.c_name))
          return "";
        return this.c_name.Replace(this.Name, "");
      }
    }

    [Category("Attributes")]
    public bool Exists
    {
      get
      {
        return this.c_baseFile != null;
      }
    }

    [Category("File")]
    public string Extension
    {
      get
      {
        if (string.IsNullOrEmpty(this.c_name))
          return "";
        return Path.GetExtension(this.c_name);
      }
    }

    [Category("File")]
    public string File
    {
      get
      {
        return this.c_name;
      }
      set
      {
        if (!(this.c_name.ToLower() != value.ToLower()))
          return;
        this.SetFile(value, "");
      }
    }

    [Browsable(false)]
    public FileInfo FileInfo
    {
      get
      {
        return this.c_baseFile;
      }
    }

    [Category("File")]
    public string FullName
    {
      get
      {
        if (string.IsNullOrEmpty(this.c_name))
          return "";
        return this.c_name;
      }
    }

    [Category("Size")]
    public int Height { get; set; }

    [Category("Custom")]
    public int Index { get; set; }

    [Category("Attributes")]
    public bool IsHidden
    {
      get
      {
        if (this.c_baseFile != null)
          return (this.c_baseFile.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        return false;
      }
    }

    [Category("Attributes")]
    public bool IsSystem
    {
      get
      {
        if (this.c_baseFile != null)
          return (this.c_baseFile.Attributes & FileAttributes.System) == FileAttributes.System;
        return false;
      }
    }

    [Category("File")]
    public string Name
    {
      get
      {
        if (string.IsNullOrEmpty(this.c_name))
          return "";
        return Path.GetFileName(this.c_name);
      }
    }

    public bool IsImage
    {
      get
      {
        if (string.IsNullOrEmpty(this.c_name))
          return false;
        string lower = this.c_name.ToLower();
        return lower.EndsWith(".png") || lower.EndsWith(".bmp") || (lower.EndsWith(".jpg") || lower.EndsWith(".jpeg")) || (lower.EndsWith(".gif") || lower.EndsWith(".tif") || (lower.EndsWith(".tag") || lower.EndsWith(".psd")));
      }
    }

    [Category("File")]
    public string NameNoExtension
    {
      get
      {
        if (string.IsNullOrEmpty(this.c_name))
          return "";
        return Path.GetFileNameWithoutExtension(this.c_name);
      }
    }

    [Category("Custom")]
    public int PosX { get; set; }

    [Category("Custom")]
    public int PosY { get; set; }

    [Category("Custom")]
    public string SubDirectory
    {
      get
      {
        return this.c_subDirectory;
      }
    }

    [Category("Custom")]
    public string SubDirectoryName
    {
      get
      {
        return this.c_subDirectory + this.Name;
      }
    }

    [Category("Size")]
    public int Width { get; set; }

    public string Pattern { get; set; }

    public SFFile()
    {
      this.c_subDirectory = "";
      this.c_name = "";
      this.c_baseFile = (FileInfo) null;
      this.Pattern = "";
    }

    public SFFile(string file)
    {
      this.Pattern = "";
      this.SetFile(file, "");
    }

    public SFFile(string file, string masterDir)
    {
      this.SetFile(file, masterDir);
    }

    public SFFile(FileInfo file)
    {
      this.c_name = "";
      this.c_baseFile = file;
      this.c_subDirectory = "";
      this.Pattern = "";
      if (file == null)
        return;
      this.c_name = file.FullName;
    }

    public SFFile(FileInfo file, string masterDir)
    {
      this.Pattern = "";
      this.c_name = "";
      this.c_baseFile = file;
      this.c_subDirectory = "";
      this.Pattern = "";
      if (file == null)
        return;
      this.c_name = file.FullName;
      if (string.IsNullOrEmpty(masterDir))
        return;
      this.c_subDirectory = this.c_name.Replace(masterDir, "");
      this.c_subDirectory = this.c_subDirectory.Replace(this.Name, "");
      if (!this.c_subDirectory.StartsWith("\\"))
        return;
      this.c_subDirectory = this.c_subDirectory.Substring(1);
    }

    public SFFile.MatchFileResultType MatchFile(SFFile file)
    {
      if (!file.Exists)
        return SFFile.MatchFileResultType.Missing;
      if (file.DateModified != this.DateModified)
        return file.ByteSize != this.ByteSize ? SFFile.MatchFileResultType.SizeAndDate : SFFile.MatchFileResultType.Date;
      return file.ByteSize != this.ByteSize ? SFFile.MatchFileResultType.Size : SFFile.MatchFileResultType.Match;
    }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.c_name))
        return "";
      return Path.GetFileName(this.c_name);
    }

    private void SetFile(string file, string masterDir)
    {
      this.c_subDirectory = "";
      this.c_name = file;
      if (!string.IsNullOrEmpty(masterDir))
      {
        this.c_subDirectory = this.c_name.Replace(masterDir, "");
        this.c_subDirectory = this.c_subDirectory.Replace(this.Name, "");
      }
      if (System.IO.File.Exists(file))
        this.c_baseFile = new FileInfo(file);
      else
        this.c_baseFile = (FileInfo) null;
    }

    public enum MatchFileResultType
    {
      Match = 1,
      Missing = 2,
      Date = 3,
      Size = 4,
      SizeAndDate = 5,
    }
  }
}
