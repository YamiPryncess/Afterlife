using Godot;
using System.Collections.Generic;

public class Leap : State {
    public Leap(StateMachine _parent) : base(_parent) {
        name = STATE.LEAP;
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