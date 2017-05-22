using System;
using System.Collections.Generic;
using System.Drawing;
using ZXing;

namespace HW.FileCollectorService.Collector.Services
{
    public class BarcodeScanner
    {
        public static string GetBarcodeIfExists(string file)
        {
            var reader = new BarcodeReader { AutoRotate = true };
            using (var bmp = (Bitmap)Bitmap.FromFile(file))
            {
                var result = reader.Decode(bmp);
                bmp.Dispose();

                return result != null && result.Text.StartsWith("SCAN") ? result.Text : null;
            }
        }
    }
}
