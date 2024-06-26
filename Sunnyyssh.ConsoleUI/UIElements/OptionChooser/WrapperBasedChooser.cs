﻿// Developed by Bulat Bagaviev (@sunnyyssh).
// This file is licensed to you under the MIT license.

namespace Sunnyyssh.ConsoleUI;

public abstract class WrapperBasedChooser<TWrapper> : OptionChooser
    where TWrapper : Wrapper
{
    protected TWrapper OptionsWrapper { get; }
    
    protected override DrawState CreateDrawState()
    {
        OptionsWrapper.OnDraw();
        return OptionsWrapper.RequestDrawState(DrawOptions.Empty);
    }

    private void RedrawWrapper(UIElement sender, RedrawElementEventArgs args)
    {
        Redraw(CreateDrawState());
    }

    private void SubscribeWrapper(TWrapper wrapper)
    {
        wrapper.RedrawElement += RedrawWrapper;
    }

    protected WrapperBasedChooser(int width, int height, TWrapper optionsWrapper, 
        IReadOnlyList<OptionElement> orderedOptions, OptionChooserKeySet keySet, bool canChooseOnlyOne, OverlappingPriority priority) 
        : base(width, height, orderedOptions, keySet, canChooseOnlyOne, priority)
    {
        ArgumentNullException.ThrowIfNull(optionsWrapper, nameof(optionsWrapper));
        ArgumentNullException.ThrowIfNull(orderedOptions, nameof(orderedOptions));
        ArgumentNullException.ThrowIfNull(keySet, nameof(keySet));
        
        OptionsWrapper = optionsWrapper;

        SubscribeWrapper(optionsWrapper);
    }
}