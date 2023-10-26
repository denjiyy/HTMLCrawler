using CrawlerHTML;
using System;

namespace HTMLCrawlerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var crawler = new HtmlCrawler();
            string htmlDocument = GetHtmlDocument();

            crawler.BuildTreeFromHtml(htmlDocument);

            Console.WriteLine("Enter a command (PRINT, SET, COPY, EXIT):");

            string command = string.Empty;
            while ((command = Console.ReadLine().ToUpper()) != "EXIT")
            {
                switch (command.ToUpper())
                {
                    case "PRINT":
                        Console.WriteLine("Enter the relative path:");
                        string relativePath = Console.ReadLine();
                        var results = crawler.SearchElementsByRelativePath(relativePath);
                        Console.WriteLine(string.Join(", ", results));
                        break;
                    case "SET":
                        Console.WriteLine("Enter the relative path:");
                        string setRelativePath = Console.ReadLine();
                        Console.WriteLine("Enter the new value:");
                        string newValue = Console.ReadLine();
                        crawler.SetContentByRelativePath(setRelativePath, newValue);
                        break;
                    case "COPY":
                        Console.WriteLine("Enter the source relative path:");
                        string sourcePath = Console.ReadLine();
                        Console.WriteLine("Enter the target relative path:");
                        string targetPath = Console.ReadLine();
                        crawler.CopyNodeByRelativePath(sourcePath, targetPath);
                        break;
                    default:
                        Console.WriteLine("Invalid command. Please enter (PRINT, SET, COPY, EXIT).");
                        break;
                }
            }
        }

        private static string GetHtmlDocument()
        {
            return @"
                <html>
                    <body>
                        <p>Text1</p>
                        <p>Text2</p>
                        <p id='p3'>Text3</p>
                        <div>
                            <div>Text4</div>
                            <p>Text5</p>
                        </div>
                        <table>
                            <tr>
                                <td>11</td>
                            </tr>
                            <tr>
                                <td>22</td>
                            </tr>
                        </table>
                        <table id='table2'>
                            <tr>
                                <td>33</td>
                            </tr>
                            <tr>
                                <td>44</td>
                            </tr>
                        </table>
                        <a href='http://www.w3schools.com'>w3schools</a>
                        <img src='img_girl.bmp'/>
                    </body>
                </html>";
        }
    }
}