using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using VbProjectParserCore.Data;

namespace VbProjectParserCore.OpenXml;

/// <summary>
/// Class that exposes members from VbaStorage in a more user-friendly way
/// </summary>
public class VbProject : IDisposable
{
    private VbaStorage m_Storage;
    private IDisposable m_documentDisposable;

    private readonly Lazy<IEnumerable<VbModule>> m_Modules;
    public IEnumerable<VbModule> Modules
    {
        get { return m_Modules.Value; }
    }

    public string Name
    {
        get { return m_Storage.DirStream.InformationRecord.NameRecord.GetProjectNameAsString(); }
    }

    public VbProject(string PathToDocument)
        : this(LoadDocumentFrom(PathToDocument), true)
    {
    }

    public VbProject(OpenXmlPackage document)
        : this(document, false)
    {
    }


    private VbProject(OpenXmlPackage document, bool DisposeDocument)
        : this((document is SpreadsheetDocument)
              ? (document as SpreadsheetDocument).WorkbookPart
              : (document as WordprocessingDocument).MainDocumentPart)  //todo clean up
    {
        if (DisposeDocument)
            this.m_documentDisposable = document;
    }

    public VbProject(OpenXmlPart workbookPart)
        : this(GetVbaProjectPartFrom(workbookPart))
    {
    }

    public VbProject(VbaProjectPart vbaProjectPart)
        : this()
    {
        if (vbaProjectPart == null)
            throw new ArgumentNullException(nameof(vbaProjectPart));

        var stream = GetVbaStreamFrom(vbaProjectPart);
        this.m_Storage = new VbaStorage(stream);
    }

    public VbProject(VbaStorage vbaStorage)
        : this()
    {
        this.m_Storage = vbaStorage;
    }

    private VbProject()
    {
        this.m_Modules = new Lazy<IEnumerable<VbModule>>(CreateModuleInfos);
    }

    public void Dispose()
    {
        if (m_documentDisposable != null)
        {
            m_documentDisposable.Dispose();
            m_documentDisposable = null;
        }

        if (m_Storage != null)
        {
            m_Storage.Dispose();
            m_Storage = null;
        }
    }

    public VbaStorage AsVbaStorage()
    {
        return m_Storage;
    }

    private IEnumerable<VbModule> CreateModuleInfos()
    {
        foreach (var kvp in m_Storage.ModuleStreams)
        {
            string module_name = kvp.Key;
            ModuleStream module_stream = kvp.Value;
            string module_code = module_stream.GetUncompressedSourceCodeAsString();

            VbModule moduleInfo = new VbModule(module_name, module_code);
            yield return moduleInfo;
        }
    }

    private static OpenXmlPackage LoadDocumentFrom(string path)
    {
        return DocumentWrapper.Open(path, true);
    }

    private static VbaProjectPart GetVbaProjectPartFrom(OpenXmlPart workbookPart)
    {
        if (workbookPart == null)
            throw new ArgumentNullException(nameof(workbookPart));

        var allParts = workbookPart.GetPartsOfType<VbaProjectPart>();
        var vba = allParts.SingleOrDefault();
        return vba;
    }

    private static Stream GetVbaStreamFrom(VbaProjectPart vbaProjectPart)
    {
        if (vbaProjectPart == null)
            throw new ArgumentNullException(nameof(vbaProjectPart));

        var stream = vbaProjectPart.GetStream();
        return stream;
    }
}