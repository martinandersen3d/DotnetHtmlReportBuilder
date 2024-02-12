using System;
using System.Text;
using System.Collections.Generic;

// Source: https://github.com/martinandersen3d/DotnetHtmlReportBuilder

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
    void AddTableHeader(params string[] headers);
    void AddTableRecord(params object[] rowData);
}

public interface IReportPage
{
    IReportTable[] Tables { get; set; }
}

public class ReportPage : IReportPage
{
    public IReportTable[] Tables { get; set; }
}

public class ReportButton : IReportButton
{
    public string ButtonText { get; set; }
    public string ButtonUrl { get; set; }
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
        sb.AppendLine("<style>body{color: #333; line-height: 1.5; font-family: Roboto, Lato, \"Lucida Grande\", Tahoma, Sans-Serif;}</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body><div>");

        foreach (var table in reportPage.Tables)
        {
            sb.AppendLine("<h2>" + table.Title + "</h2>");
            sb.AppendLine("<p>" + table.Description + "</p>");

            sb.AppendLine("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"border-collapse: collapse;  margin: 0;\">");
            sb.AppendLine("<tr>");

            foreach (var header in table.TableHeaders)
            {
                sb.AppendLine("<th style=\"padding: 4px 16px 4px 4px; background-color: #f0f0f0; text-align: left; vertical-align: top;\">" + header + "</th>");
            }

            sb.AppendLine("</tr>");

            foreach (var row in table.TableBody)
            {
                sb.AppendLine("<tr>");
                foreach (var cell in row)
                {
                    sb.AppendLine("<td  style=\"padding: 4px; border: 1px solid #cccccc; text-align: left; vertical-align: top;\">" + cell + "</td>");
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
            }
            sb.AppendLine("<hr style='margin: 24px 0 8px'>");

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

    public void AddTableRecord(params object[] rowData)
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
                //formattedRow[i] = string.Empty;
                formattedRow[i] = "null";
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