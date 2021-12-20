using Godot;
using System.Collections.Generic;

public class Attack : State {
    public Attack(StateMachine _parent) : base(_parent) {
        name = STATE.ATTACK;
    }
    public override void Enter() {
        base.Enter();
        animator.Play("Attack_1");
        self.GetNode<Catalyst>("Catalyst").attackBool = true;
        if(self.reality.target != null) 
            self.move.rotateDir = self.reality.target.GlobalTransform.origin - self.GlobalTransform.origin;
        //self.move.maxSpeed = 10;
    }
    public override void Update() {
        base.Update();
        succeed();//succeed for now but later requires callback.
    }
    public override void Exit() {
        self.GetNode<Catalyst>("Catalyst").attackBool = false;
        //self.move.maxSpeed = 16;
        base.Exit();
    }
}