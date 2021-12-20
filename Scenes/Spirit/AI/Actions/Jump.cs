using Godot;
using System.Collections.Generic;

public class Jump : State {
    public Jump(StateMachine _parent) : base(_parent) {
        name = STATE.JUMP;
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