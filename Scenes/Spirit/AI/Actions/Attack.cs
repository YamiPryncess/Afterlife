using Godot;
using System.Collections.Generic;

public class Attack : StateMachine {
    public Attack(Spirit _player) {
        name = STATE.ATTACK;
        base.player = _player;
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