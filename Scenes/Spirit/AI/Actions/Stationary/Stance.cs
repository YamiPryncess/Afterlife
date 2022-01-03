using Godot;
using System.Collections.Generic;

public class Stance : State {
    public Stance(StateMachine _parent) : base(_parent) {
        name = STATE.STANCE;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        self.move.yVelocity = 0;
        base.Update();
    }
    public override void Exit() {
        base.Exit();
    }
}