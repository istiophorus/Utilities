#region License

/* 
   Copyright (C) 2015, Rafal Skotak & Krzysztof habrat
   All rights reserved.                          

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.
*/

/*
   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#endregion License

using System;
using System.Collections.Generic;
using System.Configuration;

namespace Utilities
{
    /// <summary>
    ///     template of the configuration parameter; author: Rafal Skotak
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigurationParameter<T>
    {
        protected T DefaultValueVar { get; set; }

        protected T MinValueVar { get; set; }

        protected T MaxValueVar { get; set; }

        protected T ValueVar { get; set; }

        protected String Key { get; set; }

        protected Boolean Limited { get; set; }

        public T DefaultValue
        {
            get
            {
                return DefaultValueVar;
            }
        }

        public T MinValue
        {
            get
            {
                return MinValueVar;
            }
        }

        public T MaxValue
        {
            get
            {
                return MaxValueVar;
            }
        }

        public T Value
        {
            get
            {
                if (EqualityComparer<T>.Default.Equals(ValueVar, default(T)))
                {
                    ValueVar = Limited
                                   ? ReadFromConfig(Key, DefaultValueVar, MinValueVar, MaxValueVar)
                                   : ReadFromConfig(Key, DefaultValueVar);
                }

                return ValueVar;
            }
        }

        public String ConfigurationKey
        {
            get
            {
                return Key;
            }
        }

        public ConfigurationParameter(String key, T defaultValue)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }

            DefaultValueVar = defaultValue;

            Key = key;

            Limited = false;
        }

        public ConfigurationParameter(String key, T defaultValue, T minValue, T maxValue)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }

            if (!typeof(IComparable).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException("Generic type must implement IComparable");
            }

            if (((IComparable)minValue).CompareTo(maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException("maxValue");
            }

            if (((IComparable)defaultValue).CompareTo(minValue) < 0)
            {
                throw new ArgumentOutOfRangeException("defaultValue");
            }

            if (((IComparable)defaultValue).CompareTo(maxValue) > 0)
            {
                throw new ArgumentOutOfRangeException("defaultValue");
            }

            MinValueVar = minValue;

            MaxValueVar = maxValue;

            DefaultValueVar = defaultValue;

            Key = key;

            Limited = true;
        }

        public void Clear()
        {
            ValueVar = default(T);
        }

        protected static T ReadFromConfig(String key, T defaultValue)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                T tempValue;

                try
                {
                    if (typeof(T).IsEnum)
                    {
                        tempValue = (T)Enum.Parse(typeof(T), ConfigurationManager.AppSettings[key], true);
                    }
                    else
                    {
                        tempValue = (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
                    }
                }
                catch
                {
                    tempValue = defaultValue;
                }

                return tempValue;
            }

            return defaultValue;
        }

        protected static T ReadFromConfig(String key, T defaultValue, T minValue, T maxValue)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                T tempValue;

                try
                {
                    if (typeof(T).IsEnum)
                    {
                        tempValue = (T)Enum.Parse(typeof(T), ConfigurationManager.AppSettings[key], true);
                    }
                    else
                    {
                        tempValue = (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
                    }

                    if (!(tempValue is IComparable))
                    {
                        throw new ArgumentException("Generic type must be IComparable");
                    }

                    if (((IComparable)tempValue).CompareTo(minValue) < 0)
                    {
                        tempValue = minValue;
                    }

                    if (((IComparable)tempValue).CompareTo(maxValue) > 0)
                    {
                        tempValue = maxValue;
                    }
                }
                catch
                {
                    tempValue = defaultValue;
                }

                return tempValue;
            }

            return defaultValue;
        }

        public static implicit operator T(ConfigurationParameter<T> input)
        {
            if (null == input)
            {
                throw new ArgumentNullException("input");
            }

            return input.Value;
        }
    }
}