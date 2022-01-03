using Godot;
using System.Collections.Generic;

public class Run : State {
    public Run(StateMachine _parent) : base(_parent) {
        name = STATE.RUN;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 20f, 25f); //May eventually be limited on rotation.
    }
    public override void Exit() {
        base.Exit();
    }
}