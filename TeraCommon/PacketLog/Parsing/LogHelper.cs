﻿// Copyright (c) Gothos
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;

namespace Tera.PacketLog
{
    internal class LogHelper
    {
        private static readonly DateTime TimeOrigin = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static readonly Encoding Encoding = new UTF8Encoding(false, true);
        public static readonly string MagicBytes = "TeraConnectionLog";

        public static byte[] DateTimeToBytes(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException();

            var value = (long) Math.Round((dateTime - TimeOrigin).TotalMilliseconds);

            var byteCount = 0;
            while (value >> byteCount*8 != 0)
            {
                byteCount++;
            }

            var result = new byte[byteCount];
            for (var i = 0; i < byteCount; i++)
            {
                result[i] = unchecked((byte) (value >> i*8));
            }
            return result;
        }

        public static DateTime BytesToTimeSpan(byte[] data)
        {
            ulong value = 0;
            for (var i = 0; i < data.Length; i++)
            {
                value |= (ulong) data[i] << i*8;
            }
            return TimeOrigin + TimeSpan.FromMilliseconds(value);
        }
    }
}