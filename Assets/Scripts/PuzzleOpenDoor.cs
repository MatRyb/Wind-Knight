using UnityEngine;

public class PuzzleOpenDoor : IPuzzleSolvedEvent
{
    [SerializeField] private Transform door = null;

    [SerializeField] private Vector3 closedPosition = Vector3.zero;
    [SerializeField] private Vector3 openPosition = Vector3.zero;

    [SerializeField] private float openTimeAnimation = 0.0f;
    [SerializeField] private bool playing = false;
    private int anim_id = 0;

    public override void Solved()
    {
        if (door == null)
            return;

        if (playing)
            LeanTween.cancel(anim_id);
        playing = true;
        float time = (Vector3.Distance(door.position, openPosition) / Vector3.Distance(closedPosition, openPosition)) * openTimeAnimation;
        anim_id = LeanTween.move(door.gameObject, openPosition, time).setOnComplete(() => { playing = false; door.position = openPosition; }).id;
    }

    public override void Unsolved()
    {
        if (door == null)
            return;

        if (playing)
            LeanTween.cancel(anim_id);
        playing = true;
        float time = (Vector3.Distance(door.position, closedPosition) / Vector3.Distance(openPosition, closedPosition)) * openTimeAnimation;
        anim_id = LeanTween.move(door.gameObject, closedPosition, time).setOnComplete(() => { playing = false; door.position = closedPosition; }).id;
    }
}
