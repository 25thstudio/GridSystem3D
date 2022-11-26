using UnityEngine;

namespace The25thStudio.GridSystem3D
{
    public enum GridOrientation
    {
        XYZ,
        XZY,
        YXZ,
        YZX,
        ZXY,
        ZYX
    }

    public static class GridOrientationExtensions
    {
        
        public static Vector3 Translate(this GridOrientation grid, Vector3 original)
        {
            return grid switch
            {
                GridOrientation.XYZ => original,
                GridOrientation.XZY => new Vector3(original.x, original.z, original.y),
                GridOrientation.YXZ => new Vector3(original.y, original.x, original.z),
                GridOrientation.YZX => new Vector3(original.y, original.z, original.x),
                GridOrientation.ZXY => new Vector3(original.z, original.x, original.y),
                GridOrientation.ZYX => new Vector3(original.z, original.y, original.x),
                _ => original
            };
        }
    }
}