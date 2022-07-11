using Godot;
using System.Collections.Generic;

public class Reserved : State {
    public Reserved(StateMachine _parent) : base(_parent) {
        name = STATE.RESERVED;
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