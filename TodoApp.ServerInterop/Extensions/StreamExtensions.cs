using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.ServerInterop.Extensions
{
    public static class StreamExtensions
    {
        public static string ReadString(this Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        public static async Task<string> ReadStringAsync(this Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            return await streamReader.ReadToEndAsync();
        }

        public static Memory<byte> ReadBytes(this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static async Task<Memory<byte>> ReadBytesAsync(this Stream stream)
        {
            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
