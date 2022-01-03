using Godot;
using System.Collections.Generic;

public class Idle : State {
    public Idle(StateMachine _parent) : base(_parent) {
        name = STATE.IDLE;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        self.move.yVelocity = 0;
        if(seconds > 3) {
            succeed();
        }
    }
    public override void Exit() {
        base.Exit();
    }
}