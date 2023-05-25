using Utilities;
using System.IO.Compression;

////////////////////////////////////////////////////////////////////////////
// BYTE COMPRESSION TESTING
////////////////////////////////////////////////////////////////////////////
//string fname = "ZT333_BIG_GUY.psl";
string fname = "PC_ZTP333-08_GaugeBlock_3IN_v11_2022-09-16 12-24-11.psl";
string fPath =@"C:\ZeroTouchMServer\PointClouds\";
string OutPath =@"C:\ZeroTouchMServer\PointClouds\";
string OutName ="Temp.br";
string UnCompressedName ="Uncompressed.psl";
var bytes = System.IO.File.ReadAllBytes(fPath + fname); // MAKE SURE FILE EXISTS
Console.WriteLine("INPUT FILE IS " + bytes.Length + " bytes");

try
{   //GZIP STUFF
    //CompressThis_GZIP(fPath + fname, OutPath + OutName );
    //CompressThis_GZIP_BETER_WAY(fPath + fname, OutPath + OutName);
    //DeCompressThis_GZIP_M1(OutPath + OutName, fPath + UnCompressedName);

    //BROTLI STUFF
    //CompressThis_BROTLI(fPath + fname, OutPath + OutName);
    //CompressThis_BROTLI_M1(fPath + fname, OutPath + OutName );
    //DeCompressThis_BROTLI_M1(OutPath + OutName, fPath + UnCompressedName);
}
catch(System.Exception ex)
{
   Console.WriteLine("EXCEPTION THROWN: " + ex.ToString()); 
}

#region BROTLI FUNCTIONALITY

///////////////////////////////////////////////
//LOOKS LIKE BROTLI HAS A 5 MB MAX LIMIT 
///////////////////////////////////////////////
 void  CompressThis_BROTLI (string inFile, string compressedFileName)
 {
    DateTime TimeStart = DateTime.Now;
   
    using (FileStream originalStream = File.Open(inFile, FileMode.Open))
    {
        using (FileStream compressedStream = File.Create(compressedFileName))
		{
            using (BrotliStream compressor = new BrotliStream(compressedStream, CompressionLevel.Fastest))
			{
                
                originalStream.CopyTo(compressor);

                DateTime TimeDone = DateTime.Now;
                double msecDuration = (TimeDone - TimeStart).TotalMilliseconds;
                FileInfo fi_source = new FileInfo(inFile);
                FileInfo fi_compressed = new FileInfo(compressedFileName);
                double compressionPercentage = 100*((double)fi_compressed.Length)/((double)fi_source.Length);
                Console.WriteLine("BROTLI COMPRESSION DURATION: " + msecDuration.ToString() + " milliseconds , SourceFile:[ " 
                                    + originalStream.Length.ToString() + " ] DestFile: [" + fi_compressed.Length.ToString() 
                                    +  "] Compressed to: " + compressionPercentage.ToString("f2")  + "%");
			}
        }
    }
 }

//////////////////////////////////////////////////////////////////////////////////////
// COMPRESS USING BROTLI DECOMPRESSION - SIMPLE BYTE BASED
//////////////////////////////////////////////////////////////////////////////////////
void  CompressThis_BROTLI_M1 (string sourceFile, string compressedFileName)
 {
   
    var bytes = System.IO.File.ReadAllBytes(sourceFile);

    DateTime TimeStart = DateTime.Now;
    var compressedbytes = Utilities.Brotli.CompressBytes(bytes);
    DateTime TimeDone = DateTime.Now;

    System.IO.File.WriteAllBytes(compressedFileName, compressedbytes);

    double msecDuration = (TimeDone - TimeStart).TotalMilliseconds;
    FileInfo fi_source = new FileInfo(sourceFile);
    FileInfo fi_compressed = new FileInfo(compressedFileName);
    double compressionPercentage = 100*((double)fi_compressed.Length)/((double)fi_source.Length);
    Console.WriteLine("BROTLI_M2 COMPRESSION DURATION: " + msecDuration.ToString() + " milliseconds , SourceFile:[ " 
                                    + fi_source.Length.ToString() + " ] DestFile: [" + fi_compressed.Length.ToString() 
                                    +  "] Compressed to: " + compressionPercentage.ToString("f2")  + "%");
 }

//////////////////////////////////////////////////////////////////////////////////////
// DECOMPRESS USING BROTLI DECOMPRESSION
//////////////////////////////////////////////////////////////////////////////////////
void  DeCompressThis_BROTLI_M1 (string compressedFileName, string decompressedFilename)
 {
    var bytes = System.IO.File.ReadAllBytes(compressedFileName);
    DateTime TimeStart = DateTime.Now;
    byte[] UnCompressedBytes = Utilities.Brotli.DecompressBytes(bytes);
    DateTime TimeDone = DateTime.Now;

    System.IO.File.WriteAllBytes(decompressedFilename, UnCompressedBytes);
    double msecDuration = (TimeDone - TimeStart).TotalMilliseconds;
    FileInfo fi_Expanded = new FileInfo(decompressedFilename);
    FileInfo fi_compressed = new FileInfo(compressedFileName);
    Console.WriteLine("BROTLI_M2 DECOMPRESSION DURATION: " + msecDuration.ToString() + " milliseconds , CompressedFile:[ " 
                                    + fi_compressed.Length.ToString() + " ] UnCompressedFile: [" + fi_Expanded.Length.ToString() +  "]");
 }

#endregion

#region GZIP FUNCTIONALITY

///////////////////////////////////////////////////////////////////
// DUMB WAY OF DOING THIS
///////////////////////////////////////////////////////////////////
void CompressThis_GZIP (string inFile, string compressedFileName)
 {
   
    byte[] buffer = System.IO.File.ReadAllBytes(inFile);                // MAKE SURE FILE EXISTS
    Console.WriteLine("INPUT FILE IS " + bytes.Length + " bytes");

    FileStream destinationFile = File.Create(compressedFileName);

    DateTime TimeStart = DateTime.Now;
    
    using (GZipStream output = new GZipStream(destinationFile, CompressionMode.Compress))
    {
        output.Write(buffer, 0, buffer.Length);

        DateTime TimeDone = DateTime.Now;
        double msecDuration = (TimeDone - TimeStart).TotalMilliseconds;

        FileInfo fout = new FileInfo(compressedFileName);
        FileInfo fin = new FileInfo(inFile);

        double compressionPercentage = 100*((double)fout.Length)/((double)fin.Length);

        Console.WriteLine("GZIP COMPRESSION DURATION: " + msecDuration.ToString() + " milliseconds , SourceFile:[ " 
                                    + fin.Length.ToString() + " ] DestFile: [" + fout.Length.ToString() 
                                    +  "] Compressed to: " + compressionPercentage.ToString("f2") + "%");

    }
    // Close the files.
    destinationFile.Close();
  }

///////////////////////////////////////////////////////////////////
// A BETTER WAY OF DOING THIS - STREAM IT
///////////////////////////////////////////////////////////////////
void CompressThis_GZIP_BETTER_WAY (string sourceFile, string compressedFileName)
 {
     var bytes = System.IO.File.ReadAllBytes(sourceFile);
    DateTime TimeStart = DateTime.Now;
    byte[] CompressedBytes = GZIP_Compress(bytes);
    DateTime TimeDone = DateTime.Now;

    System.IO.File.WriteAllBytes(compressedFileName, CompressedBytes);
    double msecDuration = (TimeDone - TimeStart).TotalMilliseconds;
    FileInfo fi_source = new FileInfo(sourceFile);
    FileInfo fi_compressed = new FileInfo(compressedFileName);
    double compressionPercentage = 100*((double)fi_compressed.Length)/((double)fi_source.Length);

    Console.WriteLine("GZIP COMPRESSION DURATION: " + msecDuration.ToString() + " milliseconds , SourceFile:[ " 
                                    + fi_source.Length.ToString() + " ] DestFile: [" + fi_compressed.Length.ToString() 
                                    +  "] Compressed to: " + compressionPercentage.ToString("f2") + "%");


      

    //DateTime TimeStart = DateTime.Now;
//
  //  using (FileStream sourceFile = File.OpenRead(inFile))
    //using (FileStream destinationFile = File.Create(compressedFileName))
   // using (GZipStream output = new GZipStream(destinationFile, CompressionMode.Compress))
    //{
    //    sourceFile.CopyTo(output);

     //   DateTime TimeDone = DateTime.Now;
     //   double msecDuration = (TimeDone - TimeStart).TotalMilliseconds;
     //   FileInfo fcompressed = new FileInfo(compressedFileName);
     //   FileInfo foriginal = new FileInfo(inFile);
//
  //      double compressionPercentage = 100*((double)fcompressed.Length)/((double)sourceFile.Length);
    //    Console.WriteLine("GZIP COMPRESSION DURATION: " + msecDuration.ToString() + " milliseconds , SourceFile:[ " 
      //                              + foriginal.Length.ToString() + " ] DestFile: [" + fcompressed.Length.ToString() 
        //                            +  "] Compressed to: " + compressionPercentage.ToString("f2")  + "%");
    //}






 }

 void  DeCompressThis_GZIP(string compressedFileName, string decompressedFilename)
 {
    var bytes = System.IO.File.ReadAllBytes(compressedFileName);
    DateTime TimeStart = DateTime.Now;
    byte[] UnCompressedBytes = GZIP_Decompress(bytes);
    DateTime TimeDone = DateTime.Now;

    System.IO.File.WriteAllBytes(decompressedFilename, UnCompressedBytes);
    double msecDuration = (TimeDone - TimeStart).TotalMilliseconds;
    FileInfo fi_Expanded = new FileInfo(decompressedFilename);
    FileInfo fi_compressed = new FileInfo(compressedFileName);
    Console.WriteLine("BROTLI_M2 DECOMPRESSION DURATION: " + msecDuration.ToString() + " milliseconds , CompressedFile:[ " 
                                    + fi_compressed.Length.ToString() + " ] UnCompressedFile: [" + fi_Expanded.Length.ToString() +  "]");

 }


/////////////////////////////////////
// BASE LEVEL GZIP COMPRESSION
/////////////////////////////////////
byte[] GZIP_Compress(byte[] data)
{
    using (var compressedStream = new MemoryStream())
    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
    {
        zipStream.Write(data, 0, data.Length);
        zipStream.Close();
        return compressedStream.ToArray();
    }
}

/////////////////////////////////////
// BASE LEVEL GZIP DECOMPRESSION
/////////////////////////////////////
byte[] GZIP_Decompress(byte[] data)
{
    using (var compressedStream = new MemoryStream(data))
    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
    using (var resultStream = new MemoryStream())
    {
        zipStream.CopyTo(resultStream);
        return resultStream.ToArray();
    }
}

 #endregion