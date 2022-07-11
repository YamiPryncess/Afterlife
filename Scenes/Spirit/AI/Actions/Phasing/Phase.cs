using Godot;
using System.Collections.Generic;

public class Phase : State {
    public Phase(StateMachine _parent) : base(_parent) {
        name = STATE.PHASE;
    }
    public override void Enter() {
        base.Enter();
        self.move.gravity = -.05f;
    }
    public override void Update() {
        base.Update();
        //self.move.calcMove(self.move.inputDir, self.move.inputDir, 16f, 16f);
    }
    public override void Exit() {
        base.Exit();
    }
}