using CrawlerHTML.CustomDataStructures;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CrawlerHTML
{
    public class HtmlCrawler
    {
        private HtmlDocument document;

        public HtmlCrawler()
        {
            document = new HtmlDocument();
        }

        //public TreeNode BuildTreeFromHtml(string html)
        //{
        //    document.LoadHtml(html);

        //    var rootNode = new TreeNode(document.DocumentNode);
        //    CustomStack<TreeNode> stack = new();
        //    stack.Push(rootNode);

        //    foreach (var node in document.DocumentNode.DescendantsAndSelf())
        //    {
        //        if (node.Name != rootNode.Element.Name)
        //        {
        //            var parent = stack.Peek();
        //            var newNode = new TreeNode(node);
        //            parent.Children.Add(newNode);

        //            if (node.Name.CustomStartsWith("/"))
        //            {
        //                stack.Pop();
        //            }
        //            else if (node.NodeType == HtmlNodeType.Element)
        //            {
        //                stack.Push(newNode);
        //            }
        //        }
        //    }

        //    if (stack.Count > 1)
        //    {
        //        Console.WriteLine("Error: Unmatched opening tag(s).");
        //    }

        //    return rootNode;
        //}

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

        public CustomList<string> SearchElementsByRelativePath(string relativePath)
        {
            CustomList<string> pathSegments = new(relativePath.CustomTrim()
                .CustomSplit("/")
                .CustomWhere(x => !string.IsNullOrWhiteSpace(x)));

            if (pathSegments.Count == 0)
            {
                Console.WriteLine("Error: Invalid relative path.");
                return new CustomList<string>();
            }

            CustomList<TreeNode> currentNode = new(new[] { BuildTreeFromHtml(document.Text) });

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
                        CustomList<TreeNode> matchingNodes = new(node.Children.CustomWhere(n => n.Element.Name == tag).ToArray());

                        if (index >= 0 && index < matchingNodes.Count)
                        {
                            newNodes.Add(matchingNodes[index]);
                        }
                    }
                    else if (segment == "*")
                    {
                        newNodes.AddRange(node.Children.ToArray());
                    }
                    else if (segment.CustomContains("[@"))
                    {
                        var tag = segment.CustomSplit("[@")[0];
                        var attributePart = segment.CustomSplit("[@")[1].CustomTrimEnd(']');
                        var attributeName = attributePart.CustomSplit("=")[0];
                        var attributeValue = attributePart.CustomSplit("=")[1].CustomTrim('\'');
                        newNodes.AddRange(node.Element.DescendantsAndSelf(tag).CustomWhere(n => n.Attributes
                        .Contains(attributeName) && n.Attributes[attributeName].Value == attributeValue)
                            .CustomSelect(n => new TreeNode(n)).ToArray());
                    }
                }

                currentNode = newNodes;
            }

            CustomList<string> results = new();
            results.AddRange(currentNode.CustomSelect(node => node.Element.InnerText).ToArray());
            return results;
        }

        public void SetContentByRelativePath(string relativePath, string newValue)
        {
            CustomList<string> pathSegments = new(relativePath.CustomTrim()
                .CustomSplit("/")
                .CustomWhere(x => !string.IsNullOrWhiteSpace(x)));

            if (pathSegments.Count == 0)
            {
                Console.WriteLine("Error: Invalid relative path.");
                return;
            }

            CustomList<TreeNode> currentNode = new(new[] { BuildTreeFromHtml(document.Text) });

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

        public void ReplaceNodesByRelativePath(string relativePath, string newValue)
        {
            var nodesToReplace = SearchElementsByRelativePath(relativePath);

            for (int i = 0; i < nodesToReplace.Count; i++)
            {
                SetContentByRelativePath(relativePath, newValue);
            }
        }

        public void CopyNodeByRelativePath(string sourcePath, string targetPath)
        {
            CustomList<string> sourceNodesText = SearchElementsByRelativePath(sourcePath);
            CustomList<string> targetNodesText = SearchElementsByRelativePath(targetPath);

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
