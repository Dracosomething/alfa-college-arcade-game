using UnityEngine;

public static class Vector2Extensions
{
    public static bool IsDiagonal(this Vector2 self) =>
        self.x != 0 && self.y != 0;
}