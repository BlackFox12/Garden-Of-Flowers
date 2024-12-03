using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class Explosion : MonoBehaviour
{
    public SpriteRenderer start;
    public SpriteRenderer middle;
    public SpriteRenderer end;

    public void SetActiveRenderer(SpriteRenderer renderer)
    {
        start.enabled = renderer == start;
        middle.enabled = renderer == middle;
        end.enabled = renderer == end;
    }
    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
