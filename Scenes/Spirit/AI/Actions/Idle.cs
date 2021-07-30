using Godot;
using System.Collections.Generic;

public class Idle : StateMachine {
    public Idle(Spirit _player) {
        name = STATE.IDLE;
        base.player = _player;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        player.velocity = new Vector3(0, player.velocity.y, 0);
    }
    public override void Exit() {
        base.Exit();
    }
}