using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteGeneratorValidation : MonoBehaviour
{
    public Vector2 startLoc = Vector2.zero;
    public SpriteRenderer sprite;
    public Vector2 spriteSize = Vector2.zero;

    private void OnValidate()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        if (sprite != null)
        {
            if (startLoc == Vector2.zero)
            {
                startLoc = new Vector2(transform.localScale.x, transform.localScale.y);
            }

            if (spriteSize == Vector2.zero)
            {
                spriteSize = sprite.size;
            }

            transform.localScale = new Vector3(startLoc.x / transform.parent.localScale.x, startLoc.y / transform.parent.localScale.y, transform.localScale.z);
            sprite.size = new Vector2(spriteSize.x * transform.parent.localScale.x, spriteSize.y * transform.parent.localScale.y);
        }
    }
}
