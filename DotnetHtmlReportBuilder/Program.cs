using System;
using System.Text;
using System.Collections.Generic;

using System.IO;

public interface IReportButton
{
    string ButtonText { get; set; }
    string ButtonUrl { get; set; }
}

public interface IReportTable
{
    string Title { get; set; }
    string Description { get; set; }
    string[] TableHeaders { get; }
    List<string[]> TableBody { get; }
    IReportButton[]? Buttons { get; set; } // Nullable array of IReportButton
    void Add(params object[] rowData);
    void AddTableHeader(params string[] headers);
}

public interface IReportPage
{
    IReportTable[] Tables { get; set; }
}

public class HtmlReportGenerator
{
    public string GenerateReport(IReportPage reportPage)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<title>HTML Report</title>");
        sb.AppendLine("<style>body{font-family: Roboto, Lato, \"Lucida Grande\", Tahoma, Sans-Serif;}</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body><div style='margin-left:auto; margin-right:auto;min-width: 500px; display: inline-block;'>");

        foreach (var table in reportPage.Tables)
        {
            sb.AppendLine("<h2>" + table.Title + "</h2>");
            sb.AppendLine("<p>" + table.Description + "</p>");

            sb.AppendLine("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"border-collapse: collapse; width: 100%; max-width: 600px; margin: 0 auto;\">");
            sb.AppendLine("<tr>");

            foreach (var header in table.TableHeaders)
            {
                sb.AppendLine("<th style=\"padding: 4px; background-color: #f0f0f0; text-align: left;\">" + header + "</th>");
            }

            sb.AppendLine("</tr>");

            foreach (var row in table.TableBody)
            {
                sb.AppendLine("<tr>");
                foreach (var cell in row)
                {
                    sb.AppendLine("<td  style=\"padding: 4px; border: 1px solid #cccccc; text-align: left;\">" + cell + "</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            // Adding buttons if available
            if (table.Buttons != null && table.Buttons.Length > 0)
            {
                sb.AppendLine("<div style='padding-top: 16px;'>");
                foreach (var button in table.Buttons)
                {
                    sb.AppendLine($"<a href='{button.ButtonUrl}' style=\"background-color: #1DA1F2; color: white; text-decoration: none; padding: 10px 20px; border-radius: 5px; cursor: pointer;display: inline-block;\">{button.ButtonText}</a>");
                }
                sb.AppendLine("</div>");
                sb.AppendLine("<hr style='margin: 16px 0'>");
            }

        }

        sb.AppendLine("</div></body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}

public class ReportTable : IReportTable
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string[] TableHeaders { get; private set; }
    public List<string[]> TableBody { get; private set; } // Change to List<string[]>
    public IReportButton[]? Buttons { get; set; }

    // Empty constructor
    public ReportTable()
    {
        TableHeaders = new string[0];
        TableBody = new List<string[]>(); // Initialize as a new List<string[]>
    }

    // Constructor with headers
    public ReportTable(string title, string description, string[] tableHeaders)
    {
        Title = title;
        Description = description;
        TableHeaders = tableHeaders;
        TableBody = new List<string[]>(); // Initialize as a new List<string[]>
    }

    public void AddTableHeader(params string[] headers)
    {
        TableHeaders = headers;
    }

    public void Add(params object[] rowData)
    {
        if (rowData.Length != TableHeaders.Length)
        {
            throw new ArgumentException("Number of elements in the rowData array must match the number of table headers.");
        }

        string[] formattedRow = new string[rowData.Length];
        for (int i = 0; i < rowData.Length; i++)
        {
            if (rowData[i] == null)
            {
                formattedRow[i] = string.Empty;
            }
            else if (rowData[i] is bool)
            {
                formattedRow[i] = ((bool)rowData[i]) ? "True" : "False";
            }
            else if (rowData[i] is IFormattable)
            {
                formattedRow[i] = ((IFormattable)rowData[i]).ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                try
                {
                    formattedRow[i] = Convert.ToString(rowData[i]);
                }
                catch (Exception)
                {
                    formattedRow[i] = "[Error Conversion]";
                }
            }
        }

        TableBody.Add(formattedRow); // Add the formatted row to the list
    }
}

public class ReportPage : IReportPage
{
    public IReportTable[] Tables { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // Creating report data
        var table1 = new ReportTable();
        table1.Title = "Table 1";
        table1.Description = "This is table 1";
        table1.AddTableHeader("Header 1", "Header 2", "Header 3");

        table1.Add("Data 1", null, "Data 3"); // null value is treated as empty string
        table1.Add("Data 4", "Data 5", "Data 6");
        table1.Add(7, 8, null); // numbers are converted to string
        table1.Add(true, false, true); // booleans are converted to string

        table1.Buttons = new IReportButton[]
        {
            new ReportButton { ButtonText = "Button 1", ButtonUrl = "https://example.com/button1" },
            new ReportButton { ButtonText = "Button 2", ButtonUrl = "https://example.com/button2" }
        };

        var table2 = new ReportTable();
        table2.Title = "Table 2";
        table2.Description = "This is table 2";
        table2.AddTableHeader("Header A", "Header B", "Header C");

        table2.Add("Data A1", "Data B1", "Data C1");
        table2.Add("Data A2", "Data B2", "Data C2");
        table2.Add("Data A3", null, "Data C3"); // null value is treated as empty string

        var reportPage = new ReportPage
        {
            Tables = new IReportTable[] { table1, table2 }
        };

        // Generating HTML report
        var reportGenerator = new HtmlReportGenerator();
        string htmlReport = reportGenerator.GenerateReport(reportPage);
        Console.WriteLine(htmlReport);
        string filePath = "C:\\Github\\Dotnet\\DotnetHtmlReportBuilder\\DotnetHtmlReportBuilder\\report.html";
        File.WriteAllText(filePath, htmlReport);
    }
}

public class ReportButton : IReportButton
{
    public string ButtonText { get; set; }
    public string ButtonUrl { get; set; }
}
