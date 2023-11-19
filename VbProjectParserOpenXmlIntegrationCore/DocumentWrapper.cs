using DocumentFormat.OpenXml.Packaging;

namespace VbProjectParserCore.OpenXml;

public class DocumentWrapper
{
    //public static

    public static OpenXmlPackage Open(string path, bool isEditable = true)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"File '{path}' not found", path);

        var extension = Path.GetExtension(path).Trim().Substring(1).ToLower();

        switch (extension)
        {
            case "dot":
            case "dotm":
            case "dotx":
            case "doc":
            case "docx":
                return WordprocessingDocument.Open(path, isEditable);
            case "xlsx":
            case "xlsm":
            case "xlsb":
            case "xls":
            case "xltm":
            case "xltx":
            case "xla":
                return SpreadsheetDocument.Open(path, isEditable);

            default:
                return null;
        }


    }

}
