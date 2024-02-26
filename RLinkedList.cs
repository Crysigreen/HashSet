using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HashSet
{

    public class RLinkedListNode<T>
    {
        public T Value { get; set; }
        public RLinkedListNode<T> Next { get; set; }
        public RLinkedListNode<T> Previous { get; set; }

        public RLinkedListNode(T value)
        {
            Value = value;
        }
    }
    [DebuggerDisplay("Head = {_head.Value}, Tail = {_tail.Value}, Count = {Count}")]
    internal class RLinkedList<T>
    {
        private RLinkedListNode<T> _head;
        private RLinkedListNode<T> _tail;
        public int Count { get; private set; }

        public void AddFirst(T value)
        {
            var newNode = new RLinkedListNode<T>(value);
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                newNode.Next = _head;
                _head.Previous = newNode;
                _head = newNode;
            }
            Count++;
        }

        public void AddLast(T value)
        {
            var newNode = new RLinkedListNode<T>(value);
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                newNode.Previous = _tail;
                _tail.Next = newNode;
                _tail = newNode;
            }
            Count++;
        }

        public void RemoveFirst()
        {
            if (_head == null)
            {
                throw new InvalidOperationException("The list is empty");
            }
            if (_head == _tail)
            {
                _head = null;
                _tail = null;
            }
            else
            {
                _head = _head.Next;
                _head.Previous = null;
            }
            Count--;
        }

        public void RemoveLast()
        {
            if (_head == null)
            {
                throw new InvalidOperationException("The list is empty");
            }
            if (_head == _tail)
            {
                _head = null;
                _tail = null;
            }
            else
            {
                _tail = _tail.Previous;
                _tail.Next = null;
            }
            Count--;
        }

        public void ForEach(Action<T> action)
        {
            RLinkedListNode<T> current = _head;
            while (current != null)
            {
                action(current.Value);
                current = current.Next;
            }
        }

        public List<T> ToList()
        {
            List<T> list = new List<T>();
            RLinkedListNode<T> current = _head;
            int i = 0;
            while (current != null)
            {
                list.Add(current.Value);
                current = current.Next;
                i++;
            }
            return list;
        }


    }
}
