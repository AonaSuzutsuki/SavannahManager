using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class CommandCollector
    {
        private readonly LinkedList<string> _commands = new();
        private LinkedListNode<string> _current;

        public int MaxSaved { get; set; } = 32;

        public CommandCollector()
        {
            _commands.AddLast("");
            _current = _commands.Last;
        }

        public void Reset()
        {
            _current = _commands.Last;
        }

        public void AddCommand(string cmd)
        {
            if (_commands.Count > MaxSaved)
            {
                _commands.RemoveFirst();
            }

            var last = _commands.Last;
            if (last == null)
                return;

            var newNode = new LinkedListNode<string>(cmd);
            _commands.AddBefore(last, newNode);
        }

        public string GetPreviousCommand()
        {
            if (_current.Previous == null)
                return _current.Value;

            var value = _current.Previous.Value;
            _current = _current.Previous;
            return value;
        }


        public string GetNextCommand()
        {
            if (_current.Next == null)
                return _current.Value;

            var value = _current.Next.Value;
            _current = _current.Next;
            return value;
        }
    }
}
