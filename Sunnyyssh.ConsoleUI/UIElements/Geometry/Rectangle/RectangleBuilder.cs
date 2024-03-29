﻿namespace Sunnyyssh.ConsoleUI;

public sealed class RectangleBuilder : IUIElementBuilder<Rectangle>
{
    public Size Size { get; }
    
    public Color Color { get; }

    public OverlappingPriority OverlappingPriority { get; init; } = OverlappingPriority.Medium;

    public Rectangle Build(UIElementBuildArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        var result = new Rectangle(args.Width, args.Height, OverlappingPriority)
        {
            Color = Color
        };

        return result;
    }

    UIElement IUIElementBuilder.Build(UIElementBuildArgs args) => Build(args);

    public RectangleBuilder(Size size, Color color)
    {
        ArgumentNullException.ThrowIfNull(size, nameof(size));
        
        Size = size;
        Color = color;
    }
}