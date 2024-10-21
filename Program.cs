using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using ReportApi;
using ReportApi.Export;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/report", ([FromBody] JsonElement request) => {
    try
    {
        LocalReport report = new LocalReport();
        string? base64Rdlc = request.GetProperty("rdlc").GetString();
        var base64EncodedBytes = System.Convert.FromBase64String(base64Rdlc);
        var rdlcContent = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        using(TextReader reportFile =  new StringReader(rdlcContent)) {
            report.LoadReportDefinition(reportFile);
            var nameFile = request.GetProperty("name").GetString();
            var typeReport = request.GetProperty("type").GetString();
            //var data = request.GetProperty("dataSource");
            //IEnumerable<DataSetDTO>? dataItems =  data.Deserialize<IEnumerable<DataSetDTO>>();
            foreach(var dataset in request.GetProperty("dataSource").EnumerateArray()) {
                var collectionData = dataset.GetProperty("DataCollection").GetRawText();    
                var name = dataset.GetProperty("Name").GetString();
                DataTable? dataTable = (DataTable?)JsonConvert.DeserializeObject(collectionData, (typeof(DataTable)));
                report.DataSources.Add(new ReportDataSource(name, dataTable));
            }
            JsonElement parametersElement;
            if(request.TryGetProperty("parameters",out parametersElement))
            {
                //var textoparam = parametersElement.GetRawText();
                IEnumerable<ParamDTO>? param =  parametersElement.Deserialize<IEnumerable<ParamDTO>>();

                IEnumerable<ReportParameter>? listadoParametros = param?.Select(p => new ReportParameter(p.Name, p.Value));
                report.SetParameters(listadoParametros);
            }
            

            byte[] file = report.Render(ExportType.list[typeReport].Type);

            var mimeType = ExportType.list[typeReport].Mimetype;
            
            return Results.File( file, contentType: mimeType,$"{nameFile}.{ExportType.list[typeReport].Extension}");
        }
    }
    catch(Exception ex) {
        return Results.BadRequest(ex.Message);
    }

});


app.Run();
