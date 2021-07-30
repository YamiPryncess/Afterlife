using Godot;
using System.Collections.Generic;

public class Jump : StateMachine {
    public Jump(Spirit _player) {
        name = STATE.MOVE;
        player = _player;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
    }
    public override void Exit() {
        base.Exit();
    }
}