using UnityEngine;

public interface IClimbable
{
    void OnGrab(Transform hand);
    void OnRelease();
    bool IsHolding(Transform hand);
    /// <summary>
    /// Determines if the object can be released by the player.
    /// </summary>
    /// <returns>True if the object can be let go, false otherwise.</returns>
    bool CanRelease();

    Transform transform { get; }
}