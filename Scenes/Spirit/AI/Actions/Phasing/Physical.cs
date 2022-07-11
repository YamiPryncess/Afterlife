using Godot;
using System.Collections.Generic;

public class Physical : State {
    public Physical(StateMachine _parent) : base(_parent) {
        name = STATE.PHYSICAL;
    }
    public override void Enter() {
        base.Enter();
        self.move.gravity = -45;
    }
    public override void Update() {
        base.Update();
        //self.move.calcMove(self.move.inputDir, self.move.inputDir, 16f, 16f);
    }
    public override void Exit() {
        base.Exit();
    }
}