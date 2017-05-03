// Decompiled with JetBrains decompiler
// Type: Wyvern.Base.SFClass
// Assembly: Wyvern.Base, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C96879FF-8A02-4EC1-BB96-AE29A19B1BA7
// Assembly location: D:\WYVERN\AssembliesX\Wyvern.Base.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Wyvern.Base
{
  public class SFClass
  {
    public static string Category(object source, string propertyName)
    {
      if (source == null || string.IsNullOrEmpty(propertyName))
        return "Misc";
      return SFClass.Category(SFClass.GetPropertyInfo(source, propertyName));
    }

    public static string Category(PropertyInfo propInfo)
    {
      if (propInfo != (PropertyInfo) null)
      {
        CategoryAttribute[] customAttributes = (CategoryAttribute[]) propInfo.GetCustomAttributes(typeof (CategoryAttribute), false);
        if (customAttributes != null && customAttributes.Length != 0)
          return customAttributes[0].Category;
      }
      return "Misc";
    }

    public static T ConvertStringToObject<T>(string value, T defaultValue)
    {
      T obj = default (T);
      try
      {
        if (string.IsNullOrEmpty(value))
          return defaultValue;
        Type type = typeof (T);
        try
        {
          return (T) SFClass.ValueAs(type, value);
        }
        catch
        {
          return defaultValue;
        }
      }
      catch
      {
        return defaultValue;
      }
    }

    public static float GetCPUUsage()
    {
      PerformanceCounter performanceCounter = new PerformanceCounter();
      performanceCounter.CategoryName = "Processor";
      performanceCounter.CounterName = "% Processor Time";
      performanceCounter.InstanceName = "_Total";
      double num = (double) performanceCounter.NextValue();
      Thread.Sleep(1000);
      return performanceCounter.NextValue();
    }

    public static Dictionary<string, string> GetListAsDictionary<T>(List<T> sourceObject, string prefix)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      for (int index = 0; index < sourceObject.Count; ++index)
      {
        Dictionary<string, string> objectAsDictionary = SFClass.GetObjectAsDictionary<T>(sourceObject[index], prefix + (object) index + "_");
        if (objectAsDictionary != null && objectAsDictionary.Count > 0)
        {
          foreach (KeyValuePair<string, string> keyValuePair in objectAsDictionary)
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }
      }
      return dictionary;
    }

    public static Dictionary<string, string> GetObjectAsDictionary<T>(T sourceObject)
    {
      return SFClass.GetObjectAsDictionary<T>(sourceObject, "");
    }

    public static Dictionary<string, string> GetObjectAsDictionary<T>(T sourceObject, string prefix)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if ((object) sourceObject == null)
        return dictionary;
      foreach (PropertyInfo property in sourceObject.GetType().GetProperties())
      {
        try
        {
          string name = property.Name;
          if (property.CanRead)
          {
            object obj = property.GetValue((object) sourceObject, (object[]) null);
            string str = !(obj is DateTime) ? (!(obj is bool) ? property.GetValue((object) sourceObject, (object[]) null).ToString() : ((bool) obj ? "true" : "false")) : ((DateTime) obj).ToString("dd/MM/yyyy hh:mm:ss");
            if (str != null)
              dictionary.Add(prefix + name, str);
          }
        }
        catch
        {
        }
      }
      return dictionary;
    }

    public static string GetObjectAsString<T>(T sourceObject)
    {
      return SFClass.GetObjectAsString<T>(sourceObject, ",", false);
    }

    public static string GetObjectAsString<T>(T sourceObject, string delimiter)
    {
      return SFClass.GetObjectAsString<T>(sourceObject, delimiter, false);
    }

    public static string GetObjectAsString<T>(T sourceObject, string delimiter, bool newLine)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((object) sourceObject == null)
        return "";
      foreach (PropertyInfo property in sourceObject.GetType().GetProperties())
      {
        try
        {
          string name = property.Name;
          string str1 = "";
          if (newLine)
            str1 = " ";
          if (property.CanRead)
          {
            object obj = property.GetValue((object) sourceObject, (object[]) null);
            string str2 = !(obj is DateTime) ? (!(obj is bool) ? property.GetValue((object) sourceObject, (object[]) null).ToString() : ((bool) obj ? "true" : "false")) : ((DateTime) obj).ToString("dd/MM/yyyy hh:mm:ss");
            if (str2 != null)
            {
              if (stringBuilder.Length > 0)
                stringBuilder.Append(delimiter + str1);
              stringBuilder.Append(name).Append(":" + str1).Append(str2);
              if (newLine)
                stringBuilder.Append(Environment.NewLine);
            }
          }
        }
        catch
        {
        }
      }
      return stringBuilder.ToString();
    }

    public static PropertyInfo GetPropertyInfo(object source)
    {
      if (source == null)
        return (PropertyInfo) null;
      PropertyInfo[] properties = source.GetType().GetProperties();
      if (properties.Length != 0)
        return properties[0];
      return (PropertyInfo) null;
    }

    public static List<PropertyInfo> PropertyInfoGetAll(object source)
    {
      if (source == null)
        return (List<PropertyInfo>) null;
      return new List<PropertyInfo>((IEnumerable<PropertyInfo>) source.GetType().GetProperties());
    }

    public static void CloneObject(object cloneFrom, object cloneTo)
    {
      foreach (PropertyInfo propertyInfo in SFClass.PropertyInfoGetAll(cloneFrom))
      {
        try
        {
          SFClass.SetPropertyValue(cloneTo, propertyInfo.Name, propertyInfo.GetValue(cloneFrom));
        }
        catch
        {
        }
      }
    }

    public static object CloneObject(object source)
    {
      object instance = Activator.CreateInstance(source.GetType());
      foreach (PropertyInfo propertyInfo in SFClass.PropertyInfoGetAll(source))
      {
        try
        {
          SFClass.SetPropertyValue(instance, propertyInfo.Name, propertyInfo.GetValue(source));
        }
        catch
        {
        }
      }
      return instance;
    }

    public static T Clone<T>(T source)
    {
      if (!typeof (T).IsSerializable)
        throw new ArgumentException("The type must be serializable.", "source");
      if ((object) source == null)
        return default (T);
      IFormatter formatter = (IFormatter) new BinaryFormatter();
      Stream serializationStream = (Stream) new MemoryStream();
      using (serializationStream)
      {
        formatter.Serialize(serializationStream, (object) source);
        serializationStream.Seek(0L, SeekOrigin.Begin);
        return (T) formatter.Deserialize(serializationStream);
      }
    }

    public static PropertyInfo GetPropertyInfo(object source, string propertyName)
    {
      if (source == null || string.IsNullOrEmpty(propertyName))
        return (PropertyInfo) null;
      Type type = source.GetType();
      try
      {
        return type.GetProperty(propertyName);
      }
      catch
      {
      }
      return (PropertyInfo) null;
    }

    public static Dictionary<string, string> GetPropertyStringValues(object source, SFClass.ObjectPropertyAccessType access)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (source == null)
        return dictionary;
      foreach (PropertyInfo property in source.GetType().GetProperties())
      {
        try
        {
          bool flag = false;
          if (access == SFClass.ObjectPropertyAccessType.ReadWrite && property.CanRead && property.CanWrite)
            flag = true;
          if (access == SFClass.ObjectPropertyAccessType.ReadOnly && property.CanRead && !property.CanWrite)
            flag = true;
          if (flag)
          {
            string name = property.Name;
            string str = property.GetValue(source, (object[]) null).ToString();
            dictionary.Add(name, str);
          }
        }
        catch
        {
        }
      }
      return dictionary;
    }

    public static object GetPropertyValue(object source, string propertyName)
    {
      if (source == null || string.IsNullOrEmpty(propertyName))
        return (object) null;
      PropertyInfo propertyInfo = SFClass.GetPropertyInfo(source, propertyName);
      if (propertyInfo != (PropertyInfo) null)
      {
        try
        {
          return propertyInfo.GetValue(source);
        }
        catch
        {
        }
      }
      return (object) "";
    }

    public static T GetPropertyValue<T>(object source, string propertyName)
    {
      if (source == null || string.IsNullOrEmpty(propertyName))
        return default (T);
      PropertyInfo propertyInfo = SFClass.GetPropertyInfo(source, propertyName);
      if (propertyInfo != (PropertyInfo) null)
      {
        try
        {
          return (T) propertyInfo.GetValue(source);
        }
        catch
        {
        }
      }
      return default (T);
    }

    public static Dictionary<string, object> GetPropertyValues<T>(T source, SFClass.ObjectPropertyAccessType access)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      if ((object) source == null)
        return dictionary;
      foreach (PropertyInfo property in source.GetType().GetProperties())
      {
        try
        {
          bool flag = false;
          if (access == SFClass.ObjectPropertyAccessType.ReadWrite && property.CanRead && property.CanWrite)
            flag = true;
          if (access == SFClass.ObjectPropertyAccessType.ReadOnly && property.CanRead && !property.CanWrite)
            flag = true;
          if (flag)
          {
            string name = property.Name;
            object obj = property.GetValue((object) source, (object[]) null);
            dictionary.Add(name, obj);
          }
        }
        catch
        {
        }
      }
      return dictionary;
    }

    public static TypeConverter GetTypeConverter(PropertyInfo propInfo)
    {
      if (propInfo != (PropertyInfo) null)
      {
        TypeConverterAttribute[] customAttributes = (TypeConverterAttribute[]) propInfo.GetCustomAttributes(typeof (TypeConverterAttribute), false);
        if (customAttributes != null)
        {
          if (customAttributes.Length != 0)
          {
            try
            {
              Type type = Type.GetType(customAttributes[0].ConverterTypeName);
              return (TypeConverter) Assembly.GetAssembly(type).CreateInstance(type.FullName);
            }
            catch
            {
              goto label_6;
            }
          }
        }
        return TypeDescriptor.GetConverter(propInfo.PropertyType);
      }
label_6:
      return (TypeConverter) null;
    }

    public static bool IsBrowsable(object source, string propertyName)
    {
      if (source == null || string.IsNullOrEmpty(propertyName))
        return false;
      return SFClass.IsBrowsable(SFClass.GetPropertyInfo(source, propertyName));
    }

    public static bool IsBrowsable(PropertyInfo propInfo)
    {
      if (propInfo != (PropertyInfo) null)
      {
        BrowsableAttribute[] customAttributes = (BrowsableAttribute[]) propInfo.GetCustomAttributes(typeof (BrowsableAttribute), false);
        if (customAttributes != null && customAttributes.Length != 0)
          return customAttributes[0].Browsable;
      }
      return true;
    }

    public static string PropertyValuesToString(object source, SFClass.ObjectPropertyAccessType access, string delimiter = ":")
    {
      if (source == null)
        return "";
      Dictionary<string, string> propertyStringValues = SFClass.GetPropertyStringValues(source, access);
      if (propertyStringValues == null || propertyStringValues.Count <= 0)
        return "";
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string key in propertyStringValues.Keys)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(delimiter);
        stringBuilder.Append(key).Append(";").Append(propertyStringValues[key]);
      }
      return stringBuilder.ToString();
    }

    public static void SetObjectFromDictionary<T>(T sourceObject, Dictionary<string, string> values)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if ((object) sourceObject == null)
        return;
      foreach (PropertyInfo property in sourceObject.GetType().GetProperties())
      {
        try
        {
          string name = property.Name;
          string str;
          try
          {
            str = values[name];
          }
          catch
          {
            str = (string) null;
          }
          if (str != null)
          {
            if (property.CanWrite)
            {
              object obj = property.GetValue((object) sourceObject, (object[]) null);
              if (obj is DateTime)
              {
                DateTime date = SFStrings.ToDate(str);
                property.SetValue((object) sourceObject, (object) date);
              }
              else if (obj is bool)
              {
                if (str.ToLower() == "true" || str == "1")
                  property.SetValue((object) sourceObject, (object) true);
                else
                  property.SetValue((object) sourceObject, (object) false);
              }
              else if (!(obj is int))
              {
                if (!(obj is float))
                {
                  if (!(obj is Decimal))
                  {
                    Type type = obj.GetType();
                    property.SetValue((object) sourceObject, SFClass.ValueAs(type, str));
                  }
                }
              }
            }
          }
        }
        catch
        {
        }
      }
    }

    public static void SetObjectFromString(ref object sourceObject, char delimiter, string value)
    {
      if (string.IsNullOrEmpty(value) || sourceObject == null)
        return;
      List<string> stringList = new List<string>((IEnumerable<string>) value.Split(delimiter));
      if (stringList.Count <= 0)
        return;
      for (int index = 0; index < stringList.Count; ++index)
      {
        string[] strArray = stringList[index].Split(':');
        if (strArray.Length == 2)
          SFClass.SetPropertyValue(sourceObject, strArray[0], (object) strArray[1]);
      }
    }

    public static void SetPropertyValue(object source, string propertyName, object value)
    {
      if (source == null || value == null || string.IsNullOrEmpty(propertyName))
        return;
      PropertyInfo propertyInfo = SFClass.GetPropertyInfo(source, propertyName);
      if (!(propertyInfo != (PropertyInfo) null))
        return;
      try
      {
        if (propertyInfo.PropertyType == value.GetType())
        {
          if (value.GetType().IsGenericType)
          {
            object obj = propertyInfo.GetValue(source);
            IList list = (IList) value;
            MethodInfo method = obj.GetType().GetMethod("Add");
            obj.GetType().GetMethod("Clear").Invoke(obj, (object[]) null);
            for (int index = 0; index < list.Count; ++index)
              method.Invoke(obj, new object[1]
              {
                list[index]
              });
          }
          else
            propertyInfo.SetValue(source, value);
        }
        else
        {
          try
          {
            propertyInfo.SetValue(source, SFClass.ValueAs(propertyInfo.PropertyType, value.ToString()));
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
    }

    public static void SetPropertyValues<T>(T source, NameValueCollection values)
    {
      if ((object) source == null || values == null || values.Count == 0)
        return;
      foreach (PropertyInfo property in source.GetType().GetProperties())
      {
        try
        {
          string str = values[property.Name];
          if (str != null)
          {
            Type type = property.GetValue((object) source, (object[]) null).GetType();
            try
            {
              object obj = SFClass.ValueAs(type, str);
              property.SetValue((object) source, obj, (object[]) null);
            }
            catch
            {
            }
          }
        }
        catch
        {
        }
      }
    }

    public static void SetPropertyValues<T>(T source, Dictionary<string, string> values)
    {
      if ((object) source == null || values == null || values.Count == 0)
        return;
      foreach (PropertyInfo property in source.GetType().GetProperties())
      {
        try
        {
          string str = values[property.Name];
          if (str != null)
          {
            Type type = property.GetValue((object) source, (object[]) null).GetType();
            try
            {
              object obj = SFClass.ValueAs(type, str);
              property.SetValue((object) source, obj, (object[]) null);
            }
            catch
            {
            }
          }
        }
        catch
        {
        }
      }
    }

    public static void SetPropertyValues<T>(T source, Dictionary<string, object> values)
    {
      if ((object) source == null || values == null || values.Count == 0)
        return;
      foreach (PropertyInfo property in source.GetType().GetProperties())
      {
        try
        {
          object obj = values[property.Name];
          if (obj != null)
            property.SetValue((object) source, obj, (object[]) null);
        }
        catch
        {
        }
      }
    }

    public string GetAvailableRAM()
    {
      return ((double) new PerformanceCounter("Memory", "Available MBytes").NextValue()).ToString() + "MB";
    }

    public static List<EventInfo> AssemblyClassEvents(TypeInfo type)
    {
      try
      {
        return type.DeclaredEvents.ToList<EventInfo>();
      }
      catch
      {
      }
      return new List<EventInfo>();
    }

    public static List<MethodInfo> AssemblyClassMethods(TypeInfo type)
    {
      try
      {
        return type.DeclaredMethods.ToList<MethodInfo>();
      }
      catch
      {
      }
      return new List<MethodInfo>();
    }

    public static List<PropertyInfo> AssemblyClassProperties(TypeInfo type)
    {
      try
      {
        return type.DeclaredProperties.ToList<PropertyInfo>();
      }
      catch
      {
      }
      return new List<PropertyInfo>();
    }

    public static Assembly AssemblyInfo(string filePath)
    {
      if (!string.IsNullOrEmpty(filePath))
      {
        if (File.Exists(filePath))
        {
          try
          {
            return new ProxyDomain().GetAssembly(filePath);
          }
          catch
          {
          }
          return (Assembly) null;
        }
      }
      return (Assembly) null;
    }

    public static List<string> AssemblyNamespace(Assembly assembly)
    {
      List<string> stringList = new List<string>();
      Type[] typeArray = (Type[]) null;
      if (assembly != (Assembly) null)
      {
        try
        {
          typeArray = assembly.GetTypes();
        }
        catch
        {
        }
        if (typeArray != null)
        {
          foreach (Type type in typeArray)
          {
            string str = type.Namespace;
            if (str != null && !stringList.Contains(str))
              stringList.Add(str);
          }
        }
      }
      return stringList;
    }

    public static List<TypeInfo> AssemblyTypes(Assembly assembly)
    {
      List<TypeInfo> typeInfoList = new List<TypeInfo>();
      if (assembly != (Assembly) null)
      {
        foreach (TypeInfo definedType in assembly.DefinedTypes)
        {
          if (definedType.IsClass)
            typeInfoList.Add(definedType);
        }
      }
      return typeInfoList;
    }

    public static void CopyObject<T>(T sourceObject, T valuesObject)
    {
      if ((object) sourceObject == null || (object) valuesObject == null)
        return;
      PropertyInfo[] properties = sourceObject.GetType().GetProperties();
      valuesObject.GetType().GetProperties();
      for (int index = 0; index < properties.Length; ++index)
      {
        if (properties[index].CanWrite)
          properties[index].SetValue((object) valuesObject, SFClass.GetPropertyValue((object) sourceObject, properties[index].Name), (object[]) null);
      }
    }

    public static object ValueAs(Type type, string value)
    {
      object obj1 = (object) null;
      if (type == typeof (DateTime))
        return (object) SFStrings.ToDate(value, new DateTime(1900, 1, 1));
      if (type.IsGenericType && (object) type.GetGenericTypeDefinition() == (object) typeof (Nullable<>))
      {
        Type underlyingType = Nullable.GetUnderlyingType(type);
        Type type1 = type.GetGenericTypeDefinition().MakeGenericType(underlyingType);
        object obj2 = Convert.ChangeType((object) value, underlyingType);
        if (obj2 != null)
          obj1 = Activator.CreateInstance(type1, new object[1]
          {
            obj2
          });
      }
      else
        obj1 = Convert.ChangeType((object) value, type);
      return obj1;
    }

    public static bool IsInList<T>(T value, List<T> itemList)
    {
      if ((object) value == null || itemList == null)
        return false;
      return itemList.IndexOf(value) != -1;
    }

    public enum ObjectPropertyAccessType
    {
      ReadWrite,
      ReadOnly,
    }
  }
}
