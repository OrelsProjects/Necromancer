using System;

public enum GameState
{
    Loading,
    Playing,
    Paused,
    UI,
}

public class Game
{
    public static GameState Instance;
    private GameState _state;

    public delegate void StateChangedDelegate(GameState state);

    public event StateChangedDelegate OnStateChanged;

    public GameState State
    {
        get { return _state; }
        private set
        {
            _state = value;
            OnStateChanged?.Invoke(_state);
        }
    }

    public void SetState(GameState state) => State = state;
}

