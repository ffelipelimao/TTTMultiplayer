using UnityEngine;

public class Tweener : MonoBehaviour
{
    public float delay;
    public float duration = 0.5f;

    void OnEnable()
    {
        transform.localScale = Vector3.one;

        LeanTween.scale(gameObject, new Vector3(0.3f, 0.3f, 0.3f), duration)
            .setDelay(delay)
            .setEase(LeanTweenType.easeInOutCirc)
            .setLoopPingPong();
    }
}
