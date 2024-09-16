using System.Collections;

namespace ReportApi.Export
{
    public  class ExportType
    {
        public static readonly ExportDataType Html = new ExportDataType("text/html", "html", "HTML5");
        public static readonly ExportDataType Pdf = new ExportDataType("application/pdf", "pdf", "PDF");
        public static readonly ExportDataType Excel = new ExportDataType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "xlsx", "EXCELOPENXML");
        public static readonly ExportDataType Word = new ExportDataType("application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "docx", "WORDOPENXML");
    

    public static readonly Dictionary<string, ExportDataType> list = new Dictionary<string, ExportDataType>()
        {
            {"HTML", Html},
            {"PDF", Pdf},
            {"EXCEL",Excel},
            {"WORD", Word }
        };

    }

    public record ExportDataType(string Mimetype, string Extension, string Type);

    public record ExportData(string Name, IEnumerable DataSource, bool Success = true, string Error = "", bool ValidateData = true);

}
