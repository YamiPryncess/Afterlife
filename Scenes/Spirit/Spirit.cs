using Godot;
using System.Collections.Generic;

public class Spirit : KinematicBody {
    public Master master {set; get;}
    public StateMachine sm {set; get;}
    public Camera camera {set; get;}
    public Hud hud {set; get;}
    public Stats stats {set; get;}
    public Phaser phaser {set; get;}
    public KinematicCollision collision {set; get;}
    public float idleDelta {set; get;}
    public Movement move {set; get;}
    public MOVEMODE moveMode = MOVEMODE.SLIDE;
    [Export] public int player {set; get;} = -1;
    [Export] public bool disable {set; get;} = false;
    public BTree bTree {set; get;}
    public Reality reality {set; get;}
    public Dictionary<BELIEF, bool> beliefs {set; get;} = new Dictionary<BELIEF, bool>();
    public bool animBool {set; get;} = false;
    public bool stanceBool {set; get;} = false;
    public Vector3 snap {set; get;}
    public Area floor {set; get;}
    public override void _Ready() {
        master = GetNode<Master>("/root/Master");
        sm = new StateMachine(this, master.mechEvents);
        reality = new Reality();
        phaser = GetNode<Phaser>("Phaser");
        phaser.self = this;
        floor = GetNode<Area>("Floor");
        hud = (Hud)GetTree().GetNodesInGroup("Huds")[player > -1 ? player - 1 : 1];
        stats = new Stats();
        move = new Movement(this);
        snap = Vector3.Down;
        
        if(isPlayer()) {
            //master.mechanics.Call("basics", this);
            camera = GetNode<Camera>("../Camera");
        }
        else if(player == -1) {
            master.behavior.Call("sample", this);
            reality.target = (Spirit)GetTree().GetNodesInGroup("Players")[0];
        }
    }
    public string pad(string button) {
        string btn = button + "_" + player;
        return btn;
    }
    public bool isPlayer() {
        if(player < 5 && player > 0) {
            return true;
        } else return false;
    }
    public override void _Process(float delta) {
        idleDelta = delta;
        if(disable) {
            return;
        }
        if(stats.lifePoints <= 0) { QueueFree(); }
        if(isPlayer()) {
            preProcessState(delta);//Mutates controller variables like inputDir, also calls state change events
            sm.process(delta);//Mutates state variables like speedDir & rotateDir (may use velocity to change them)
        } else if(bTree != null) {
            bTree.process();
        }
    }
    public override void _PhysicsProcess(float delta) {
        if(disable) {
            return;
        }
        //if(player == 1) GD.Print(sm.currentState.name, " ", velocity);
        postProcessState(delta); //Mutates physics variables like Velocity, then resets controller and input variables
    }
    public void preProcessState(float delta) {
        //Handles Event Observer pattern changes state before state processes
        endAnimator(delta);
        //if(!move.moveBool) 
        move.moveInput();//Also preProcesses important variables for state.
        move.preProcess(delta);
        move.crouchInput();
        move.dashInput();
        move.stanceInput();
        attack(delta);
        phaser.phase(delta, this);
    }
    public void postProcessState(float delta) {
        //Mandatory logic post state, the state can manipulate how it happens though.
        phaser.solidify(this);
        //GD.Print(move.velocity.y);
        //if(player == 1 && sm.currentState.name == STATE.AIR) GD.Print(sm.currentState.name, " | Vel: ", move.velocity.y, " | Mod: ", move.modVelocity, " | IsOnFloor: ", IsOnFloor());
        //if(player == 1) GD.Print(sm.currentState.name, " | Vel: ", move.velocity.y, " | Mod: ", move.modVelocity, " | IsOnFloor: ", IsOnFloor());
        //if(player == 1) GD.Print(sm.currentState.name, " ", move.modVelocity);
        //if(player == 1) GD.Print(move.velocity.y + move.gravity * delta);
        //if(player == 1 && GlobalTransform.origin.y > 2.45) GD.Print(GlobalTransform.origin.y);
        move.velocity = MoveAndSlide(new Vector3(move.pMoveDir.x, 
                        (move.velocity.y + move.modVelocity) + (move.gravity * delta), 
                        move.pMoveDir.z), Vector3.Up, true); 
        //Always moves but states like idle can manipulate it.
        //if(player == 1) GD.Print(sm.currentState.name, " ", move.velocity);
        sm.activeStates[STATETYPE.MOTION].physicsCycledOnce = true;
        move.rotate(delta);
        resetPhysics();
    }
    public void resetPhysics() {//For restarting state.
        move.inputDir = Vector3.Zero;
        move.pMoveDir = Vector3.Zero;
        move.modVelocity = 0;
    }
    public void frameSignal() {//Game is idle process delta based so I don't need this unless as an option.
        if(phaser.phaseState > 0) {
            phaser.frame++;
        }
    }
    public void endAnimator(float delta) {
        if(!sm.animator.IsPlaying() && !sm.nextIsTrue() && animBool == true) {
            sm.runEvent(MECHEVENT.ANIMEND);
            animBool = false;
        } else {
            animBool = true;
        }
    }
    public void final_frame() {
        sm.statesLock(false, STATETYPE.ACTION);
    }
    public void enforce_anim() {
        sm.statesLock(true, STATETYPE.ACTION);
    }
    public void attack(float delta) {
        if(Input.IsActionJustPressed(pad("attack"))) {
            //GD.Print("Button has been pressed!");
            //GD.Print(sm.mechEvents.ContainsKey(STATETYPE.ACTION));
            //GD.Print(sm.mechEvents[STATETYPE.ACTION].ContainsKey(MECHEVENT.ATTACKPRESS));
            sm.mechEvents[STATETYPE.ACTION][MECHEVENT.ATTACKPRESS].validate(this);
        }
    }
    public void hurt() {
        stats.modify(STATS.LIFE, -(20 * (1-stats.phasePoints)));
        hud.healthProgress.Value = stats.lifePoints;
    }
}

public enum MOVEMODE {
    SLIDE, COLLIDE, SNAP
}
//Old
// switch(moveMode) {
//             case MOVEMODE.SLIDE: 
//                 break;
//                         case MOVEMODE.COLLIDE:
//                 move.modVelocity += move.gravity * delta;
//                 collision = MoveAndCollide(new Vector3(move.pMoveDir.x * delta, move.modVelocity, move.pMoveDir.z * delta));
//                 break;
//             case MOVEMODE.SNAP: 

//                 break;
//         }
    // public void processState(float delta) {//StateMachine.Update() runs once per frame confirmed.
    //     //Makes update() frames run regardless if state is exited before in preProcessState().
    //     if(currentState.getStage() == EVENT.EXIT) {
    //         currentState = currentState.process(delta);//Update hasn't happened yet, so exit first.
    //     }
    //     currentState = currentState.process(delta);//Now we run this always for the update.
    // }

//Create, Update, Delete (Can be unreliable vs reliable)
//RCP Reliable- "guarantees" packet will be recieved (doing damage)
//Rcp unreliable- doesn't guarantee it (if it doesn't matter, movement is usually here)

//Transferring inputs

//Transfer velocity
//Hey this is my velocity at this game time, info is used by server
//rewinds and simulates whether the velocity was plausible.

//Local player authoritive on clients?
//Duro says that's why you have rollback?
//Zylann says it's expeensive depending the complexity of your game
//Rollback is perfectly acceptable for majority of games according to duro
//Zylaan says resimulating the game ten times per frames isn't always an option?
//Bad for multiagents physics driven game according to zylann
//Duro you just do a simulation of the 2 conflicting actors
//If you shoot, server says from this vector, shooting a bullet into this vector, is a plausible move
