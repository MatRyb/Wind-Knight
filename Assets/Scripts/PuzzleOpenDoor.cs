using UnityEngine;
using NaughtyAttributes;

public class PuzzleOpenDoor : IPuzzleSolvedEvent
{
    [SerializeField] private Transform door = null;

    [SerializeField] private Vector3 closedPosition = Vector3.zero;
    [SerializeField] private Vector3 openPosition = Vector3.zero;

    [SerializeField] private Vector3 closedScale = Vector3.zero;
    [SerializeField] private Vector3 openScale = Vector3.zero;

    [SerializeField] private float openTimeAnimation = 0.0f;
    [SerializeField] private bool playing = false;
    private int transform_anim_id = 0;
    private int scale_anim_id = 0;

    public override void Solved()
    {
        if (door == null)
            return;

        if (playing)
        {
            LeanTween.cancel(transform_anim_id);
            LeanTween.cancel(scale_anim_id);
        }

        playing = true;

        float scale_time = (Vector3.Distance(door.localScale, openScale) / Vector3.Distance(closedScale, openScale)) * openTimeAnimation;
        float transform_time = (Vector3.Distance(door.position, openPosition) / Vector3.Distance(closedPosition, openPosition)) * openTimeAnimation;

        float time = scale_time > transform_time ? (float.IsInfinity(scale_time) ? transform_time : scale_time) : (float.IsInfinity(transform_time) ? (float.IsInfinity(scale_time) ? 0f : scale_time) : transform_time);

        transform_anim_id = LeanTween.move(door.gameObject, openPosition, time).setOnComplete(() => { playing = false; door.position = openPosition; }).id;
        scale_anim_id = LeanTween.scale(door.gameObject, openScale, time).setOnComplete(() => { playing = false; door.localScale = openScale; }).id;
    }

    public override void Unsolved()
    {
        if (door == null)
            return;

        if (playing)
        {
            LeanTween.cancel(transform_anim_id);
            LeanTween.cancel(scale_anim_id);
        }

        playing = true;

        float scale_time = (Vector3.Distance(door.localScale, closedScale) / Vector3.Distance(openScale, closedScale)) * openTimeAnimation;
        float transform_time = (Vector3.Distance(door.position, closedPosition) / Vector3.Distance(openPosition, closedPosition)) * openTimeAnimation;

        float time = scale_time > transform_time ? (float.IsInfinity(scale_time) ? transform_time : scale_time) : (float.IsInfinity(transform_time) ? (float.IsInfinity(scale_time) ? 0f : scale_time) : transform_time);

        transform_anim_id = LeanTween.move(door.gameObject, closedPosition, time).setOnComplete(() => { playing = false; door.position = closedPosition; }).id;
        scale_anim_id = LeanTween.scale(door.gameObject, closedScale, time).setOnComplete(() => { playing = false; door.localScale = closedScale; }).id;
    }
}
