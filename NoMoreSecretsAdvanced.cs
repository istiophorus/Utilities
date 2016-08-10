﻿#region License

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
using System.Dynamic;
using System.Reflection;

namespace Utilities
{
    public sealed class NoMoreSecretsAdvanced<T> : DynamicObject
    {
        private readonly T _instance;

        private readonly Type _targetType;

        public NoMoreSecretsAdvanced(T instance)
        {
            if (null == instance)
            {
                throw new ArgumentNullException("instance");
            }

            _instance = instance;

            _targetType = _instance.GetType();
        }

        public T Instance
        {
            get
            {
                return _instance;
            }
        }

        public static implicit operator T(NoMoreSecretsAdvanced<T> input)
        {
            return input._instance;
        }

        public override Boolean TryGetMember(GetMemberBinder binder, out Object result)
        {
            FieldInfo field = _targetType.GetField(binder.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (null != field)
            {
                result = field.GetValue(_instance);

                if (null != result)
                {
                    ConstructorInfo ctor = typeof(NoMoreSecretsAdvanced<>).MakeGenericType(result.GetType()).GetConstructor(new[] { result.GetType() });

                    result = ctor.Invoke(new[] { result });
                }

                return true;
            }

            PropertyInfo property = _targetType.GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (null != property)
            {
                result = property.GetValue(_instance);

                if (null != result)
                {
                    ConstructorInfo ctor = typeof(NoMoreSecretsAdvanced<>).MakeGenericType(result.GetType()).GetConstructor(new[] { result.GetType() });

                    result = ctor.Invoke(new[] { result });
                }

                return true;
            }

            result = null;

            return false;
        }

        public override Boolean TrySetMember(SetMemberBinder binder, Object value)
        {
            FieldInfo field = _targetType.GetField(binder.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (null != field)
            {
                field.SetValue(_instance, value);

                return true;
            }

            PropertyInfo property = _targetType.GetProperty(binder.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (null != property)
            {
                property.SetValue(_instance, value);

                return true;
            }

            return false;
        }
    }
}