using System.Collections.Generic;


namespace The25thStudio.GridSystem3D
{
    public class GridCell<T>
    {
        private T _value;
        private int _width;
        private int _height;
        private GridCell<T> _parent;
        private readonly List<GridCell<T>> _children;

        public GridCell(T value, int width, int height, GridCell<T> parent = default)
        {
            _children = new List<GridCell<T>>();
            SetValue(value, width, height, parent);
        }

        public void SetValue(T value, int width, int height, GridCell<T> parent = default)
        {
            _value = value;
            _width = width;
            _height = height;
            _parent = parent;
        }

        public bool RemoveValue(out T value)
        {
            if (HasParent())
            {
                return _parent.RemoveValue(out value);

            }
            else if (!IsNullValue())
            {
                value = _value;
                _children.ForEach(c => c.SetValue(default, 1, 1));
                SetValue(default, 1, 1, default);
                _children.Clear();
                
                return true;
            }

            value = default;
            return false;
        }

        public void SetParent(GridCell<T> parent)
        {
            SetValue(default, 1, 1, parent);
            parent.AddChild(this);
        }

        public void AddChild(GridCell<T> child)
        {
            _children.Add(child);
        }

        public bool IsEmpty()
        {
            return !HasParent() && IsNullValue();
        }


        public bool IsNullValue()
        {
            return EqualityComparer<T>.Default.Equals(_value, default);
        }

        public T Value => HasParent() ? _parent.Value : _value;

        public override string ToString()
        {
            return $"W {_width} - H {_height} - Value {_value} - Has Parent {HasParent()} - Is Empty: {IsEmpty()}";
        }


        private bool HasParent()
        {
            return _parent != default;
        }
    }
}
