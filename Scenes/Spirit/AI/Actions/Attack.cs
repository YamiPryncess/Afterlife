using Godot;
using System.Collections.Generic;

public class Attack : State {
    public Attack(StateMachine _parent) : base(_parent) {
        name = STATE.ATTACK;
    }
    public override void Enter() {
        base.Enter();
        animator.Play("Attack_1");
    }
    public override void Update() {
        base.Update();
    }
    public override void Exit() {
        base.Exit();
    }
}