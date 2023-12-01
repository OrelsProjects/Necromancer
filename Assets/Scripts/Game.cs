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
    private static Game _instance;
    private GameState _state;

    public delegate void StateChangedDelegate(GameState state);

    public event StateChangedDelegate OnStateChanged;

    public void SubscribeToStateChanged(StateChangedDelegate stateChangedDelegate)
    {
        OnStateChanged += stateChangedDelegate;
    }

    public void UnsubscribeToStateChanged(StateChangedDelegate stateChangedDelegate)
    {
        OnStateChanged -= stateChangedDelegate;
    }

    // Private constructor to prevent external instantiation
    private Game() { }

    // Public method to access the singleton instance
    public static Game Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Game();
            }
            return _instance;
        }
    }

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
