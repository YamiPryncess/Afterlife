using Godot;
using System.Collections.Generic;

public class Float : State {
    public Float(StateMachine _parent) : base(_parent) {
        name = STATE.FLOAT;
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