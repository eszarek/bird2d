using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics;
using System;


public class Hook : MonoBehaviour
{
    public Transform hookedTransfrom;

    private Camera mainCamera;
    private Collider2D coll;

    private int length;
    private int strength;
    private int birdCount;

    private bool canMove;

    private List<Bird> hookedFishes;

    private Tweener cameraTween;
    private SpriteRenderer hookRenderer;

    void Awake () {
        mainCamera = Camera.main;
        coll = GetComponent<Collider2D>();
        hookedFishes = new List<Bird>();
        hookRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && Input.GetMouseButton(0))
        {
            Vector3 vector = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position;
            position.x = vector.x;
            transform.position = position;
        }
    }

    public void StartBirding()
    {
        length = IdleManager.instance.length - 20;
        strength = IdleManager.instance.strength;
        birdCount = 0;
        float time = (-length) * 0.1f;

        cameraTween = mainCamera.transform.DOMoveY(length, 1 + time * 0.25f, false).OnUpdate(delegate
        {
            if (mainCamera.transform.position.y <= -11)
                transform.SetParent(mainCamera.transform);
        }).OnComplete(delegate
        {
            coll.enabled = true;
            cameraTween = mainCamera.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            {
                if (mainCamera.transform.position.y >= -25f)
                    StopFishing();
            });
        });

        ScreensManager.instance.ChangeScreen(Screens.GAME);
        coll.enabled = false;
        canMove = true;
        hookedFishes.Clear();
    }

    void StopFishing()
    {
        canMove = false;
        cameraTween.Kill(false);
        cameraTween = mainCamera.transform.DOMoveY(0, 2, false).OnUpdate(delegate
        {
            if (mainCamera.transform.position.y >= -11)
            {
                transform.SetParent(null);
                transform.position = new Vector2(transform.position.x, -6);
            }
        }).OnComplete(delegate
        {
            transform.position = Vector2.down * 6;
            coll.enabled = true;
            int num = 0;
            for(int i = 0; i < hookedFishes.Count; i++)
            {
                hookedFishes[i].transform.SetParent(null);
                hookedFishes[i].ResetBird();
                num += hookedFishes[i].Type.price;
            }
            IdleManager.instance.totalGain = num;
            ScreensManager.instance.ChangeScreen(Screens.END);
        });
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if(target.CompareTag("Bird") && birdCount != strength)
        {
            birdCount++;
            Bird component = target.GetComponent<Bird>();
            component.Hooked();
            hookedFishes.Add(component);
            target.transform.SetParent(transform);
            // Move bird to a slightly spread-out position in the center
            Vector3 spreadOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.5f, 0.5f), 0);
            target.transform.DOMove(hookedTransfrom.position + spreadOffset, 1f).OnComplete(() =>
            {
                component.StartFloating(); // Start floating after reaching position
            });
            FlashHook();
            if (birdCount == strength)
                StopFishing();
        }
    }    
        public void StartFloating()
    {
        float floatDistance = 1f; // How far left/right to float
        float floatDuration = UnityEngine.Random.Range(2f, 4f); // Randomize duration slightly

        transform.DOLocalMoveX(transform.localPosition.x + floatDistance, floatDuration)
            .SetLoops(-1, LoopType.Yoyo) // Repeat forever
            .SetEase(Ease.InOutSine);
    }

    private void FlashHook()
    {
        if (hookRenderer != null)
        {
            Color originalColor = hookRenderer.color;
            hookRenderer.DOColor(Color.white * 1.5f, 0.1f) // Brighten effect
                .OnComplete(() => hookRenderer.DOColor(originalColor, 0.1f));
        }
    }

}

