using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Collections;

public class Tile : MonoBehaviour
{
    public Type type;
    public Vector2Int TilePosition { get; private set; }
    public bool Alive {get; private set;}

    private float AnimationSpeed => GameManager.Instance.AnimationSpeed;

    private TileGrid.FromTilePositionToLocalPositionConverter converter;
    private Action<Tile> OnClick;
    Action<Tile> OnMovementStart;
    Action<Tile> OnMovementEnd;

    public void SetCallbacks(TileGrid.FromTilePositionToLocalPositionConverter converter, Action<Tile> OnClick, Action<Tile> OnMovementStart, Action<Tile> OnMovementEnd)
    {
        this.converter = converter;
        this.OnClick = OnClick;
        this.OnMovementStart = OnMovementStart;
        this.OnMovementEnd = OnMovementEnd;
        
    }

    public void Init()
    {
        gameObject.SetActive(true);
        Alive = true;
    }

    public void Kill()
    {
        TilePosition = -Vector2Int.one;
        transform.DOKill();
        gameObject.SetActive(false);
        Alive = false;
    }

    public void GoTo(int x, int y, bool skipAnimation = false)
    {
        TilePosition = new(x, y);
        var endPosition = converter(TilePosition);
        
        if (skipAnimation) transform.position = endPosition;
        else transform.DOLocalMove(endPosition, (transform.localPosition.y - endPosition.y) / AnimationSpeed).SetEase(Ease.Linear).OnStart(() => OnMovementStart(this)).OnComplete(() => OnMovementEnd(this));
    }

    private void OnMouseDown()
    {
        OnClick(this);
    }

    public enum Type
    {
        RED,
        GREEN,
        BLUE,
        YELLOW
    }
}
