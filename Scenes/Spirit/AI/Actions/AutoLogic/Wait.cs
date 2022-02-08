using Godot;
using System.Collections.Generic;

public class Wait : State {
    public Wait(StateMachine _parent) : base(_parent) {
        name = STATE.WAIT;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        if(seconds > 3) {
            succeed();
        }
    }
    public override void Exit() {
        base.Exit();
    }
}