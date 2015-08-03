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
using System.Configuration;

namespace Utilities
{
    /// <summary>
    ///     template of the writable configuration parameter; author: Rafal Skotak
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WritableConfigurationParameter<T> : ConfigurationParameter<T>
    {
        public WritableConfigurationParameter(String key, T defaultValue)
            : base(key, defaultValue)
        {
        }

        public WritableConfigurationParameter(String key, T defaultValue, T minValue, T maxValue)
            : base(key, defaultValue, minValue, maxValue)
        {
        }

        public new T Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                if (Limited)
                {
                    if (!(value is IComparable))
                    {
                        throw new ArgumentException("Generic type must be IComparable");
                    }

                    if (((IComparable)value).CompareTo(MinValueVar) < 0)
                    {
                        value = MinValueVar;
                    }

                    if (((IComparable)value).CompareTo(MaxValueVar) > 0)
                    {
                        value = MaxValueVar;
                    }
                }

                WriteToConfig(ConfigurationKey, value);

                ValueVar = value;
            }
        }

        private static void WriteToConfig(String key, T value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (null != config.AppSettings.Settings[key])
            {
                config.AppSettings.Settings.Remove(key);
            }

            config.AppSettings.Settings.Add(key, value.ToString());

            config.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}