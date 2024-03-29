﻿using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TaF_Redis.Services.MutalMethods
{
    public class HashDataParser
    {
        public static HashEntry[] ToHashEntries(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
                .Select
                (
                      property =>
                      {
                          object propertyValue = property.GetValue(obj);
                          string hashValue;
                          if (propertyValue is IEnumerable<object>)
                          {
                              hashValue = JsonConvert.SerializeObject(propertyValue);
                          }
                          else if(propertyValue is byte[])
                          {
                              hashValue = Convert.ToBase64String((byte[])propertyValue);
                          }
                          else
                          {
                              hashValue = propertyValue.ToString();
                          }

                          return new HashEntry(property.Name, hashValue);
                      }
                )
                .ToArray();
        }

        public static T ConvertFromRedis<T>(HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;

                if (property.PropertyType.Name == "Byte[]")
                    property.SetValue(obj, Convert.ChangeType(Convert.FromBase64String(entry.Value), property.PropertyType));
                else if (property.PropertyType.Name == "Guid")
                    property.SetValue(obj, Convert.ChangeType(new Guid(entry.Value.ToString()), property.PropertyType));
                else
                    property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }
    }
}
