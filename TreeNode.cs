using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrawlerHTML.CustomDataStructures;

namespace CrawlerHTML
{
    public class TreeNode
    {
        public HtmlNode Element { get; set; }
        public CustomList<TreeNode> Children { get; set; }
        public CustomDictionary<string, string> Attributes { get; set; }


        public TreeNode(HtmlNode element)
        {
            Element = element;
            Children = new CustomList<TreeNode>();
        }

        public void AddChildren(TreeNode child)
        {
            Children.Add(child);
        }

        public TreeNode DeepCopy()
        {
            HtmlNode copyElement = new HtmlNode(Element.NodeType, Element.OwnerDocument, Element.StreamPosition);

            foreach (var attribute in Element.Attributes)
            {
                copyElement.Attributes.Add(attribute.Clone());
            }

            TreeNode copy = new(copyElement);

            foreach (var child in Children)
            {
                copy.Children.Add(child.DeepCopy());
            }

            return copy;
        }
    }
}
