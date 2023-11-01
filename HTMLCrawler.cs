using CrawlerHTML.CustomDataStructures;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CrawlerHTML
{
    public class HTMLCrawler
    {
        private HtmlDocument document;

        public HTMLCrawler()
        {
            document = new HtmlDocument();
        }

        public TreeNode BuildTreeFromHtml(string html)
        {
            document.LoadHtml(html);

            var rootNode = new TreeNode(document.DocumentNode);
            CustomStack<TreeNode> stack = new();
            stack.Push(rootNode);

            try
            {
                foreach (var node in document.DocumentNode.DescendantsAndSelf())
                {
                    if (node.NodeType == HtmlNodeType.Element)
                    {
                        var parent = stack.Peek();
                        var newNode = new TreeNode(node);
                        parent.Children.Add(newNode);
                        stack.Push(newNode);
                    }
                    else if (node.NodeType == HtmlNodeType.Text)
                    {
                        var parent = stack.Peek();
                        parent.Children.Add(new TreeNode(node));
                    }
                    else if (node.NodeType == HtmlNodeType.Document && node != document.DocumentNode)
                    {
                        stack.Pop();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Unmatched opening or closing tag(s).");
                Console.WriteLine(ex.Message);
            }

            return rootNode;
        }

        public CustomList<string> SearchElementsByRelativePath(string relativePath, TreeNode rootNode)
        {
            CustomList<string> pathSegments = new(relativePath.CustomTrim()
                .CustomSplit("/")
                .CustomWhere(x => !string.IsNullOrWhiteSpace(x)));

            if (pathSegments.Count == 0)
            {
                Console.WriteLine("Error: Invalid relative path.");
                return new CustomList<string>();
            }

            CustomList<TreeNode> currentNode = new(new[] { rootNode });

            foreach (var segment in pathSegments)
            {
                CustomList<TreeNode> newNodes = new();

                string currentSegment = segment;

                if (currentSegment == "//")
                {
                    // Handle double slashes as a root selector
                    newNodes.Add(rootNode);
                }
                else if (currentSegment.CustomStartsWith("//"))
                {
                    currentSegment = currentSegment.Substring(2);
                    newNodes.AddRange(currentNode[0].Element.DescendantsAndSelf(currentSegment)
                        .Where(n => n.NodeType == HtmlNodeType.Element || (n.NodeType == HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText)))
                        .Select(n => new TreeNode(n)).ToArray());
                }
                else
                {
                    foreach (var node in currentNode)
                    {
                        if (currentSegment.CustomContains("[") && currentSegment.CustomEndsWith("]"))
                        {
                            var tag = currentSegment.CustomSplit("[")[0];
                            var index = int.Parse(currentSegment.Split(new char[] { '[', ']' })[1]) - 1;
                            CustomList<TreeNode> matchingNodes = new(node.Children
                                .Where(n => n.Element.Name == tag)
                                .ToArray());

                            if (index >= 0 && index < matchingNodes.Count)
                            {
                                newNodes.Add(matchingNodes[index]);
                            }
                        }
                        else if (currentSegment == "*")
                        {
                            // Handle "*" as a wildcard to select all elements within the current tag
                            newNodes.AddRange(node.Element.ChildNodes
                                .Where(n => n.NodeType == HtmlNodeType.Element || (n.NodeType == HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText)))
                                .Select(n => new TreeNode(n)).ToArray());
                        }
                        else
                        {
                            // Modify the logic to stop searching when the specified tag is encountered
                            bool found = false;
                            foreach (var n in node.Element.Elements(currentSegment))
                            {
                                if (n.Name == currentSegment)
                                {
                                    found = true;
                                    newNodes.Add(new TreeNode(n));
                                }
                                else if (found)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                currentNode = newNodes;
            }

            Console.WriteLine($"Debug - Current path segments: {string.Join(", ", pathSegments)}");
            Console.WriteLine($"Debug - Current node count: {currentNode.Count}");

            CustomList<string> results = new CustomList<string>();
            results.AddRange(currentNode
                .Select(node => node.Element.InnerText).Where(text => !string.IsNullOrWhiteSpace(text)).Distinct().ToArray());

            return results;
        }

        public void SetContentByRelativePath(string relativePath, string newValue, TreeNode rootNode)
        {
            CustomList<string> pathSegments = new(relativePath.CustomTrim()
                .CustomSplit("/")
                .CustomWhere(x => !string.IsNullOrWhiteSpace(x)));

            if (pathSegments.Count == 0)
            {
                Console.WriteLine("Error: Invalid relative path.");
                return;
            }

            CustomList<TreeNode> currentNode = new(new[] { rootNode });

            foreach (var segment in pathSegments)
            {
                CustomList<TreeNode> newNodes = new();

                foreach (var node in currentNode)
                {
                    if (segment.CustomStartsWith("//"))
                    {
                        newNodes.AddRange(node.Element.DescendantsAndSelf(segment.TrimStart('/')).CustomSelect(n => new TreeNode(n)).ToArray());
                    }
                    else if (segment.CustomContains("[") && segment.CustomEndsWith("]"))
                    {
                        var tag = segment.CustomSplit("[")[0];
                        var index = int.Parse(segment.Split(new char[] { '[', ']' })[1]) - 1;
                        CustomList<TreeNode> matchingNodes = new(node.Children
                            .CustomWhere(n => n.Element.Name == tag).ToArray());

                        if (index >= 0 && index < matchingNodes.Count)
                        {
                            newNodes.Add(matchingNodes[index]);
                        }
                    }
                }

                currentNode = newNodes;
            }

            foreach (var node in currentNode)
            {
                if (node.Element.NodeType == HtmlNodeType.Element)
                {
                    node.Element.InnerHtml = newValue;
                }
                else if (node.Element.NodeType == HtmlNodeType.Text)
                {
                    node.Element.InnerHtml = HtmlDocument.HtmlEncode(newValue);
                }
            }
        }

        public void ReplaceNodesByRelativePath(string relativePath, string newValue, TreeNode rootNode)
        {
            var nodesToReplace = SearchElementsByRelativePath(relativePath, rootNode);

            for (int i = 0; i < nodesToReplace.Count; i++)
            {
                SetContentByRelativePath(relativePath, newValue, rootNode);
            }
        }

        public void CopyNodeByRelativePath(string sourcePath, string targetPath, TreeNode rootNode)
        {
            CustomList<string> sourceNodesText = SearchElementsByRelativePath(sourcePath, rootNode);
            CustomList<string> targetNodesText = SearchElementsByRelativePath(targetPath, rootNode);

            if (sourceNodesText.Count == 0)
            {
                Console.WriteLine("Error: Source node not found.");
                return;
            }

            if (targetNodesText.Count == 0)
            {
                Console.WriteLine("Error: Target node not found.");
                return;
            }

            CustomList<TreeNode> copiedNodes = new();

            foreach (var sourceNodeText in sourceNodesText)
            {
                var sourceNode = FindNodeByInnerText(BuildTreeFromHtml(document.Text), sourceNodeText);

                if (sourceNode is not null)
                {
                    TreeNode copy = sourceNode.DeepCopy();
                    copiedNodes.Add(copy);
                }
            }

            foreach (var targetNodeText in targetNodesText)
            {
                var targetNode = FindNodeByInnerText(BuildTreeFromHtml(document.Text), targetNodeText);
                targetNode?.Children.AddRange(copiedNodes.ToArray());
            }
        }

        public TreeNode FindNodeByInnerText(TreeNode node, string innerText)
        {
            if (node.Element.InnerText == innerText)
            {
                return node;
            }

            foreach (var child in node.Children)
            {
                var foundNode = FindNodeByInnerText(child, innerText);

                if (foundNode != null)
                {
                    return foundNode;
                }
            }

            return null;
        }
    }
}