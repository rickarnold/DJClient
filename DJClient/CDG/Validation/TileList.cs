using System;
using System.Collections.Generic;
using System.Text;

namespace CDG.Validation
{
    /// <summary>
    /// Encapsulates a list of tile commands that can 
    /// be found by row and column easily.
    /// </summary>
    class TileMemory
    {
        const int COL_COUNT = 50;
        const int ROW_COUNT = 18;

        public TileMemory()
        {
            _Columns = new List<List<List<CDG.Chunks.TileBlock>>>();
            CreateStorage();
        }

        public void Add(CDG.Chunks.TileBlock tile)
        {
            int column = tile.Column.Value;
            int row = tile.Row.Value;

            if (column < COL_COUNT && row < ROW_COUNT)
            {
                _Columns[column][row].Add(tile);
            }
        }

        public List<CDG.Chunks.TileBlock> Get(int column, int row)
        {
            return _Columns[column][row];
        }

        public void Clear()
        {
            _Columns.Clear();
            CreateStorage();
        }

        private void CreateStorage()
        {
            for (int columnIndex = 0; columnIndex < COL_COUNT; columnIndex++)
            {
                List<List<CDG.Chunks.TileBlock>> row = new List<List<CDG.Chunks.TileBlock>>();
                _Columns.Add(row);

                for (int rowIndex = 0; rowIndex < ROW_COUNT; rowIndex++)
                {
                    row.Add(new List<CDG.Chunks.TileBlock>());
                }
            }
        }

        List<List<List<CDG.Chunks.TileBlock>>> _Columns;
    }
}
