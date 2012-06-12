using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SevenZip;

namespace ZeroDayExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ThrowsException Zero Day Extractor");
            Console.Write("Directory: ");
            
            string directory = Console.ReadLine();
            IEnumerable<string> bookFolders = Directory.EnumerateDirectories(directory);
            string outputDirectory = @"D:\Books\TestFolder";
            SevenZipExtractor.SetLibraryPath(@"D:\Code\ZeroDayExtractor\ZeroDayExtractor\7z64.dll");
            int i = 1;
            foreach (string currentDirectory in bookFolders)
            {
                int lastPart = currentDirectory.LastIndexOf("\\")+1 ;
                int length = currentDirectory.Length - lastPart;
                string finalFileName = (currentDirectory).Substring(lastPart,length).Replace(".", " ").Trim();
                Console.WriteLine("{0} {1}", i, finalFileName);
                
                IEnumerable<string> archiveFiles = Directory.EnumerateFiles(currentDirectory,"*.zip");
                foreach(string fileName in archiveFiles)
                {
                    try
                    {
                        using (var tmp = new SevenZipExtractor(fileName))
                        {
                            tmp.ExtractArchive(currentDirectory);
                        }
                    }
                    catch (SevenZipArchiveException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (ExtractionFailedException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                IEnumerable<string> rarFiles = Directory.EnumerateFiles(currentDirectory, "*.rar");
                foreach (string fileName in rarFiles)
                {
                    try
                    {
                        using (var tmp = new SevenZipExtractor(fileName))
                        {
                            tmp.ExtractArchive(currentDirectory);
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(String.Format("{0} was caught. No archives found in this folder", e.Message));
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(String.Format("{0} was caught. This archive is missing necessary files to unpack", e.Message));
                    }
                    catch (SevenZipArchiveException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                string file = Directory.EnumerateFiles(currentDirectory, "*.pdf").FirstOrDefault();
                string newPath =  outputDirectory+"\\"+finalFileName+".pdf";
                if (!String.IsNullOrEmpty(file) && !File.Exists(newPath))
                    File.Copy(file, newPath);
                
                i++;
            }
          
            Console.WriteLine("Done. Press Enter to Continue");
            Console.ReadLine();
            
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}
