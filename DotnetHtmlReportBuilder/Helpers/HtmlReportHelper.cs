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
    void AddButton(string buttonText, string buttonUrl);
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
    public string ButtonText { get; set; } = "";
    public string ButtonUrl { get; set; } = "";
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
        sb.AppendLine("<style>");
        sb.AppendLine("body{color: #333; padding:25px; line-height: 1.5; font-family: Lato, Roboto,  \"Lucida Grande\", Tahoma, Sans-Serif;background-color: #efefef  ;}");
        sb.AppendLine(".shadow{box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-moz-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-webkit-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-o-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-ms-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);}");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body><div>");

        foreach (var table in reportPage.Tables)
        {
            if (table.Title != "") sb.AppendLine("<h2>" + table.Title + "</h2>");

            if (table.Description != "") sb.AppendLine("<p>" + table.Description + "</p>");

            sb.AppendLine("<table class=\"shadow\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"  style=\"border-collapse: collapse; border-radius: 10px; overflow: hidden; border: 1px solid #2f3030;\" >");

            // TABLE HEADERS
            if (table.TableHeaders != null && table.TableHeaders.Length > 0)
            {
                sb.AppendLine("<tr>");

                foreach (var header in table.TableHeaders)
                {
                    sb.AppendLine("<th style=\"padding: 16px 18px 16px 16px; color:white; font-size:15px; background-color:#2f3030;text-align: left; font-weight: normal; vertical-align: top;\">" + header + "</th>");
                }
                sb.AppendLine("</tr>");
            }

            // TABLE BODY
            if (table.TableBody != null && table.TableBody.Count > 0)
            {
                var counter = 0;
                foreach (var row in table.TableBody)
                {
                    // Color every second tr
                    var style = counter % 2 == 1  ? " style=\"background-color:rgb(242, 242, 242);\"" : " style=\"background-color:white;\""; 
                    sb.AppendLine( string.Format("<tr{0}>", style) );

                    foreach (var cell in row)
                    {
                        sb.AppendLine($"<td  style=\"padding: 10px 16px 10px 16px; border-top: 0px solid #cccccc; text-align: left; font-size:15px; line-height: 1.2; vertical-align: top; color: gray;\">" + cell + "</td>");
                        
                    }
                    sb.AppendLine("</tr>");
                    counter++;
                }

                sb.AppendLine("</table>");
            }

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
    public string[]? TableHeaders { get; private set; } // Nullable string array
    public List<string[]>? TableBody { get; private set; } // Nullable list of string arrays
    public IReportButton[]? Buttons { get; set; }

    // Empty constructor
    public ReportTable()
    {
        // Initialize as null
        TableHeaders = null;
        TableBody = null;
    }

    // Constructor with headers
    public ReportTable(string title, string description, string[]? tableHeaders)
    {
        Title = title;
        Description = description;
        TableHeaders = tableHeaders;
        TableBody = null; // Initialize as null
    }

    public void AddTableHeader(params string[] headers)
    {
        TableHeaders = headers;
    }

    public void AddTableRecord(params object[] rowData)
    {
        if (TableHeaders == null)
        {
            throw new InvalidOperationException("Table headers must be set before adding table records.");
        }

        if (TableBody == null)
        {
            TableBody = new List<string[]>(); // Initialize if not already initialized
        }

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

    public void AddButton(string buttonText, string buttonUrl)
    {
        if (Buttons == null)
        {
            Buttons = new IReportButton[] { new ReportButton { ButtonText = buttonText, ButtonUrl = buttonUrl } };
        }
        else
        {
            var list = new List<IReportButton>(Buttons);
            list.Add(new ReportButton { ButtonText = buttonText, ButtonUrl = buttonUrl });
            Buttons = list.ToArray();
        }
    }
}
