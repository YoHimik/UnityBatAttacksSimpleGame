using UnityEngine;

internal static class BezierCurve {
    internal static Vector3[] MakeQuadraticBezierCurve(int dotCount, Vector3 start, Vector3 middle, Vector3 end) {
        var curve = new Vector3[dotCount];
        for (var i = 1; i <= dotCount; i++) {
            var t = i / (float) dotCount;
            curve[i - 1] = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * middle + t * t * end;
        }
        return curve;
    }
}