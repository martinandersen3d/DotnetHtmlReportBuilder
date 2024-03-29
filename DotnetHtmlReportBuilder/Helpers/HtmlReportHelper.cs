﻿using System;
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
    void SetTableHeader(params string[] headers);
    void AddTableRecord(params object[] rowData);
    void AddButton(string buttonText, string buttonUrl);
}

public interface IReportPage
{
    IReportTable[] Tables { get; set; }
    void AddReportTable(IReportTable table);
    void AddReportTables(params IReportTable[] tables);
}

public class ReportPage : IReportPage
{
    public IReportTable[] Tables { get; set; }

    public ReportPage()
    {
        Tables = Array.Empty<IReportTable>();
    }

    public ReportPage(params IReportTable[] tables)
    {
        Tables = tables;
    }

    public void AddReportTable(IReportTable table)
    {
        List<IReportTable> tableList = new List<IReportTable>(Tables);
        tableList.Add(table);
        Tables = tableList.ToArray();
    }

    public void AddReportTables(params IReportTable[] tables)
    {
        List<IReportTable> tableList = new List<IReportTable>(Tables);
        tableList.AddRange(tables);
        Tables = tableList.ToArray();
    }
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
        sb.AppendLine("body{color: #333; padding:25px; line-height: 1.5; font-family: Aptos, Lato, Roboto, Arial, \"Lucida Grande\", Tahoma, Sans-Serif;}");
        // HACK: Outlook does not like fontsize using 'px', so we use 'pt' inline on the element, and override it with 'px' here. It gives the most consistent result
        sb.AppendLine("td, th, a {font-size:16px !important;}");
        sb.AppendLine("p, span {font-size:16px !important;}");
        // sb.AppendLine(".shadow{box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-moz-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-webkit-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-o-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);-ms-box-shadow: 0 0 40px 0 rgba(0,0,0,.15);}");
        sb.AppendLine(".data-table tr:hover>td {background-color:#dbf4ff;}");
        sb.AppendLine(".data-table tr:hover { outline: 1px solid  #007bff;}");
        sb.AppendLine(".data-table {border-radius: 4px;border-collapse: separate !important; border-spacing: 0 !important;}");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body><div>");

        foreach (var table in reportPage.Tables)
        {
            if (table.Title != "" && table.Title != null) sb.AppendLine("<h2>" + table.Title + "</h2>");

            if (table.Description != "" && table.Description != null) sb.AppendLine("<p style=\"font-size:11pt;\">" + table.Description + "</p>");
            if (table.TableBody != null || table.TableHeaders != null)
                sb.AppendLine("<table class=\"data-table\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\"  style=\"border-collapse: collapse; overflow: hidden; border: 1px solid #2f3030;\" >");

            // TABLE HEADERS
            if (table.TableHeaders != null && table.TableHeaders.Length > 0)
            {
                var counter = 1;
                var total = table.TableHeaders.Length;

                sb.AppendLine("<thead><tr style=\"height:40px;\">");

                foreach (var header in table.TableHeaders)
                {
                    var str = "";
                    if (counter >= 1 && counter != total) str += "padding-left: 16px;";
                    if (total == counter && counter > 1) str += "padding-left: 16px; padding-right: 16px;";
                    if (total == counter && counter == 1) str += "padding-left: 16px; padding-right: 16px;";

                    sb.AppendLine("<th style=\"" + str + "font-size:10pt; color:white; background-color:#2f3030;text-align: left; font-weight: normal; vertical-align: middle;\">" + header + "</th>");
                    counter++;
                }
                sb.AppendLine("</tr></thead>");
            }

            // TABLE BODY
            if (table.TableBody != null && table.TableBody.Count > 0)
            {
                var counter = 0;
                foreach (var row in table.TableBody)
                {
                    // Color every second tr
                    var style = counter % 2 == 1 ? "background-color:rgb(242, 242, 242);" : "background-color:white;";
                    sb.AppendLine(string.Format("<tr style=\"{0}\">", style));

                    foreach (var cell in row)
                    {
                        sb.AppendLine($"<td  style=\"font-size:10pt; padding: 10px 16px 10px 16px; border-top: 0px solid #cccccc; text-align: left; line-height: 1.2; vertical-align: top; color: gray;\">" + cell + "</td>");

                    }
                    sb.AppendLine("</tr>");
                    counter++;
                }

            }
            if (table.TableBody != null || table.TableHeaders != null) sb.AppendLine("</table>");

            // Adding buttons if available
            if (table.Buttons != null && table.Buttons.Length > 0)
            {
                sb.AppendLine("<div'>");
                sb.AppendLine(@" <table cellspacing=""0"" cellpadding=""0"">");
                sb.AppendLine(@"<tr>");
                foreach (var button in table.Buttons)
                {
                    sb.AppendLine(GenerateButton(button.ButtonText, button.ButtonUrl));
                }
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</div>");
            }
            sb.AppendLine("<hr style='margin: 24px 0 8px'>");

        }

        sb.AppendLine("</div></body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    // Outlook email: yes all this is needed to render correctly in Outlook
    private string GenerateButton(string buttonText, string buttonUrl)
    {
        var text = @"
  
     <td align=""left"" style=""padding: 32px 10px 10px 0; "">
       <table border=""0"" class=""mobile-button"" cellspacing=""0"" cellpadding=""0"">
         <tr>
           <td align=""left"" bgcolor=""#007bff"" style=""background-color: #007bff; margin: auto; max-width: 600px; -webkit-border-radius: 4px; -moz-border-radius: 4px; border-radius: 4px; padding: 9px 18px; "" width=""100%"">
           <!--[if mso]>&nbsp;<![endif]-->
               <a href=""__BUTTONURL__"" target=""_blank"" style=""font-family: Arial, sans-serif; color: #ffffff; font-weight:normal; text-align:center; background-color: #007bff; text-decoration: none; border: none; -webkit-border-radius: 4px; -moz-border-radius: 4px; border-radius: 4px; display: inline-block;"">
                   <span style=""font-size: 11pt; font-family: Arial, sans-serif; color: #ffffff; font-weight:normal; line-height:1.5em; text-align:center;"">__BUTTONTEXT__</span>
             </a>
           <!--[if mso]>&nbsp;<![endif]-->
           </td>
         </tr>
       </table>
     </td>
";

        return text.Replace("__BUTTONURL__", buttonUrl).Replace("__BUTTONTEXT__", buttonText);
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

    public void SetTableHeader(params string[] headers)
    {
        TableHeaders = headers;
    }

    public void AddTableRecord(params object[] rowData)
    {
        // if (TableHeaders == null)
        // {
        //     throw new InvalidOperationException("Table headers must be set before adding table records.");
        // }

        if (TableBody == null)
        {
            TableBody = new List<string[]>(); // Initialize if not already initialized
        }

        // if (rowData.Length != TableHeaders.Length)
        // {
        //     throw new ArgumentException("Number of elements in the rowData array must match the number of table headers.");
        // }

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
