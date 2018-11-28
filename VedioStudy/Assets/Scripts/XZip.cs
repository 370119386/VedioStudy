using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

public class XZip
{
    public static byte[] CompressZip(byte[] rawData)
    {
        MemoryStream ms = new MemoryStream();
        GZipOutputStream compressedzipStream = new GZipOutputStream(ms);
        compressedzipStream.Write(rawData, 0, rawData.Length);
        compressedzipStream.Close();
        return ms.ToArray();
    }

    public static byte[] DecompressZip(byte[] byteArray)
    {
        GZipInputStream gzi = new GZipInputStream(new MemoryStream(byteArray));
        MemoryStream re = new MemoryStream(50000);
        int count;
        byte[] data = new byte[50000];
        while ((count = gzi.Read(data, 0, data.Length)) != 0)
        {
            re.Write(data, 0, count);
        }
        byte[] overarr = re.ToArray();

        return overarr;
    }
}