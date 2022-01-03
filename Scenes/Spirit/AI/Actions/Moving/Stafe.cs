using Godot;
using System.Collections.Generic;

public class Strafe : State {
    public Strafe(StateMachine _parent) : base(_parent) {
        name = STATE.STRAFE;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 8f, 16f);
    }
    public override void Exit() {
        base.Exit();
    }
}