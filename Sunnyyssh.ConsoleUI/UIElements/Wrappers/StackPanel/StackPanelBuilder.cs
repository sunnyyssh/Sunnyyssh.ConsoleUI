﻿namespace Sunnyyssh.ConsoleUI;

public sealed class StackPanelBuilder : IUIElementBuilder<StackPanel>
{
    private static readonly ConsoleKeyCollection DefaultVerticalNextKeys =
        new[] { ConsoleKey.Tab, ConsoleKey.DownArrow }.ToCollection();
    
    private static readonly ConsoleKeyCollection DefaultVerticalPreviousKeys =
        new[] { ConsoleKey.UpArrow }.ToCollection();
    
    private static readonly ConsoleKeyCollection DefaultHorizontalNextKeys =
        new[] { ConsoleKey.Tab, ConsoleKey.RightArrow }.ToCollection();
    
    private static readonly ConsoleKeyCollection DefaultHorizontalPreviousKeys =
        new[] { ConsoleKey.LeftArrow }.ToCollection();
    
    private readonly List<QueuedChildWithOffset> _orderedQueuedChildren = new();

    public Size Size { get; }

    public bool FocusFlowLoop { get; init; } = false;

    public bool OverridesFocusFlow { get; init; } = true;
    
    public ConsoleKeyCollection? FocusNextKeys { get; init; }
    
    public ConsoleKeyCollection? FocusPreviousKeys { get; init; }

    public ConsoleKeyCollection FocusChangeKeys { get; init; } = new [] { ConsoleKey.Tab }.ToCollection();
    
    public Orientation Orientation { get; }

    public OverlappingPriority OverlappingPriority { get; init; } = OverlappingPriority.Medium;

    public StackPanelBuilder Add(IUIElementBuilder builder, int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));
        
        _orderedQueuedChildren.Add(new QueuedChildWithOffset(builder, offset));

        return this;
    }

    public StackPanel Build(UIElementBuildArgs args)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));
        
        int width = args.Width;
        int height = args.Height;

        var buildedChildren = Orientation switch
        {
            Orientation.Vertical => 
                BuildVertical(width, height),
            
            Orientation.Horizontal => 
                BuildHorizontal(width, height),
            
            _ => throw new ArgumentOutOfRangeException()
        };

        var focusFlowSpecification = InitializeFocusSpecification(buildedChildren);
        
        var resultStackPanel = new StackPanel(width, height, buildedChildren, 
            Orientation, focusFlowSpecification, OverlappingPriority);

        return resultStackPanel;
    }

    private FocusFlowSpecification InitializeFocusSpecification(ChildrenCollection orderedChildren)
    {
        var nextKeys = FocusNextKeys ??
                       (Orientation == Orientation.Horizontal
                           ? DefaultHorizontalNextKeys
                           : DefaultVerticalNextKeys);

        var prevKeys = FocusPreviousKeys ??
                       (Orientation == Orientation.Horizontal
                           ? DefaultHorizontalPreviousKeys
                           : DefaultVerticalPreviousKeys);
        
        var focusables = orderedChildren
            .Where(child => child.Child is IFocusable)
            .Select(child => (IFocusable)child.Child)
            .ToArray();

        return InitializeFocusSpecification(focusables, prevKeys, nextKeys);
    }

    private FocusFlowSpecification InitializeFocusSpecification(IFocusable[] focusables, ConsoleKeyCollection prevKeys,
        ConsoleKeyCollection nextKeys)
    {
        var specBuilder = new FocusFlowSpecificationBuilder(OverridesFocusFlow);
        
        if (focusables.Length <= 1)
        {
            if (focusables.Length == 1)
            {
                specBuilder.Add(focusables[0])
                    .AddLoseFocus(focusables[0], nextKeys);
            }
            
            return specBuilder.Build();
        }

        foreach (var focusable in focusables)
        {
            specBuilder.Add(focusable);
        }

        for (int i = 0; i < focusables.Length - 1; i++)
        {
            specBuilder.AddFlow(focusables[i], focusables[i + 1], nextKeys)
                .AddFlow(focusables[i + 1], focusables[i], prevKeys);
        }

        if (FocusFlowLoop)
        {
            specBuilder.AddFlow(focusables[^1], focusables[0], nextKeys)
                .AddFlow(focusables[0], focusables[^1], prevKeys);
        }
        else
        {
            specBuilder.AddLoseFocus(focusables[^1], nextKeys)
                .AddLoseFocus(focusables[0], prevKeys);
        }
        
        return specBuilder.Build();
    }

    UIElement IUIElementBuilder.Build(UIElementBuildArgs args) => Build(args);

    private ChildrenCollection BuildHorizontal(int width, int height)
    {
        var placer = new ElementsFieldBuilder(width, height, 
            // false because StackPanel should contain children sequentially.
            false);

        int accumulatedLeft = 0;
        int absoluteTop = 0;

        foreach (var queuedChild in _orderedQueuedChildren)
        {
            accumulatedLeft += queuedChild.Offset;
            var position = new Position(accumulatedLeft, absoluteTop);
            
            ChildInfo childInfo;
            
            placer.Place(queuedChild.Builder, position, out childInfo);

            accumulatedLeft += childInfo.Width;
        }

        return placer.Build();
    }

    private ChildrenCollection BuildVertical(int width, int height)
    {
        var placer = new ElementsFieldBuilder(width, height, 
            // false because StackPanel should contain children sequentially.
            false);
        
        int absoluteLeft = 0;
        int accumulatedTop = 0;

        foreach (var queuedChild in _orderedQueuedChildren)
        {
            accumulatedTop += queuedChild.Offset;
            var position = new Position(absoluteLeft, accumulatedTop);
            
            ChildInfo childInfo;
            
            placer.Place(queuedChild.Builder, position, out childInfo);
            
            accumulatedTop += childInfo.Height;
        }

        return placer.Build();
    }

    public StackPanelBuilder(int width, int height, Orientation orientation)
        : this(new Size(width, height), orientation)
    { }
    
    public StackPanelBuilder(int width, double heightRelation, Orientation orientation)
        : this(new Size(width, heightRelation), orientation)
    { }
    
    public StackPanelBuilder(double widthRelation, int height, Orientation orientation)
        : this(new Size(widthRelation, height), orientation)
    { }
    
    public StackPanelBuilder(double widthRelation, double heightRelation, Orientation orientation)
        : this(new Size(widthRelation, heightRelation), orientation)
    { }
    
    public StackPanelBuilder(Size size, Orientation orientation)
    {
        ArgumentNullException.ThrowIfNull(size, nameof(size));
        
        Size = size;
        Orientation = orientation;
    }
    
    private class QueuedChildWithOffset : QueuedChild
    {
        public int Offset { get; }

        public QueuedChildWithOffset(IUIElementBuilder builder, int offset) : base(builder)
        {
            Offset = offset;
        }
    }
}