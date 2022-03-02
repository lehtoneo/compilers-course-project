using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject
{
    public class Node<T>
    {
        public T value { get; set; }
        public List<Node<T>> children { get; set; }
        public Node(T value)
        {
            this.value = value;
            this.children = new List<Node<T>>();
        }

        public Node(T value, List<Node<T>> children) : this(value)
        {
            this.children = children;
        }


        public void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.WriteLine(value);

            for (int i = 0; i < children.Count; i++)
                children[i].PrintPretty(indent, i == children.Count - 1);
        }
    }

}
