using System.Collections;
using UnityEngine;

namespace The25thStudio.GridSystem3D
{
    public enum CellPivotPoint 
    {
        Center,
        TopLeft,
        Top,
        TopRight,
        Left,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public static class CellPivotPointExtensions
    {

        public static Vector3 Position(this CellPivotPoint pivot)
        {
            return pivot switch
            {
                CellPivotPoint.Center => Vector3.zero,
                CellPivotPoint.TopLeft => Vector3.up + Vector3.left,
                CellPivotPoint.Top => Vector3.up,
                CellPivotPoint.TopRight=> Vector3.up + Vector3.right,
                CellPivotPoint.Left => Vector3.left,
                CellPivotPoint.Right => Vector3.right,
                CellPivotPoint.BottomLeft => Vector3.down + Vector3.left,
                CellPivotPoint.Bottom => Vector3.down,
                CellPivotPoint.BottomRight => Vector3.down + Vector3.right,
                _ => Vector3.zero
            };
        }
    }
}