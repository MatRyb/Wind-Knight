using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrigamiHammerAnimation : IPuzzleSolvedEvent
{
    [SerializeField] private GameObject hammer;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private float startDelay;
    [SerializeField] private float toEndPosTime;
    [SerializeField] private float delayTime;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject wall;

    [SerializeField] private SpriteRenderer wallStateHandler;
    [SerializeField] private Sprite[] wallStates;

    [SerializeField] private Color destroyColor1;
    [SerializeField] private Color destroyColor2;
    [SerializeField] private ParticleSystem destroyParticle;

    private LTDescr tween;

    private bool ended = false;

    private IEnumerator Checker()
    {
        while (!ended)
        {
            if (GameTimer.TimeMultiplier == GameTimer.STOPPED && tween != null)
            {
                tween.pause();
                animator.speed = 0f;
            }
            else if (GameTimer.TimeMultiplier == GameTimer.PLAYING && tween != null)
            {
                tween.resume();
                animator.speed = 1f;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override void Solved()
    {
        StartCoroutine(Checker());
        hammer.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        hammer.GetComponent<SpriteRenderer>().sortingOrder = 0;
        hammer.GetComponent<ObjectScript>().enabled = false;
        hammer.GetComponent<SpriteMask>().enabled = false;
        animator.enabled = false;
        tween = hammer.LeanMoveLocal(endPos, toEndPosTime).setDelay(startDelay).setOnComplete(() =>
        {
            tween = hammer.LeanDelayedCall(delayTime, () => 
            {
                animator.enabled = true;
                animator.Play("HammerBreakWall");
            });
        });
    }

    private int breakWallCounter = -1;
    public void BreakWall()
    {
        breakWallCounter++;
        if (breakWallCounter == wallStates.Length)
        {
            animator.enabled = false;
            hammer.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            hammer.GetComponent<SpriteRenderer>().sortingOrder = 1;
            hammer.GetComponent<ObjectScript>().enabled = true;
            hammer.GetComponent<SpriteMask>().enabled = true;
            ParticleSystem particle = Instantiate(destroyParticle, wall.transform.position, new Quaternion(0, 0, 0, 0));
            var mainModule = particle.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(destroyColor1, destroyColor2);
            Destroy(wall);
            particle.Play();
            ended = true;
        }
        else
        {
            wallStateHandler.GetComponent<SpriteRenderer>().sprite = wallStates[breakWallCounter];
        }
    }

    public override void Unsolved()
    {
        throw new System.NotImplementedException();
    }
}
