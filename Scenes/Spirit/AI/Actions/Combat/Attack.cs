using Godot;
using System.Collections.Generic;

public class Attack : State {
    public Attack(StateMachine _parent) : base(_parent) {
        name = STATE.ATTACK;
        stateType = STATETYPE.ACTION;
    }
    public override void Enter() {
        base.Enter();
        animator.Play("Punch");
        locked = true;
        self.GetNode<Catalyst>("Catalyst").state = CATALYST.ENTER;
        if(self.reality.target != null && self.player == -1) 
            self.move.pRotDir = self.reality.target.GlobalTransform.origin - self.GlobalTransform.origin;
        //self.move.maxSpeed = 10;
    }
    public override void Update() {
        succeed();//succeed for now but later requires callback.
        if(animator.CurrentAnimationLength <= animator.CurrentAnimationPosition) {
            locked = false;
            next = new Reserved(sm);
        }
        base.Update();
    }
    public override void Exit() {
        self.GetNode<Catalyst>("Catalyst").state = CATALYST.EXIT;
        animator.Play("Idle");
        //self.move.maxSpeed = 16;
        base.Exit();
    }
}