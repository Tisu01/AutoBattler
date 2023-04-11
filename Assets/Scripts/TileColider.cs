using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileColider : MonoBehaviour
{
    public SpriteRenderer sprite;
    public SpriteRenderer highlightSprite;
    [Range(1, 10)]
    public float highlightBlinkingSpeed = 5;
    public Node node;
    public GameManager gameManager;

    public void InitTileCollider(Node node, Vector3 cellSize, GameManager gameManager)
    {
        sprite.enabled = false;
        this.node = node;
        transform.localScale = cellSize;
        this.gameManager = gameManager;

        highlightSprite.DOFade(0, 5 / highlightBlinkingSpeed).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter TileCollider");
        sprite.enabled = true;

        gameManager.TileColliderFocused(this);
    }

    private void OnMouseExit()
    {
        Debug.Log("OnMouseExit TileCollider");
        sprite.enabled = false;


        gameManager.TileColliderUnfocesued(this);
    }
}

