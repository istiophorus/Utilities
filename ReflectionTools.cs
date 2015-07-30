#region License

/* 
   Copyright (C) 2015, Rafal Skotak
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
using System.Reflection;

namespace Utilities
{
    public static class ReflectionTools
    {
        public static void SetPrivateField(Type type, Object target, String name, Object value)
        {
            FieldInfo fi;

            if (null != target)
            {
                fi = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {
                fi = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (null != fi)
            {
                fi.SetValue(target, value);

                return;
            }

            PropertyInfo property;

            if (null != target)
            {
                property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {
                property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (null != property)
            {
                property.SetValue(target, value);

                return;
            }

            throw new TargetException("Field or property of the specified name has not been found " + name);
        }

        public static Object GetPrivateField(Type type, Object target, String name)
        {
            if (null == type)
            {
                throw new ArgumentNullException("type");
            }

            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            FieldInfo field;

            if (null != target)
            {
                field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {
                field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (null != field)
            {
                return field.GetValue(target);    
            }

            PropertyInfo property;

            if (null != target)
            {
                property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {
                property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (null != property)
            {
                return property.GetValue(target);
            }

            throw new TargetException("Field or property of the specified name has not been found " + name);
        }

        public static void SetPrivateFieldExt(this Object target, String name, Object value)
        {
            SetPrivateField(target.GetType(), target, name, value);
        }

        public static T GetPrivateFieldExt<T>(this Object target, String name)
        {
            return (T)GetPrivateField(target.GetType(), target, name);
        }
    }
}
