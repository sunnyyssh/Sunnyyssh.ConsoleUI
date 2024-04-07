﻿namespace Sunnyyssh.ConsoleUI;

public class BindableObject<TValue> : IBindable<TValue, ValueChangedEventArgs<TValue>>
{
    private TValue _value;

    public TValue Value
    {
        get => _value;
        set
        {
            _value = value;
            Updated?.Invoke(this, new ValueChangedEventArgs<TValue>(_value));
        }
    }

    public event UpdatedEventHandler<TValue, ValueChangedEventArgs<TValue>>? Updated;

    public void HandleUpdate(ValueChangedEventArgs<TValue> args)
    {
        _value = args.NewValue;
        BoundUpdate?.Invoke(this, new ValueChangedEventArgs<TValue>(_value));
    }

    public event UpdatedEventHandler<TValue, ValueChangedEventArgs<TValue>>? BoundUpdate;

    public BindableObject(TValue initValue)
    {
        _value = initValue;
    }
}