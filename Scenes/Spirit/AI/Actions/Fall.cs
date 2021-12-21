using Godot;
using System.Collections.Generic;

public class Drop : State {
    public Drop(StateMachine _parent) : base(_parent) {
        name = STATE.DROP;
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