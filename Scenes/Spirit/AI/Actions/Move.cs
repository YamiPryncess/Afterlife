using Godot;
using System.Collections.Generic;

public class Move : StateMachine {
    public Move(Spirit _player) {
        name = STATE.MOVE;
        player = _player;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        player.rotate(delta);
        base.Update();
    }
    public override void Exit() {
        base.Exit();
    }
}