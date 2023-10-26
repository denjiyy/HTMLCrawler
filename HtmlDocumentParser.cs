using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerHTML
{
    public class HtmlDocumentParser
    {
        private HtmlDocument htmlDocument;

        public HtmlDocumentParser(string htmlContent)
        {
            htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);
        }

        public TreeNode BuildTreeModel()
        {
            return new TreeNode(htmlDocument.DocumentNode);
        }

        public List<string> RelativePathSearch(string path)
        {
            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes(path);

            if (nodes is not null)
            {
                return nodes.Select(n => n.InnerHtml).ToList();
            }

            return new List<string>();
        }

        public void ChangeNodeContent(string path, string newContent)
        {
            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes(path);

            if (nodes is not null)
            {
                foreach (var node in nodes)
                {
                    node.InnerHtml = newContent;
                }
            }
        }

        public void CopyNodeContent(string sourcePath, string targetPath)
        {
            HtmlNodeCollection sourceNodes = htmlDocument.DocumentNode.SelectNodes(sourcePath);
            HtmlNode targetNode = htmlDocument.DocumentNode.SelectSingleNode(targetPath);

            if (sourceNodes is not null && targetNode is not null)
            {
                foreach (var sourceNode in sourceNodes)
                {
                    var newNode = sourceNode.CloneNode(true);
                    targetNode.AppendChild(newNode);
                }
            }
        }

        public void SaveToFile(string filePath)
        {
            string serializedHtml = htmlDocument.DocumentNode.OuterHtml;

            File.WriteAllText(filePath, serializedHtml);
        }
    }
}
