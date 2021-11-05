using Godot;
using System.Collections.Generic;

public class Attack : State {
    public Attack(StateMachine _parent) : base(_parent) {
        name = STATE.ATTACK;
    }
    public override void Enter() {
        base.Enter();
        animator.Play("Attack_1");
        if(self.reality.target != null) 
            self.rotateDir = self.reality.target.GlobalTransform.origin - self.GlobalTransform.origin;
    }
    public override void Update() {
        base.Update();
        succeed();//succeed for now but later requires callback.
    }
    public override void Exit() {
        base.Exit();
    }
}