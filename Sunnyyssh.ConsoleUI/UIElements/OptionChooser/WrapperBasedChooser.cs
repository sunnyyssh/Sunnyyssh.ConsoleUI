﻿namespace Sunnyyssh.ConsoleUI;

public abstract class WrapperBasedChooser<TWrapper> : OptionChooser
    where TWrapper : Wrapper
{
    protected TWrapper OptionsWrapper { get; }
    
    protected override DrawState CreateDrawState(int width, int height)
    {
        return OptionsWrapper.RequestDrawState(DrawOptions.Empty);
    }

    private void RedrawWrapper(UIElement sender, RedrawElementEventArgs args)
    {
        Redraw(CreateDrawState(Width, Height));
    }

    private void SubscribeWrapper(TWrapper wrapper)
    {
        wrapper.RedrawElement += RedrawWrapper;
    }

    protected WrapperBasedChooser(int width, int height, TWrapper optionsWrapper, 
        OptionElement[] orderedOptions, OptionChooserKeySet keySet, bool canChooseOnlyOne, OverlappingPriority priority) 
        : base(width, height, orderedOptions, keySet, canChooseOnlyOne, priority)
    {
        ArgumentNullException.ThrowIfNull(optionsWrapper, nameof(optionsWrapper));
        ArgumentNullException.ThrowIfNull(orderedOptions, nameof(orderedOptions));
        ArgumentNullException.ThrowIfNull(keySet, nameof(keySet));
        
        OptionsWrapper = optionsWrapper;

        SubscribeWrapper(optionsWrapper);
    }
}