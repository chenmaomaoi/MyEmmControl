using System;

namespace MyEmmControl
{
    public partial class MyXbox
    {
        public class StickValue
        {
            private bool useFilter = false;

            public StickValue() { }

            public StickValue(FilterKalman filterX, FilterKalman filterY, bool useFilter = true)
            {
                this.filterX = filterX;
                this.filterY = filterY;
                this.useFilter = useFilter;
            }

            public event EventHandler<StickValue> StickValueChanged;

            private short x;
            private readonly FilterKalman filterX;
            public short X
            {
                get => x;
                set
                {
                    if (useFilter)
                    {
                        short tmp = (short)filterX.filter(value);
                        if (x == tmp) return;
                        x = tmp;
                    }
                    else
                    {
                        if (x == value) return;
                        x = value;
                    }
                    StickValueChanged?.Invoke(this, this);
                }
            }

            private short y;
            private readonly FilterKalman filterY;
            public short Y
            {
                get => y;
                set
                {
                    if (useFilter)
                    {
                        short tmp = (short)filterY.filter(value);
                        if (y == tmp) return;
                        y = tmp;
                    }
                    else
                    {
                        if (y == value) return;
                        y = value;
                    }
                    StickValueChanged?.Invoke(this, this);
                }
            }
        }
    }
}
