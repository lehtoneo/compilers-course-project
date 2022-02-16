using System;
using System.Collections.Generic;
using System.Text;

namespace CompilersProject
{
    public class Node<T>
    {
        public T value { get; set; }
        public List<Node<T>> children { get; set; }
        public Node (T value) {
            this.value = value;
            this.children = new List<Node<T>>();
        }

        public Node (T value, List<Node<T>> children) : this (value)
        {
            this.children = children;
        }
    }
}
