using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace The25thStudio.GridSystem3D
{
    public class Grid<T>
    {
        private readonly GridSettings _settings;
        private readonly Vector3 _originPosition;
        private readonly GridCell<T>[,] _gridArray;
        private readonly UnityEvent<int, int, T> _setValueEvent;

        public Grid(GridSettings settings, Vector3 originPosition, Func<int, int, T> initializeGrid = default)
        {
            this._settings = settings;
            this._originPosition = originPosition;
            this._gridArray = new GridCell<T>[_settings.Width, _settings.Height];
            this._setValueEvent = new UnityEvent<int, int, T>();


            InitializeGridArray(initializeGrid);

        }

        private void InitializeGridArray(Func<int, int, T> createGridObject = default)
        {
            bool validFunction = createGridObject != default;
            for (var x = 0; x < _settings.Width; x++)
            {
                for (var y = 0; y < _settings.Height; y++)
                {
                    var value = validFunction ? createGridObject(x, y) : default;
                    _gridArray[x, y] = new GridCell<T>(value, 1, 1);
                }
            }
        }

        #region Listeners
        public void AddListener(UnityAction<int, int, T> action)
        {
            _setValueEvent.AddListener(action);
        }


        public void RemoveListener(UnityAction<int, int, T> action)
        {
            _setValueEvent.RemoveListener(action);
        }

        #endregion


        #region Position
        public bool IsEmpty(int x, int y)
        {
            return IsEmpty(x, y, 1, 1);
        }

        public bool IsEmpty(int x, int y, int width, int height)
        {
            for (var w = 0; w < width; w++)
            {
                for (var h = 0; h < height; h++)
                {
                    var xw = x + w;
                    var yh = y + h;
                    if (IsValidPosition(xw, yh))
                    {
                        var value = _gridArray[xw, yh];                        
                        if (!value.IsEmpty())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }


                }
            }
            return true;
        }

        public bool IsEmpty(Vector3 worldPosition)
        {
            return IsEmpty(worldPosition, 1, 1);

        }

        public bool IsEmpty(Vector3 worldPosition, int width, int height)
        {
            return IsEmpty(worldPosition, width, height, out _, out _);
        }

        public bool IsEmpty(Vector3 worldPosition, int width, int height, out int x, out int y)
        {
            if (GetXY(worldPosition, out x, out y))
            {
                return IsEmpty(x, y, width, height);
            }
            return false;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return _settings.GetWorldPosition(_originPosition, x, y);
        }

        public bool GetXY(Vector3 worldPosition, out int x, out int y)
        {
            return _settings.GetXY(worldPosition - _originPosition, out x, out y);
        }

        public bool IsValidPosition(int x, int y)
        {
            return _settings.IsValidPosition(x, y);
        }

        #endregion

        #region Value
        public bool SetValue(int x, int y, T value)
        {
            return SetValue(x, y, 1, 1, value);
        }

        public bool SetValue(int x, int y, int width, int height, T value)
        {
            if (IsEmpty(x, y, width, height))
            {
                var parent = _gridArray[x, y];
                for (var w = 0; w < width; w++)
                {
                    for (var h = 0; h < height; h++)
                    {
                        var xw = x + w;
                        var yh = y + h;

                        if (xw == x && yh == y)
                        {
                            // The parent
                            _gridArray[xw, yh].SetValue(value, width, height);                            
                        }
                        else
                        {
                            _gridArray[xw, yh].SetParent(parent);                            
                        }
                        
                    }
                }

                // Invoke the set value event
                _setValueEvent.Invoke(x, y, value);
                return true;
            }
            return false;
        }


        public bool SetValue(Vector3 worldPosition, T value)
        {
            return SetValue(worldPosition, value, out _, out _);
        }

        public bool SetValue(Vector3 worldPosition, int width, int height, T value)
        {
            return SetValue(worldPosition, width, height, value, out _, out _);
        }

        public bool SetValue(Vector3 worldPosition, T value, out int x, out int y)
        {
            return SetValue(worldPosition, 1, 1, value, out x, out y);
        }

        public bool SetValue(Vector3 worldPosition, int width, int height, T value, out int x, out int y)
        {
            if (GetXY(worldPosition, out x, out y))
            {
                return SetValue(x, y, width, height, value);
            }
            return false;
        }




        public T GetValue(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _gridArray[x, y].Value;
            }
            return default;
        }

        public T GetValue(Vector3 worldPosition)
        {
            if (GetXY(worldPosition, out int x, out int y))
            {
                return GetValue(x, y);
            }
            return default;

        }

        public bool RemoveValue(int x, int y, out T value)
        {
            if (IsValidPosition(x, y))
            {
                var cell = _gridArray[x, y];
                return cell.RemoveValue(out value);
                /*
                if (!IsNullValue(value))
                {
                    _gridArray[x, y] = default;
                    return true;
                }
                */
            }
            value = default;
            return false;
        }

        public bool RemoveValue(Vector3 worldPosition, out T value)
        {
            if (GetXY(worldPosition, out var x, out var y))
            {
                return RemoveValue(x, y, out value);
            }
            value = default;
            return false;
        }

        #endregion

        #region Editor Methods

        public static void DrawGizmos(GridSettings settings, Vector3 position, Color color)
        {
            Gizmos.color = color;


            var gridSize = settings.GridSize;
            var wireCellSize = settings.WireCellSize();

            for (var x = 0; x < gridSize.x; x++)
            {
                for (var y = 0; y < gridSize.y; y++)
                {
                    var p = settings.GetWorldPosition(position, x, y);
                    Gizmos.DrawWireCube(p, wireCellSize);
                }
            }

        }

        public static void OnValidate(GridSettings settings, BoxCollider collider)
        {
            var offset = settings.Offset();
            var size = new Vector3(settings.Width, settings.Height);
            var center = size * (settings.CellSize / 2);
            collider.center = settings.GridOrientation.Translate(center - offset);
            collider.size = settings.GridOrientation.Translate(size) * settings.CellSize;
        }

        #endregion

    }




}
