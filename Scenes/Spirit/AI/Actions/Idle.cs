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
        self.velocity = new Vector3(0, self.velocity.y, 0);
    }
    public override void Exit() {
        base.Exit();
    }
}