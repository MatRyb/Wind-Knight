using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSpriteChange : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite ground;
    public Sprite wall;

    private void OnValidate()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            if (transform.parent.localScale.x > 1)
            {
                spriteRenderer.sprite = ground;
            }
            else if (transform.parent.localScale.y > 1)
            {
                spriteRenderer.sprite = wall;
            }
        }
    }
}
