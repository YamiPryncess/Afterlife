using Godot;
using System.Collections.Generic;

public class Crouch : State {
    Movement move;
    public Crouch(StateMachine _parent) : base(_parent) {
        name = STATE.CROUCH;
    }
    public override void Enter() {
        base.Enter();
        move = self.move;
    }
    public override void Update() {
        base.Update();
    }
    public override void Exit() {
        base.Exit();
    }
}