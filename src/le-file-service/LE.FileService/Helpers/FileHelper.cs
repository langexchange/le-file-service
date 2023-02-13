using System;
using System.Collections.Generic;

namespace LE.FileService.Helpers
{
    public static class FileHelper
    {
        public static string GenFullPath(int id)
        {
            var prefix = $"{GenerateIdPartition(id)}";
            var fullPath = $"{prefix}/{DateTime.UtcNow.ToString("yyyyMMddhhmmss")}";
            return fullPath;
        }

        public static string GenerateIdPartition(int id)
        {
            var idPartition = id.ToString("D9");
            return string.Join("/", SplitString(idPartition.ToString(), 3));
        }
        public static IEnumerable<string> SplitString(string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Split value must be positive.");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }
}
