using OpenMcdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VbProjectParserCore.Compression;
using VbProjectParserCore.Data;
using static Utils.Extensions;

namespace VbProjectParserCore;

class Program
{
    static void Main(string[] args)
    {
        string filename = @"H:\test.vbaProject.bin"; //todo
        var VbaStorage = new VbaStorage(filename);

        Console.WriteLine("- - - INFO - - -");

        PrintKeyValue("Project name", VbaStorage.DirStream.InformationRecord.NameRecord.GetProjectNameAsString());
        PrintKeyValue("Project docstring", VbaStorage.DirStream.InformationRecord.DocStringRecord.GetDocStringAsString());
        PrintKeyValue("Project constants", VbaStorage.DirStream.InformationRecord.ConstantsRecord.GetConstantsAsString());

        foreach (KeyValuePair<string, ModuleStream> ms in VbaStorage.ModuleStreams)
        {
            Console.WriteLine($"\tModule stream: {ms.Key}");

            Console.WriteLine("Source code:");
            Console.WriteLine(ms.Value.GetUncompressedSourceCodeAsString());
        }



        /*
        CompoundFile cf = new CompoundFile(filename);
        var root = cf.RootStorage;
        //root.VisitEntries(Visit, false);

        // Read (compressed data)
        CFStream thisWorkbookStream = root.GetStorage("VBA").GetStream("dir");
        Byte[] compressedData = thisWorkbookStream.GetData();
        File.WriteAllBytes("CompressedDirStream.bin", compressedData);
        var reader = new XlBinaryReader(ref compressedData);
        var container = new CompressedContainer(reader);

        // Decompress
        var buffer = new DecompressedBuffer();
        container.Decompress(buffer);
        Byte[] uncompressed = buffer.GetData();
        File.WriteAllBytes("UncompressedDirStream.bin", uncompressed);


        var uncompressedDataReader = new XlBinaryReader(ref uncompressed);
        var dirStream = new DirStream(uncompressedDataReader);

        //var uncompressedData = container.Uncompress();



       // ProcessDirStream(uncompressedData);

        Console.WriteLine("- - - INFO - - -");
        Console.WriteLine("Project name: {0}", dirStream.InformationRecord.NameRecord.GetProjectNameAsString());
        Console.WriteLine("Project docstring: {0}", dirStream.InformationRecord.DocStringRecord.GetDocStringAsString());
        Console.WriteLine("Project constants: {0}", dirStream.InformationRecord.ConstantsRecord.GetConstantsAsString());
        Console.WriteLine("Modules");
        foreach(var module in dirStream.ModulesRecord.Modules)
        {
            Console.WriteLine();
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            Console.WriteLine("\tName: {0}  (stream {1})", module.NameRecord.GetModuleNameAsString(), module.StreamNameRecord.GetStreamNameAsString());

            var streamName = module.StreamNameRecord.GetStreamNameAsString();
            var stream = root.GetStorage("VBA").GetStream(streamName).GetData();
            var localreader = new XlBinaryReader(ref stream);

            var moduleStream = new ModuleStream(dirStream.InformationRecord, module, localreader);

            Console.WriteLine("Source code:");
            Console.WriteLine(moduleStream.GetUncompressedSourceCodeAsString());

            Console.WriteLine("got stream");
        }*/



        Console.WriteLine("done");
        Console.ReadKey();
    }



    protected CFStream GetThisWorkbookStream(CompoundFile cf)
    {
        return cf.RootStorage.GetStorage("VBA").GetStream("ThisWorkbook");
    }

    protected CFStream GetThisWordDocumentStream(CompoundFile cf)
    {
        return cf.RootStorage.GetStorage("VBA").GetStream("ThisDocument");
    }

    protected static void Visit(CFItem item)
    {
        Console.WriteLine($"Item: {item.Name}. Is Storage: {item.IsStorage}. Is Stream: {item.IsStream}");

        if (item.IsStorage)
        {
            CFStorage storage = (CFStorage)item;
            Console.WriteLine($"Visiting Children of storage {storage.Name}");

            storage.VisitEntries(Visit, false);
        }

        if (item.IsStream && item.Name == "ThisWorkbook")  //todo
        {
            Console.WriteLine("Parsing ThisWorkbook");  //todo

            CFStream stream = (CFStream)item;
            byte[] data = stream.GetData();

            File.WriteAllBytes("ThisWorkbook.bin", data);  //todo

        }
    }
}
