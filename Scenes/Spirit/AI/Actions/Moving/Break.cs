using Godot;
using System.Collections.Generic;

public class Break : State {
    public Break(StateMachine _parent) : base(_parent) {
        name = STATE.BREAK;
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