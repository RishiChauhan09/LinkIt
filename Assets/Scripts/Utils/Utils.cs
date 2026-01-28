using DG.Tweening;
using UnityEngine;

public static class Utils {

    public static Tween DoGradient(this UnityEngine.UI.Extensions.Gradient gradient, UnityEngine.UI.Extensions.Gradient toGradient, float duration) {

        Color vertex1Color = gradient.Vertex1;
        Color vertex2Color = gradient.Vertex2;

        Color targetVertex1 = toGradient.Vertex1;
        Color targetVertex2 = toGradient.Vertex2;

        

        return DOTween.To(() => 0f,
            t => {
                gradient.Vertex1 = Color.Lerp(vertex1Color, targetVertex1, t);
                gradient.Vertex2 = Color.Lerp(vertex2Color, targetVertex2, t);
            }, 1f, duration);
    }

}