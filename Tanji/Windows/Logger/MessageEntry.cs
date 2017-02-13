using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

namespace Tanji.Windows.Logger
{
    public class MessageEntry : IEnumerable<Tuple<string, Color>>
    {
        private readonly List<Tuple<string, Color>> _writeChunks;

        public MessageEntry()
        {
            _writeChunks = new List<Tuple<string, Color>>();
        }

        public void AddWriteChunk(string value, Color highlight)
        {
            _writeChunks.Add(Tuple.Create(value, highlight));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_writeChunks).GetEnumerator();
        }
        public IEnumerator<Tuple<string, Color>> GetEnumerator()
        {
            return _writeChunks.GetEnumerator();
        }
    }
}