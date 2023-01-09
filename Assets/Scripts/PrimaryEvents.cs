using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrimaryEvents : MonoBehaviour
{
    public static bool isPaused = false;

    public class OnEscapeEventArgs : EventArgs
    {
        public Scene scene;
        public Player.PlayerState playerState;

        public OnEscapeEventArgs(Scene scene, Player.PlayerState playerState)
        {
            this.scene = scene;
            this.playerState = playerState;
        }
    }

    public static event EventHandler<OnEscapeEventArgs> Escape;

    private Player _player;

    private void Start()
    {
        _player = Manager.Instance.player;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeEvent(this, new OnEscapeEventArgs(SceneManager.GetActiveScene(), _player.GetPlayerState()));
        }
    }

    protected virtual void OnEscapeEvent(object sender, OnEscapeEventArgs e)
    {
        Escape?.Invoke(sender, e);
    }
}
