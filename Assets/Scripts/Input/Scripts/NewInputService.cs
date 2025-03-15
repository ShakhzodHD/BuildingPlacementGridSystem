using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
    public class NewInputService : IInputService
    {
        private readonly GameInputActions inputActions;

        public NewInputService()
        {
            inputActions = new GameInputActions();
            inputActions.Gameplay.Enable();
        }

        public Vector2 MousePosition
        {
            get => inputActions.Gameplay.MousePos.ReadValue<Vector2>();
        }

        public bool IsLeftClickPressed
        {
            get
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return false;
                }
                return inputActions.Gameplay.LeftClick.IsPressed();
            }
        }
    }
}