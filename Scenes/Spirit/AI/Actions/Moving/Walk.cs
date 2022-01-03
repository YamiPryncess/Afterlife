using Godot;
using System.Collections.Generic;

public class Walk : State {
    public Walk(StateMachine _parent) : base(_parent) {
        name = STATE.WALK;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 16f, 16f);
    }
    public override void Exit() {
        base.Exit();
    }
}