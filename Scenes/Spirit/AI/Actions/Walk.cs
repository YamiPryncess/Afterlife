using Godot;
using System.Collections.Generic;

public class Walk : State {
    Movement move;
    public Walk(StateMachine _parent) : base(_parent) {
        name = STATE.WALK;
    }
    public override void Enter() {
        base.Enter();
        move = self.move;
    }
    public override void Update() {
        base.Update();
        move.rotateDir = move.inputDir;
        move.speedDir = move.chooseVel(VELOCITY.CONTROL);
        if(move.speedDir.Length() > move.maxSpeed){
            move.speedDir = move.speedDir.Normalized() * move.maxSpeed;
        }
        move.yVelocity = self.IsOnFloor() ? 0 : move.gravity + move.velocity.y;
        //newGravity = self.stats.phasePoints >= self.phaser.phasable ? 0 : gravity;
    }
    public override void Exit() {
        base.Exit();
    }
}