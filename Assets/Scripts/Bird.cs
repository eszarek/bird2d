using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Bird : MonoBehaviour
{
    private BirdType type;
    private CircleCollider2D coll;
    private float screenLeft;
    private Tweener tweener;
    private SpriteRenderer birdRenderer;
    public BirdType Type
    {
        get { return type; }
        set
        {
            type = value;
            coll.radius = type.collierRadius;
        }
    }

    void Awake()
    {
        coll = GetComponent<CircleCollider2D>();
        screenLeft = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        birdRenderer = GetComponent<SpriteRenderer>();
    }

    public void ResetBird()
    {
        if (tweener != null)
            tweener.Kill(false);

        float spawnHeight = UnityEngine.Random.Range(type.minLenght, type.maxLenght);
        coll.enabled = true;

        Vector3 position = transform.position;
        position.y = spawnHeight;
        position.x = screenLeft;
        transform.position = position;

        float heightVariation = 1f;
        float targetY = UnityEngine.Random.Range(spawnHeight - heightVariation, spawnHeight + heightVariation);
        Vector2 targetPos = new Vector2(-position.x, targetY);

        float moveDuration = 3f;
        float delay = UnityEngine.Random.Range(0, 2 * moveDuration);
        tweener = transform.DOMove(targetPos, moveDuration, false)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear)
            .SetDelay(delay)
            .OnStepComplete(() =>
            {
                Vector3 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
            });
    }

    public void Hooked()
    {
        coll.enabled = false;
        if (tweener != null) tweener.Kill(false);
        
        
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("takeoff"); // Play Takeoff animation
            FlashBird();
            // Wait for Takeoff to finish before transitioning to Flap
            StartCoroutine(TransitionToFlap(animator));
        }

    }
    // Coroutine to transition from Takeoff to Flap
    public IEnumerator TransitionToFlap(Animator animator)
    {
        // Wait until "Takeoff" animation finishes playing
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Transition to "Flap" after the "Takeoff" animation is done
        animator.Play("flap"); // Transition to Flap
        StartFloating();
    }
    public void StartFloating()
    {
        float floatDistance = 1f; // How far left/right to float
        float floatDuration = UnityEngine.Random.Range(2f, 4f); // Randomize duration slightly

        transform.DOLocalMoveX(transform.localPosition.x + floatDistance, floatDuration)
            .SetLoops(-1, LoopType.Yoyo) // Repeat forever
            .SetEase(Ease.InOutSine);
    }


    private void FlashBird()
    {
        if (birdRenderer == null)
        {
            Debug.LogError("birdRenderer is null! Make sure the Bird has a SpriteRenderer.");
            return;
        }

        Debug.Log("Flashing bird!");

        Color originalColor = birdRenderer.color;
        birdRenderer.color = Color.red; // Bright white effect
        DOVirtual.DelayedCall(0.5f, () => birdRenderer.color = originalColor); // Revert after  sec
    }




    [Serializable]
    public class BirdType
    {
        public int price;
        public float BirdCount;
        public float minLenght;
        public float maxLenght;
        public float collierRadius;
    }
}
