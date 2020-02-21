using UnityEngine;

public interface IMovement {
    void Update();
    void Jump();
    void MoveTo(Vector2 to);
}