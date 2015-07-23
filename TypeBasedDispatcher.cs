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
using System.Collections.Generic;

namespace Utilities
{
    public class TypeBasedDispatcher<TRes>
    {
        private readonly Dictionary<Type, Func<Object, TRes>> _actions = new Dictionary<Type, Func<Object, TRes>>();

        public void Register<TArg>(Func<TArg, TRes> action)
        {
            if (null == action)
            {
                throw new ArgumentNullException("action");
            }

            Type parameterType = typeof(TArg);

            if (_actions.ContainsKey(parameterType))
            {
                throw new ArgumentException("Action for type is already registered " + parameterType.AssemblyQualifiedName);
            }

            _actions.Add(parameterType, 
                x =>
                {
                    TArg arg = default(TArg);

                    if (null != x)
                    {
                        arg = (TArg)x;
                    }

                    return action(arg);
                });
        }

        public TRes DispatchForType(Type targetType)
        {
            if (null == targetType)
            {
                throw new ArgumentNullException("targetType");
            }

            Func<Object, TRes> action;

            if (_actions.TryGetValue(targetType, out action))
            {
                return action(null);
            }

            throw new KeyNotFoundException(targetType.AssemblyQualifiedName);
        }

        public TRes DispatchForType<TArg>()
        {
            return DispatchForType(typeof(TArg));
        }

        public TRes Dispatch(Object arg)
        {
            if (null == arg)
            {
                throw new ArgumentNullException("arg");
            }

            Type parameterType = arg.GetType();

            Func<Object, TRes> action;

            if (_actions.TryGetValue(parameterType, out action))
            {
                return action(arg);
            }
            
            throw new KeyNotFoundException(parameterType.AssemblyQualifiedName);
        }
    }
}