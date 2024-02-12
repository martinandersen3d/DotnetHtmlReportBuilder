using System;
using System.Text;
using System.Collections.Generic;

using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Creating report data

        // TABLE 1
        var table1 = new ReportTable();
        table1.Title = "Title 1";
        table1.Description = "Description for table 1";
        table1.AddTableHeader("Header 1", "Header 2", "Header 3");

        table1.AddTableRecord("Data 1", "Data 2", "Data 3"); // null value is treated as empty string
        table1.AddTableRecord("Data 4", "Data 5", "Data 6");
        table1.AddTableRecord(7, 8, null); // numbers are converted to string
        table1.AddTableRecord(true, false, true); // booleans are converted to string

        // TABLE 1 - BUTTONS (Optional)

        table1.Buttons = new IReportButton[]
        {
            new ReportButton { ButtonText = "Button 1", ButtonUrl = "https://example.com/button1" },
            new ReportButton { ButtonText = "Button 2", ButtonUrl = "https://example.com/button2" }
        };

        // TABLE 2

        var table2 = new ReportTable();
        table2.Title = "Title 2";
        table2.Description = "Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2";
        table2.AddTableHeader("Header A", "Header B", "Header C");

        table2.AddTableRecord("Data A1", "Data B1", "Data C1");
        table2.AddTableRecord("Data A2", "Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 ", "Data C2");
        table2.AddTableRecord("Data A3", null, "Data C3"); // null value is treated as empty string

        // Creating Report Page

        var reportPage = new ReportPage
        {
            Tables = new IReportTable[] { table1, table2 /*, add more tables here */}
        };

        // Generating HTML report
        var reportGenerator = new HtmlReportGenerator();
        string htmlReport = reportGenerator.GenerateReport(reportPage);

        // Log And Write File
        Console.WriteLine(htmlReport);
        string filePath = "C:\\Github\\Dotnet\\DotnetHtmlReportBuilder\\DotnetHtmlReportBuilder\\report.html";
        File.WriteAllText(filePath, htmlReport);
    }
}
