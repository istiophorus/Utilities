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
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public sealed class NoMoreSecrets<T> : DynamicObject where T : class
	{
		private readonly T _instance;

		private readonly Type _targetType;

		public NoMoreSecrets(T instance)
		{
			if (null == instance)
			{
				throw new ArgumentNullException("instance");
			}

			_instance = instance;

			_targetType = _instance.GetType();
		}

		public override Boolean TryInvokeMember(InvokeMemberBinder binder, Object[] args, out Object result)
		{
			MethodInfo method = _targetType.GetMethod(binder.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

			if (null != method)
			{
				result = method.Invoke(_instance, args);

				return true;
			}

			result = null;

			return false;
		}

		public override Boolean TryGetMember(GetMemberBinder binder, out Object result)
		{
			FieldInfo field = _targetType.GetField(binder.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

			if (null != field)
			{
				result = field.GetValue(_instance);

				return true;
			}

			PropertyInfo property = _targetType.GetProperty(binder.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

			if (null != property)
			{
				result = property.GetValue(_instance);

				return true;
			}

			result = null;

			return false;
		}

		public override Boolean TrySetMember(SetMemberBinder binder, Object value)
		{
			FieldInfo field = _targetType.GetField(binder.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

			if (null != field)
			{
				field.SetValue(_instance, value);

				return true;
			}

			PropertyInfo property = _targetType.GetProperty(binder.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

			if (null != property)
			{
				property.SetValue(_instance, value);

				return true;
			}

			return false;
		}
	}
}
