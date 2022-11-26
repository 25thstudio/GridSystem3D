using System;
using UnityEngine;

namespace The25thStudio.GridSystem3D
{
    [Serializable]
    public class GridSettings 
    {
        [SerializeField]
        private Vector2Int gridSize;

        [SerializeField]
        private float cellSize;

        [SerializeField]
        private GridOrientation orientation;

        [SerializeField]
        private CellPivotPoint pivotPoint;

        public int Width => gridSize.x;

        public int Height => gridSize.y;

        public Vector2Int GridSize => gridSize;
        public float CellSize => cellSize;

        public GridOrientation GridOrientation => orientation;

        public CellPivotPoint CellPivotPoint => pivotPoint;


        public bool IsValidPosition(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < gridSize.x && y < gridSize.y);
        }

        internal Vector3 Offset() { return new Vector3(gridSize.x, gridSize.y) / 2 * cellSize;  }

        private Vector3 HalfSize()
        {
            return orientation.Translate(new Vector3(cellSize / 2, cellSize / 2)); ;
        }

        
        internal Vector3 GetWorldPosition(Vector3 origin, int x, int y)
        {
            var transformPosition = orientation.Translate(origin) - Offset();
            var newPosition = (new Vector3(x, y) * cellSize) + transformPosition;
            newPosition = orientation.Translate(newPosition) + HalfSize();


            return newPosition;
        }

        internal bool GetXY(Vector3 position, out int x, out int y)
        {
            var offset = orientation.Translate(Offset());

            var newPosition = position + offset;


            newPosition = orientation.Translate(newPosition) / cellSize;

            x = Mathf.FloorToInt(newPosition.x);
            y = Mathf.FloorToInt(newPosition.y);

            return IsValidPosition(x, y);

        }

        internal Vector3 WireCellSize()
        {
            return orientation.Translate(new Vector3(cellSize, cellSize));
        }
    }
}