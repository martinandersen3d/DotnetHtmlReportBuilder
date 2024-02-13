using System;
using System.Text;
using System.Collections.Generic;

using System.IO;
using static HtmlReportGenerator;

class Program
{
    static void Main(string[] args)
    {
        // Creating report data

        // TABLE 1 -------------------------------------------------------
        var table1 = new ReportTable();
        table1.Title = "Title 1 Example";
        table1.Description = "Description for table 1";
        table1.AddTableHeader("Header 1", "Header 2", "Header 3");

        table1.AddTableRecord("Data 1", "Data 2", "Data 3"); // null value is treated as empty string
        table1.AddTableRecord("Data 4", "Data 5", "Data 6");
        table1.AddTableRecord(7, 8, null); // numbers are converted to string
        table1.AddTableRecord(true, false, true); // booleans are converted to string

        // TABLE 1 - BUTTONS (Optional)
        table1.AddButton("Link Button 1", "https://example.com/button1");
        table1.AddButton("Link Button 2 Example", "https://example.com/button2");

        // TABLE 2 -------------------------------------------------------

        var table2 = new ReportTable();
        table2.Title = "Title 2 Example";
        table2.Description = "Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2, Description for table 2";
        table2.AddTableHeader("Header A", "Header B", "Header C", "Header D");

        table2.AddTableRecord("Data A1", "Data B1", "Data C1", "Data D1");
        table2.AddTableRecord("Data A2", "Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 Data B2 ", "Data C2", "Data D2");
        table2.AddTableRecord("Data A3", null, "Data C3", "Data D3"); // null value is treated as empty string

        // TABLE 3 -------------------------------------------------------

        var table3 = new ReportTable();
        table3.Title = "Title 3 Example";
        table3.Description = "Just demonstrating a title and description.";

        // TABLE 4 -------------------------------------------------------
        var table4 = new ReportTable();
        table4.AddTableHeader("Id", "Key", "Value");

        table4.AddTableRecord("1", "Table", "4"); // null value is treated as empty string
        table4.AddTableRecord("2", "Description", "Just demonstrating a table, without title and description");
        table4.AddTableRecord("3", "John", "Doe");

        // Creating Report Page ------------------------------------------

        var reportPage = new ReportPage
        {
            Tables = new IReportTable[] { table1, table2, table3, table4 /*, add more tables here */}
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
