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
    }
    public override void Exit() {
        base.Exit();
    }
}