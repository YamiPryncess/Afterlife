using Godot;
using System.Collections.Generic;

public class Dash : State {
    public Dash(StateMachine _parent) : base(_parent) {
        name = STATE.DASH;
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