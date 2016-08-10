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
using System.Xml;

namespace Utilities
{
    public sealed class DynamicXmlWrapper : DynamicObject
    {
        private readonly XmlNode _node;

        private readonly Boolean _returnNullForMissing;

        private static XmlDocument Prepare(String xmlText)
        {
            if (null == xmlText)
            {
                throw new ArgumentNullException("xmlText");
            }

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(xmlText);

            return xmlDoc;
        }

        public DynamicXmlWrapper(String xmlText, Boolean returnNullForMissing)
            : this(Prepare(xmlText), returnNullForMissing)
        {
        }

        public DynamicXmlWrapper(XmlNode node, Boolean returnNullForMissing)
        {
            if (null == node)
            {
                throw new ArgumentNullException("node");                
            }

            _node = node;

            _returnNullForMissing = returnNullForMissing;
        }

        public String this[String attributeName]
        {
            get
            {
                if (null == attributeName)
                {
                    throw new ArgumentNullException("attributeName");
                }

                XmlAttribute attr = _node.Attributes[attributeName];

                if (null != attr)
                {
                    return attr.Value;
                }

                return null;
            }
        }

        public static implicit operator String(DynamicXmlWrapper input)
        {
            return input._node.InnerText;
        }

        public override Boolean TryGetMember(GetMemberBinder binder, out Object result)
        {
            result = null;

            XmlNodeList nodes = _node.SelectNodes("./" + binder.Name);

            if (null == nodes)
            {
                if (_returnNullForMissing)
                {
                    return true;
                }
                else
                {
                    return false;                    
                }
            }

            if (nodes.Count <= 0)
            {
                XmlAttribute attr = _node.Attributes[binder.Name];

                if (null != attr)
                {
                    result = attr.Value;

                    if (null != result)
                    {
                        return true;
                    }
                }

                if (_returnNullForMissing)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (nodes.Count == 1)
            {
                result = new DynamicXmlWrapper(nodes[0], _returnNullForMissing);

                return true;
            }

            List<DynamicXmlWrapper> items = new List<DynamicXmlWrapper>(nodes.Count);
            
            items.AddRange(from XmlNode node in nodes select new DynamicXmlWrapper(node, _returnNullForMissing));

            result = items.ToArray();

            return true;
        }

        public override Boolean TrySetMember(SetMemberBinder binder, Object value)
        {
            return false;
        }
    }
}
