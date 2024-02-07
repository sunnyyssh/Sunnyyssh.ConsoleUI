﻿using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Sunnyyssh.ConsoleUI;

public sealed class DrawState
{
    private readonly InternalDrawState _internalState;
    
    public PixelLine[] Lines => _internalState.Lines;
    
    public DrawState(PixelLine[] lines) : this(new InternalDrawState(lines))
    { }

    private DrawState(InternalDrawState internalState)
    {
        _internalState = internalState;
    }

    [Pure]
    public DrawState IntersectWith(DrawState state)
    {
        return new DrawState(_internalState.IntersectWith(state._internalState));
    }

    [Pure]
    public DrawState SubtractWith(DrawState state)
    {
        return new DrawState(_internalState.SubtractWith(state._internalState));
    }

    [Pure]
    public DrawState Shift(int left, int top)
    {
        return new DrawState(_internalState.Shift(left, top));
    }

    [Pure]
    public DrawState Crop(int width, int height)
    {
        return new DrawState(_internalState.Crop(width, height));
    }
    
    [Pure]
    internal InternalDrawState ToInternal(int left, int top)
    {
        return _internalState.Shift(left, top);
    }

    [Pure]
    internal bool TryGetPixel(int left, int top, [NotNullWhen(true)] out PixelInfo? resultPixel)
    {
        return _internalState.TryGetPixel(left, top, out resultPixel);
    }
    
    public static DrawState Combine(params DrawState[] states)
    {
        var combined = InternalDrawState.Combine(
            states
                .Select(st => st._internalState)
                .ToArray()
        );
        return new DrawState(combined);
    }
}