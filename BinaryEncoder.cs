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
using System.IO;

namespace Utilities
{
    public sealed class BinaryEncoder
    {
        private readonly Byte[] _bytes = new Byte[9];

        public Int32 ReadInt32Decoded(BinaryReader reader)
        {
            return (Int32)ReadUInt32Decoded(reader);
        }

        public UInt32 ReadUInt32Decoded(BinaryReader reader)
        {
            Byte firstByte = reader.ReadByte();
            
            // single byte
            if ((firstByte & 0x80) == 0) 
            {
                return firstByte;
            }
            
            if ((firstByte & 0xC0) == 0x80)
            {
                Byte secondByte = reader.ReadByte();

                return (UInt32)(((firstByte & 0x7F) << 8) | secondByte);
            }
            
            if ((firstByte & 0xE0) == 0xC0)
            {
                Byte byte2 = reader.ReadByte();
                Byte byte3 = reader.ReadByte();

                return (UInt32)(((firstByte & 0x1F) << 16) |
                                (byte2 << 8) |
                                byte3);
            }
            
            if ((firstByte & 0xF0) == 0xE0)
            {
                Byte byte2 = reader.ReadByte();
                Byte byte3 = reader.ReadByte();
                Byte byte4 = reader.ReadByte();

                return (UInt32)(((firstByte & 0x0F) << 24) |
                                (byte2 << 16) |
                                (byte3 << 8) |
                                byte4);
            }
            
            if (firstByte == 0xF0)
            {
                return reader.ReadUInt32();
            }
            
            throw new InvalidDataException("Can not recognize value code " + firstByte);
        }

        public Int32 WriteInt32Encoded(BinaryWriter writer, Int32 value)
        {
            return WriteUInt32Encoded(writer, (UInt32)value);
        }

        public Int32 WriteUInt32Encoded(BinaryWriter writer, UInt32 value)
        {
            if (value < 0x80)
            {
                writer.Write((Byte)value);

                return 1;
            }
            
            if (value < 0x4000)
            {
                var valueToWrite = (UInt16)value;

                valueToWrite |= 0x8000;

                _bytes[0] = (Byte)((valueToWrite >> 8) & 0xFF);
                _bytes[1] = (Byte)(valueToWrite & 0xFF);

                writer.Write(_bytes, 0, 2);

                return 2;
            }
            
            if (value < 0x200000)
            {
                var valueToWritePartB = (UInt16)(value & 0xFFFF);

                var valueToWritePartA = (Byte)(((value >> 16) & 0x1F) | 0xC0);

                _bytes[0] = valueToWritePartA;
                _bytes[1] = (Byte)((valueToWritePartB >> 8) & 0xFF);
                _bytes[2] = (Byte)(valueToWritePartB & 0xFF);

                writer.Write(_bytes, 0, 3);

                return 3;
            }
            
            if (value < 0x10000000)
            {
                _bytes[0] = (Byte)(((value >> 24) & 0xFF) | 0xE0);
                _bytes[1] = (Byte)((value >> 16) & 0xFF);
                _bytes[2] = (Byte)((value >> 8) & 0xFF);
                _bytes[3] = (Byte)(value & 0xFF);

                writer.Write(_bytes, 0, 4);

                return 4;
            }
            
            writer.Write((Byte)0xF0);
            writer.Write(value);

            return 5;
        }

        public Int32 WriteInt64Encoded(BinaryWriter writer, Int64 value)
        {
            return WriteUInt64Encoded(writer, (UInt64)value);
        }

        public Int32 WriteUInt64Encoded(BinaryWriter writer, UInt64 value)
        {
            // 0
            if (value < 0x80) 
            {
                writer.Write((Byte)value);

                return 1;
            }
            
            // 10
            if (value < 0x4000) 
            {
                var valueToWrite = (UInt16)value;

                valueToWrite |= 0x8000;

                _bytes[0] = (Byte)((valueToWrite >> 8) & 0xFF);
                _bytes[1] = (Byte)(valueToWrite & 0xFF);

                writer.Write(_bytes, 0, 2);

                return 2;
            }
            
            // 110
            if (value < 0x200000)
            {
                var valueToWritePartB = (UInt16)(value & 0xFFFF);

                var valueToWritePartA = (Byte)(((value >> 16) & 0x1F) | 0xC0);

                _bytes[0] = valueToWritePartA;
                _bytes[1] = (Byte)((valueToWritePartB >> 8) & 0xFF);
                _bytes[2] = (Byte)(valueToWritePartB & 0xFF);

                writer.Write(_bytes, 0, 3);

                return 3;
            }

            // 1110
            if (value < 0x10000000)
            {
                _bytes[0] = (Byte)(((value >> 24) & 0xFF) | 0xE0);
                _bytes[1] = (Byte)((value >> 16) & 0xFF);
                _bytes[2] = (Byte)((value >> 8) & 0xFF);
                _bytes[3] = (Byte)(value & 0xFF);

                writer.Write(_bytes, 0, 4);

                return 4;
            }

            // 11110
            if (value < 0x0800000000)
            {
                _bytes[0] = (Byte)(((value >> 32) & 0xFF) | 0xF0);
                _bytes[1] = (Byte)((value >> 24) & 0xFF);
                _bytes[2] = (Byte)((value >> 16) & 0xFF);
                _bytes[3] = (Byte)((value >> 8) & 0xFF);
                _bytes[4] = (Byte)(value & 0xFF);

                writer.Write(_bytes, 0, 5);

                return 5;
            }

            // 111110
            if (value < 0x040000000000)
            {
                _bytes[0] = (Byte)(((value >> 40) & 0xFF) | 0xF8);
                _bytes[1] = (Byte)((value >> 32) & 0xFF);
                _bytes[2] = (Byte)((value >> 24) & 0xFF);
                _bytes[3] = (Byte)((value >> 16) & 0xFF);
                _bytes[4] = (Byte)((value >> 8) & 0xFF);
                _bytes[5] = (Byte)(value & 0xFF);

                writer.Write(_bytes, 0, 6);

                return 6;
            }

            // 1111110
            if (value < 0x02000000000000)
            {
                _bytes[0] = (Byte)(((value >> 48) & 0xFF) | 0xFC);
                _bytes[1] = (Byte)((value >> 40) & 0xFF);
                _bytes[2] = (Byte)((value >> 32) & 0xFF);
                _bytes[3] = (Byte)((value >> 24) & 0xFF);
                _bytes[4] = (Byte)((value >> 16) & 0xFF);
                _bytes[5] = (Byte)((value >> 8) & 0xFF);
                _bytes[6] = (Byte)(value & 0xFF);

                writer.Write(_bytes, 0, 7);

                return 7;
            }

            // 11111110
            if (value < 0x0100000000000000)
            {
                _bytes[0] = (Byte)(((value >> 56) & 0xFF) | 0xFE);
                _bytes[1] = (Byte)((value >> 48) & 0xFF);
                _bytes[2] = (Byte)((value >> 40) & 0xFF);
                _bytes[3] = (Byte)((value >> 32) & 0xFF);
                _bytes[4] = (Byte)((value >> 24) & 0xFF);
                _bytes[5] = (Byte)((value >> 16) & 0xFF);
                _bytes[6] = (Byte)((value >> 8) & 0xFF);
                _bytes[7] = (Byte)(value & 0xFF);

                writer.Write(_bytes, 0, 8);

                return 8;
            }
            
            writer.Write((Byte)0xFF);
            writer.Write(value);

            return 9;
        }

        public UInt64 ReadUInt64Decoded(BinaryReader reader)
        {
            Byte byte0 = reader.ReadByte();

            // 0   single byte 
            if ((byte0 & 0x80) == 0)
            {
                return byte0;
            }

            // 10
            if ((byte0 & 0xC0) == 0x80)
            {
                Byte byte1 = reader.ReadByte();

                return ((byte0 & 0x7FUL) << 8) | byte1;
            }

            // 110
            if ((byte0 & 0xE0) == 0xC0)
            {
                Byte byte1 = reader.ReadByte();
                Byte byte2 = reader.ReadByte();

                return (byte0 & 0x1FUL) << 16 |
                       ((UInt64)byte1 << 8) |
                       byte2;
            }

            // 1110
            if ((byte0 & 0xF0) == 0xE0)
            {
                Byte byte1 = reader.ReadByte();
                Byte byte2 = reader.ReadByte();
                Byte byte3 = reader.ReadByte();

                return ((byte0 & 0x0FUL) << 24) |
                       ((UInt64)byte1 << 16) |
                       ((UInt64)byte2 << 8) |
                       byte3;
            }

            // 11110
            if ((byte0 & 0xF8) == 0xF0)
            {
                Byte byte1 = reader.ReadByte();
                Byte byte2 = reader.ReadByte();
                Byte byte3 = reader.ReadByte();
                Byte byte4 = reader.ReadByte();

                return ((byte0 & 0x07UL) << 32) |
                       ((UInt64)byte1 << 24) |
                       ((UInt64)byte2 << 16) |
                       ((UInt64)byte3 << 8) |
                       byte4;
            }

            // 111110
            if ((byte0 & 0xFC) == 0xF8)
            {
                Byte byte1 = reader.ReadByte();
                Byte byte2 = reader.ReadByte();
                Byte byte3 = reader.ReadByte();
                Byte byte4 = reader.ReadByte();
                Byte byte5 = reader.ReadByte();

                return ((byte0 & 0x03UL) << 40) |
                       ((UInt64)byte1 << 32) |
                       ((UInt64)byte2 << 24) |
                       ((UInt64)byte3 << 16) |
                       ((UInt64)byte4 << 8) |
                       byte5;
            }

            // 1111110
            if ((byte0 & 0xFE) == 0xFC)
            {
                Byte byte1 = reader.ReadByte();
                Byte byte2 = reader.ReadByte();
                Byte byte3 = reader.ReadByte();
                Byte byte4 = reader.ReadByte();
                Byte byte5 = reader.ReadByte();
                Byte byte6 = reader.ReadByte();

                return ((byte0 & 0x03UL) << 48) |
                       ((UInt64)byte1 << 40) |
                       ((UInt64)byte2 << 32) |
                       ((UInt64)byte3 << 24) |
                       ((UInt64)byte4 << 16) |
                       ((UInt64)byte5 << 8) |
                       byte6;
            }
            
            if ((byte0 & 0xFF) == 0xFE)
            {
                Byte byte1 = reader.ReadByte();
                Byte byte2 = reader.ReadByte();
                Byte byte3 = reader.ReadByte();
                Byte byte4 = reader.ReadByte();
                Byte byte5 = reader.ReadByte();
                Byte byte6 = reader.ReadByte();
                Byte byte7 = reader.ReadByte();

                return ((byte0 & 0x01UL) << 56) |
                       ((UInt64)byte1 << 48) |
                       ((UInt64)byte2 << 40) |
                       ((UInt64)byte3 << 32) |
                       ((UInt64)byte4 << 24) |
                       ((UInt64)byte5 << 16) |
                       ((UInt64)byte6 << 8) |
                       byte7;
            }
            
            if ((byte0 & 0xFF) == 0xFF)
            {
                return reader.ReadUInt64();
            }
            
            throw new InvalidDataException("Can not recognize value code " + byte0);
        }

        public Int64 ReadInt64Decoded(BinaryReader reader)
        {
            return (Int64)ReadUInt64Decoded(reader);
        }
    }
}