using Godot;
using System.Collections.Generic;

public class Jump : State {
    Movement move;
    float startPoint;
    public Jump(StateMachine _parent) : base(_parent) {
        name = STATE.JUMP;
    }
    public override void Enter() {
        base.Enter();
        move = self.move;
        self.snap = Vector3.Zero;
        move.modVelocity = 15; //Bug: Double Jump is possible by pressing x fast. (Might be fixed)
        animator.Play("Jump");
        animator.Advance(.5f);
        //startPoint = self.GlobalTransform.origin.y;
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 16f, 16f);
        //Y velocity keeps getting applied if jump doesn't end.
    }
    public override void Exit() {
        base.Exit();
        //GD.Print(startPoint, ", ", self.GlobalTransform.origin.y);
    }
}

        // move.yVelocity = self.IsOnFloor() ? 0 : move.gravity + move.velocity.y;

        // if(self.IsOnFloor() || move.coyote()) { //Running is on floor here would make jump lag but that's why it exits later on if FAILED.
        //     if(!attemptingJump){ //Start Jump.
        //         move.yVelocity = move.jumpImpulse; //Only if on floor. Also must happen in update not entry.
        //         attemptingJump = true;
        //     } else if(trulyJumped) {//End Jump. Will not land unless touches floor. For now.
        //         endEnforce(); //Has to happen before which is bad!
        //         self.events[MECHEVENT.LANDED].validate(self);
        //     }
        // } 
        // GD.Print(seconds, attemptingJump, trulyJumped, self.IsOnFloor());
        // if(attemptingJump && !self.IsOnFloor()) {
        //     trulyJumped = true;
        // } else if (seconds > .033333 && !trulyJumped){
        //     endEnforce();
        //     self.events[MECHEVENT.FAILED].validate(self);
        // }

        // move.pRotDir = move.inputDir;
        // move.pMoveDir = move.chooseVel(VELOCITY.CONTROL, speed);
        // if(move.pMoveDir.Length() > max){
        //     move.pMoveDir = move.pMoveDir.Normalized() * max;
        // }
