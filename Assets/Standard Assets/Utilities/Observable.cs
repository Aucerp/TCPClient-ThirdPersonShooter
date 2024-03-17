using System;
using System.Collections.Generic;
public class Observable<T>
{
    private T value;
    public event Action<T> OnValueChange;

    public T Value
    {
        get { return value; }
        set
        {
            if (!EqualityComparer<T>.Default.Equals(this.value, value))
            {
                this.value = value;
                NotifyValueChange();
            }
        }
    }

    private void NotifyValueChange()
    {
        OnValueChange?.Invoke(value);
    }
}
