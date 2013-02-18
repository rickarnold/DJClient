using System;
using System.Collections.Generic;
using System.Text;

namespace CDG
{
    public class CDGChunkEnumerator : IEnumerator<Chunks.Chunk>
    {
        public CDGChunkEnumerator(List<Chunks.Chunk> chunks)
        {
            _Chunks = chunks;            
        }

        List<Chunks.Chunk> _Chunks;
        int _Index;

        int FindCDGChunk(int start)
        {
            int index = start;
            while (index < _Chunks.Count)
            {
                if (_Chunks[index].CDG)
                {
                    break;
                }
                index++;
            }

            return index;
        }

        #region IEnumerator Members

        public object Current
        {
            get
            {
                return _Chunks[_Index];
            }
        }

        public bool MoveNext()
        {
            _Index = FindCDGChunk(_Index + 1);
            return _Index < _Chunks.Count;
        }

        public void Reset()
        {
            _Index = FindCDGChunk(0);
        }

        #endregion

        #region IEnumerator<Chunk> Members

        CDG.Chunks.Chunk IEnumerator<CDG.Chunks.Chunk>.Current
        {
            get 
            {
                return _Chunks[_Index];
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
