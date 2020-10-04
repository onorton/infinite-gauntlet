using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public UnityEvent<int> OnMoveUsed;
    public UnityEvent OnFightingEnding;
    public UnityEvent OnInfiniteLoopFound;

    private List<Character> _characters;

    private GridTile[,] _tiles;
    private HashSet<GridPosition> _borderPositions;

    private IDictionary<GridTile, Character> _occupiedTiles;

    private List<BoardState> _previousStates;

    private int _currentInitiative;

    private bool _actionsThisTurn;


    private Vector3 _gridOriginInWorldSpace;

    public GameObject GridTile;

    private Selector _selector;


    public int _movesLeft = 3;
    private bool _runningGauntlet = false;
    void Awake()
    {
        PositionHandler.OnAttemptedMove += UpdateMoves;
        PositionHandler.OnSelected += ShowPossibleLocations;
        PositionHandler.OnDeselected += HidePossibleLocations;
        Selectable.OnAttemptedMove += UpdateMoves;

        _selector = transform.Find("Selector")?.GetComponent<Selector>();

        var tileMap = transform.Find("Grid/Tilemap").GetComponent<Tilemap>();

        var bounds = tileMap.cellBounds;
        var cellSize = tileMap.cellSize;

        _gridOriginInWorldSpace = new Vector3(bounds.position.x + cellSize.x / 2, bounds.position.y + cellSize.y / 2, bounds.position.z);

        _characters = transform.Find("Characters").GetComponentsInChildren<Character>().ToList();

        _tiles = new GridTile[bounds.size.y, bounds.size.x];
        // positions adjacent but outside grid, needed for displaying arrows correctly
        _borderPositions = new HashSet<GridPosition>();
        for (int x = 0; x < bounds.size.x; x += 1)
        {
            for (int y = 0; y < bounds.size.y; y += 1)
            {
                var tile = Instantiate(GridTile, new Vector3(bounds.x + x * cellSize.x + cellSize.x / 2, bounds.y + y * cellSize.y + cellSize.y / 2, 0), Quaternion.identity);
                tile.transform.parent = this.transform;
                _tiles[y, x] = tile.GetComponent<GridTile>();
                _tiles[y, x].SetPosition(new GridPosition { x = x, y = y });

                _borderPositions.Add(new GridPosition { x = -1, y = y });
                _borderPositions.Add(new GridPosition { x = bounds.size.x, y = y });
            }

            _borderPositions.Add(new GridPosition { x = x, y = -1 });
            _borderPositions.Add(new GridPosition { x = x, y = bounds.size.y });
        }
        // Corners
        _borderPositions.Add(new GridPosition { x = -1, y = -1 });
        _borderPositions.Add(new GridPosition { x = -1, y = bounds.size.y });
        _borderPositions.Add(new GridPosition { x = bounds.size.x, y = -1 });
        _borderPositions.Add(new GridPosition { x = bounds.size.x, y = bounds.size.y });

        _currentInitiative = 0;
        _actionsThisTurn = true;
    }

    void Start()
    {
        OnMoveUsed.Invoke(_movesLeft);
        _previousStates = new List<BoardState>();
        _occupiedTiles = new Dictionary<GridTile, Character>();
        foreach (var c in _characters)
        {
            c.PositionHandler.Initialise(_gridOriginInWorldSpace);
            _occupiedTiles[_tiles[c.PositionHandler.GridPosition.y, c.PositionHandler.GridPosition.x]] = c;
        }
        _selector?.Initialise(_gridOriginInWorldSpace);

        foreach (var tile in _tiles)
        {
            tile.UpdateViableLocations(_occupiedTiles.Keys.Select(t => t.Position).ToList().Concat(_borderPositions));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_runningGauntlet && !_characters.Any(c => c.PlayingAnimation()))
        {
            if (_currentInitiative == 0)
            {
                _previousStates.Add(new BoardState(_characters));
                _currentInitiative = _characters.Count > 0 ? _characters.Max(c => c.Initiative) : 1;

                if (!_actionsThisTurn)
                {
                    OnFightingEnding.Invoke();
                    _runningGauntlet = false;
                    return;
                }
                else if (new HashSet<BoardState>(_previousStates).Count < _previousStates.Count)
                {
                    OnInfiniteLoopFound.Invoke();
                }
                _actionsThisTurn = false;
            }

            var actionExecuted = ProcessTurn();
            _actionsThisTurn = _actionsThisTurn || actionExecuted;
        }

    }


    public void RunGauntlet()
    {
        _runningGauntlet = true;
        PreventPlayerMoves();
    }

    private void PreventPlayerMoves()
    {
        foreach (var c in _characters)
        {
            c.PositionHandler.CanMove = false;
        }
        _selector?.PreventPlayerMoves();
    }

    private bool ProcessTurn()
    {
        var attacksExecuted = false;
        var charactersWithCurrentInitiative = _characters.Where(c => c.Initiative == _currentInitiative);


        // Carry out actions
        foreach (var c in charactersWithCurrentInitiative)
        {
            if (c.ExecuteTurn(_characters))
            {
                attacksExecuted = true;
            }
        }

        List<Character> charactersToRemove = new List<Character>();
        // Resolve effects
        foreach (var c in _characters)
        {
            if (c.IsDead())
            {
                charactersToRemove.Add(c);
            }
        }

        foreach (var c in charactersToRemove)
        {
            _characters.Remove(c);
            Destroy(c.gameObject);
        }
        _currentInitiative--;
        return attacksExecuted;
    }

    void OnDestroy()
    {
        PositionHandler.OnAttemptedMove -= UpdateMoves;
        PositionHandler.OnSelected -= ShowPossibleLocations;
        PositionHandler.OnDeselected -= HidePossibleLocations;
        Selectable.OnAttemptedMove -= UpdateMoves;
        OnMoveUsed.RemoveAllListeners();
        OnFightingEnding.RemoveAllListeners();
    }

    private void UpdateMoves(GridPosition oldPosition, GridPosition newPosition)
    {
        // Don't update if newPosition outside bounds
        if (_borderPositions.Contains(newPosition))
        {
            return;
        }

        var oldTile = _tiles[oldPosition.y, oldPosition.x];
        var newTile = _tiles[newPosition.y, newPosition.x];
        var c = _occupiedTiles[oldTile];

        // Don't update if tile occupied
        if (_occupiedTiles.ContainsKey(newTile))
        {
            return;
        }
        _occupiedTiles.Remove(oldTile);
        _occupiedTiles[newTile] = c;
        c.PositionHandler.UpdatePosition(newPosition);

        foreach (var tile in _tiles)
        {
            tile.UpdateViableLocations(_occupiedTiles.Keys.Select(t => t.Position).Concat(_borderPositions));
        }

        _movesLeft -= 1;
        OnMoveUsed.Invoke(_movesLeft);
        if (_movesLeft <= 0)
        {
            PreventPlayerMoves();
        }
    }


    private void UpdateMoves(Selectable selectable, GridPosition newPosition)
    {
        // Don't update if newPosition outside bounds
        if (_borderPositions.Contains(newPosition))
        {
            return;
        }

        var newTile = _tiles[newPosition.y, newPosition.x];

        // Don't update if tile occupied
        if (_occupiedTiles.ContainsKey(newTile))
        {
            return;
        }


        var character = GameObject.Instantiate(selectable.Character, transform.Find("Characters"));
        _occupiedTiles[newTile] = character;
        character.PositionHandler.Initialise(_gridOriginInWorldSpace);
        character.PositionHandler.UpdatePosition(newPosition);
        character.GetComponent<BoxCollider2D>().enabled = true;
        character.GetComponent<PositionHandler>().CanMove = true;
        _characters.Add(character);

        foreach (var tile in _tiles)
        {
            tile.UpdateViableLocations(_occupiedTiles.Keys.Select(t => t.Position).Concat(_borderPositions));
        }

        _movesLeft -= 1;
        OnMoveUsed.Invoke(_movesLeft);
        if (_movesLeft <= 0)
        {
            PreventPlayerMoves();
        }
    }

    private void ShowPossibleLocations(GridPosition position)
    {
        _tiles[position.y, position.x].TogglePossibleLocations(true);
    }

    private void HidePossibleLocations(GridPosition position)
    {
        _tiles[position.y, position.x].TogglePossibleLocations(false);
    }
}



