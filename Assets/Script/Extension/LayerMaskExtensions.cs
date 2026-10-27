using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool IsLayer(this LayerMask self, int layer) =>
        // We first shift 1 by the layer. Then we check if self is equal to that by doing a bitwise OR.
        self.Equals(self | (1 << layer));
}