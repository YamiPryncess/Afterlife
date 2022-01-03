using Godot;
using System.Collections.Generic;

public class Skip : State {
    public Skip(StateMachine _parent) : base(_parent) {
        name = STATE.SKIP;
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