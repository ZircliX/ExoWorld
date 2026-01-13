using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public interface IPanel
    {
        void Show();
        void Hide();
        bool IsActive { get; }
    }
    
    public interface INavigablePanel : IPanel
    {
        event Action OnBackClicked;
        void InvokeBackClicked();
    }

    public interface ISelectable
    {
        Selectable FirstSelectable { get; }
    }

    public interface IInputProvider
    {
        InputAction NavigationAction { get; }
        InputAction SubmitAction { get; }
        InputAction CancelAction { get; }
    }
}