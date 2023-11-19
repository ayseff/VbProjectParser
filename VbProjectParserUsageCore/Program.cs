using DocumentFormat.OpenXml.Packaging;
using OpenMcdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.OpenXml;
using VbProjectParserCore.Data;

namespace VbProjectParserUsageCore;

class Program
{
    static void Main(string[] args)
    {
        bool run = true;
        while (run)
        {
            Console.WriteLine("Select one of the following actions:");
            Console.WriteLine("\t1 - Display test.vbaProject.bin contents");
            Console.WriteLine("\t2 - Read and display vbProject of a local document file on your disk");
            Console.WriteLine("\t3 - Replace the vbProject of a local document file with contents from a local .bin file");
            Console.WriteLine("\t4 or any other key - exit");
            Console.WriteLine();
            Console.Write("Your choice: ");

            char choice = Console.ReadKey().KeyChar;
            Console.WriteLine();

            switch (choice)
            {
                case '1':
                    DisplayBinFile();
                    break;
                case '2':
                    DisplayLocalDocumentFile();
                    break;
                case '3':
                    ReplaceVbaParts();
                    break;
                default:
                    run = false;
                    break;
            }
        }

        Console.WriteLine("Done. The next keypress will close this window.");
        Console.ReadKey();
    }

    private static void DisplayBinFile()
    {
        ReadBinFile(@"D:\_Data\Downloads\seff.normal\word\vbaProject.bin"); // "test.vbaProject.bin");
    }

    private static void DisplayLocalDocumentFile()
    {
        string path = null;
        while (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            Console.WriteLine("Enter full path to file: ");
            path = Console.ReadLine();

            if (!File.Exists(path))
                Console.WriteLine($"File '{path}' not found");
            else
                Console.WriteLine();
        }

        ReadDocumentFile(path);
    }

    private static void ReadBinFile(string path)
    {
        using (var VbaStorage = new VbaStorage(path))
        {
            PrintStorage(VbaStorage);
        }
    }

    private static void ReadDocumentFile(string path)
    {
        using (var storage = new VbProject(path))
        {
            PrintStorage(storage);
        }
    }

    /// <summary>
    /// This function doesn't have anything to do specifically with this library, but is something that can be done
    /// simply with OpenXML
    /// </summary>
    private static void ReplaceVbaParts()
    {
        Console.WriteLine("Replace an existing vbProject of a document file with another vbProject");
        Console.WriteLine();

        Console.WriteLine("Enter path of document to open: ");
        string path = Console.ReadLine();

        if (!File.Exists(path))
            throw new FileNotFoundException($"File {path} does not exist");

        Console.WriteLine("Enter path of .bin file to open");
        string binPath = Console.ReadLine();

        if (!File.Exists(binPath))
            throw new FileNotFoundException($"File {binPath} does not exist");

        using (OpenXmlPackage doc = DocumentWrapper.Open(path, true))
        {
            var docPart = doc.RootPart; //.WorkbookPart;

            using (var storage = new VbProject(docPart))
            {
                PrintStorage(storage);
            }

            Console.WriteLine();
            Console.WriteLine("---------------REPLACING VBA PART----------------");
            Console.WriteLine();

            // Replace parts
            var vba = docPart
                .GetPartsOfType<VbaProjectPart>()
                .SingleOrDefault();

            if (vba != null)
                docPart.DeletePart(vba);

            VbaProjectPart newVbaPart = docPart.AddNewPart<VbaProjectPart>();
            using (var stream = File.OpenRead(binPath))
            {
                newVbaPart.FeedData(stream);
            }

            using (var storage = new VbProject(docPart))
            {
                PrintStorage(storage);
            }

            docPart.RootElement.Save(); //Workbook
        }
    }

    private static void PrintStorage(VbProject VbProject)
    {
        PrintStorage(VbProject.AsVbaStorage());
    }

    private static void PrintStorage(VbaStorage VbaStorage)
    {
        if (VbaStorage == null)
            throw new ArgumentNullException(nameof(VbaStorage));

        Console.WriteLine("- - - VbaStorage - - -");
        Console.WriteLine($"Project name: {VbaStorage.DirStream.InformationRecord.NameRecord.GetProjectNameAsString()}");
        Console.WriteLine($"Project docstring: {VbaStorage.DirStream.InformationRecord.DocStringRecord.GetDocStringAsString()}");
        Console.WriteLine($"Project constants: {VbaStorage.DirStream.InformationRecord.ConstantsRecord.GetConstantsAsString()}");

        foreach (KeyValuePair<string, ModuleStream> ms in VbaStorage.ModuleStreams)
        {
            Console.WriteLine($"\tModule stream: {ms.Key}");

            Console.WriteLine("Source code:");
            Console.WriteLine(ms.Value.GetUncompressedSourceCodeAsString());
        }
    }
}
