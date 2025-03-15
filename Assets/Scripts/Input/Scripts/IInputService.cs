using UnityEngine;

public interface IInputService
{
    Vector2 MousePosition { get; }
    bool IsLeftClickPressed { get; }
}