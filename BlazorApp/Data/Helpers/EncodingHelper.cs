using System;
using System.IO;
using System.Text;
using Ude; // pour CharsetDetector

namespace BlazorApp.Data.Helpers
{
    public static class EncodingHelper
    {
        public static Encoding DetectEncoding(byte[] buffer)
        {
            // Détecter l'encodage
            var detector = new CharsetDetector();
            detector.Feed(buffer, 0, buffer.Length);
            detector.DataEnd();

            string encodingName = detector.Charset ?? "utf-8";

            Console.WriteLine($"Encoding = {encodingName}");

            return Encoding.GetEncoding(encodingName);
        }
    }
    
}
