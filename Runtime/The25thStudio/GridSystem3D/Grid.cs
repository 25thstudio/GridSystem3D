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
        private readonly T[,] _gridArray;
        private readonly UnityEvent<int, int, T> _setValueEvent;

        public Grid(GridSettings settings, Vector3 originPosition, Func<int, int, T> initializeGrid = default)
        {
            this._settings = settings;            
            this._originPosition = originPosition;
            this._gridArray = new T[_settings.Width, _settings.Height];
            this._setValueEvent = new UnityEvent<int, int, T>();


            if (initializeGrid != default) 
            {
                InitializeGridArray(initializeGrid);
            }
            
        }

        private void InitializeGridArray(Func<int, int, T> createGridObject = default)
        {
            bool validFunction = createGridObject != default;
            for(var x = 0; x < _settings.Width; x++)
            {
                for (var y = 0; y < _settings.Height; y++)
                {
                    _gridArray[x,y] = validFunction ? createGridObject(x, y) : default;
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
            if (IsValidPosition(x, y))
            {
                var value = _gridArray[x, y];
                return IsNullValue(value);
            }
            return true;
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
        private bool IsNullValue(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }

        public void SetValue(int x, int y, T value)
        {
            if (IsValidPosition(x,y))
            {
                _gridArray[x, y] = value;
                // Invoke the set value event
                _setValueEvent.Invoke(x, y, value);
            }
        }

        public void SetValue(Vector3 worldPosition, T value)
        {            
            if (GetXY(worldPosition, out int x, out int y))
            {
                SetValue(x, y, value);
            }
        }


        public T GetValue(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _gridArray[x, y];
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
                value = _gridArray[x, y];
                if (!IsNullValue(value))
                {
                    _gridArray[x, y] = default;
                    return true;
                }
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
