﻿// Developed by Bulat Bagaviev (@sunnyyssh).
// This file is licensed to you under the MIT license.

using System.Collections.Immutable;

namespace Sunnyyssh.ConsoleUI;

/// <summary>
/// Presents <see cref="UIElement"/> that can be one of other <see cref="UIElement"/>'s. It means you can switch states.
/// </summary>
public sealed class UIElementSwitcher : Wrapper, IFocusable
{
    public IReadOnlyList<Canvas> PresentationStates { get; }

    public int StateCount => PresentationStates.Count;
    
    public int CurrentStateIndex { get; private set; }

    public void SwitchTo(int stateIndex)
    {
        if (stateIndex < 0 || stateIndex >= PresentationStates.Count)
            throw new ArgumentOutOfRangeException(nameof(stateIndex), stateIndex, null);

        if (stateIndex == CurrentStateIndex)
            return;

        int lastIndex = CurrentStateIndex;
        CurrentStateIndex = stateIndex;
        
        PresentationStates[lastIndex].IsWaitingFocus = false;
        PresentationStates[CurrentStateIndex].IsWaitingFocus = true;
        
        TryGiveFocusTo(PresentationStates[CurrentStateIndex]);
        
        if (IsDrawn)
        {
            if (PresentationStates[lastIndex].IsDrawn)
            {
                PresentationStates[lastIndex].OnRemove();
            }
            
            Redraw(CreateDrawState());
        }
    }
    
    protected override DrawState CreateDrawState()
    {
        if (!PresentationStates[CurrentStateIndex].IsDrawn)
        {
            OnDraw();
        }

        var gotState = PresentationStates[CurrentStateIndex].RequestDrawState(new DrawOptions());
        
        // gotState could be not filled but it need to hide previous one.
        var stateBuilder = new DrawStateBuilder(Width, Height)
            .Fill(Color.Transparent)
            .Place(0, 0, gotState);
        
        return stateBuilder.ToDrawState();
    }

    private static ImmutableList<ChildInfo> ToLeftTopChildren(IReadOnlyList<UIElement> elements)
    {
        return elements
            .Select(el => new ChildInfo(el, 0, 0))
            .ToImmutableList();
    }

    private void RedrawCanvasState(UIElement sender, RedrawElementEventArgs args)
    {
        if (PresentationStates[CurrentStateIndex] != sender)
            return;
        
        Redraw(sender.CurrentState!);
    }

    private void PrepareStates(IReadOnlyList<Canvas> presentationStates)
    {
        foreach (var canvasState in presentationStates)
        {
            canvasState.RedrawElement += RedrawCanvasState;

            canvasState.IsWaitingFocus = false;
        }

        presentationStates[CurrentStateIndex].IsWaitingFocus = true;
    }

    internal UIElementSwitcher(int width, int height, ImmutableList<Canvas> presentationStates, 
        FocusFlowSpecification excludingFocusSpecification,OverlappingPriority priority) 
        : base(width, height, ToLeftTopChildren(presentationStates), excludingFocusSpecification, priority)
    {
        ArgumentNullException.ThrowIfNull(presentationStates, nameof(presentationStates));

        if (presentationStates.IsEmpty)
            throw new ArgumentException("There must be at least one state.", nameof(presentationStates));

        PresentationStates = presentationStates;
        PrepareStates(presentationStates);
    }
}