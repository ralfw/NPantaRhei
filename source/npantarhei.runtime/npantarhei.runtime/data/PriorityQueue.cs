using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace npantarhei.runtime.data
{
    class PriorityQueue<T>
    {
        private readonly List<Entry<T>> _entries = new List<Entry<T>>();

        private class Entry<T>
        {
            public readonly int Priority;
            public readonly T Data;

            public Entry(int priority, T data)
            {
                Priority = priority;
                Data = data;
            }
        }


        public void Enqueue(T data) { Enqueue(0, data); }
        public void Enqueue(int priority, T data)
        {
            var e = new Entry<T>(priority, data);

            if (_entries.Count == 0)
                _entries.Add(e);
            else
            {
                for (var i = 0; i < _entries.Count; i++)
                {
                    if (_entries[i].Priority < priority)
                    {
                        _entries.Insert(i, e);
                        e = null;
                        break;
                    }
                }
                if (e != null) _entries.Add(e);
            }
        }


        public T Dequeue()
        {
            if (_entries.Count == 0) throw new InvalidOperationException("Priority queue is empty!");

            var entry = _entries[0];
            _entries.RemoveAt(0);
            return entry.Data;
        }


        public int Count
        {
            get { return _entries.Count; }
        }
    }
}
