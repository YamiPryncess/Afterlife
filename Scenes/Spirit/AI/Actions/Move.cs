using Godot;
using System.Collections.Generic;

public class Move : State {
    public Move(StateMachine _parent) : base(_parent) {
        name = STATE.MOVE;
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