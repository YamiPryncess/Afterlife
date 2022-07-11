using Godot;
using System.Collections.Generic;
using System;

public class State {
    public STATE name {set; get;}
    public STATETYPE stateType = STATETYPE.ANY;
    public StateMachine sm {set; get;}
    public State next {set; get;}
    public STAGE stage {set; get;}
    public Spirit self {set; get;}
    public Reality reality {set; get;}
    public AnimationPlayer animator {set; get;}
    public float delta {set; get;} = 0;
    public float seconds {set; get;} = 0;
    public int frames {set; get;} = 0;
    public bool exited {set; get;} = false;
    public bool succeeding = false; //Used for enforced animations who may succeed upon a callback event
    public BSIGNAL sSignal {set; get;} = BSIGNAL.FAIL;//But do not return a signal until the last frame.
    public bool physicsCycledOnce {set; get;} = false;
    public bool locked = false;
    public State(StateMachine _parent) {
        sm = _parent;
        self = sm.self;
        animator = sm.animator;
        stage = STAGE.ENTER;
    }
    public void cancel() {
        locked = false;
        stage = STAGE.EXIT;
    }
    public virtual void Enter() {
        stage = STAGE.UPDATE;
        self.snap = Vector3.Down;
        //if(self.player == 1) GD.Print(stateType + " Enter: ", name);
    }
    public virtual void Update() { //No need to redundantly set stage to update again.
        stage = STAGE.UPDATE;
        seconds += delta;
        //if(stateType == STATETYPE.ACTION) animator.Advance(delta);
        if(!locked && next != null) {
            sSignal = succeeding == true ? BSIGNAL.PASS : BSIGNAL.FAIL;
            stage = STAGE.EXIT;
        } else {
            sSignal = BSIGNAL.RUNNING;
        } frames++;
        //GD.Print("Player ", self.player, " ", name, ": ", frames, 
        //" isPlaying: ", animator.IsPlaying(), " stage: ", stage, " nextState: ", parent.nextState);
    }
    public virtual void Exit() { stage = STAGE.EXIT; exited = true; } 
    public void process(float _delta, STATETYPE type) { //Here are the if statements that tell all process functions to run
	    delta = _delta;
        stateType = type;
        if (stage == STAGE.ENTER) Enter();
	    if (stage == STAGE.UPDATE) Update(); 
	    if (stage == STAGE.EXIT) Exit();
    }
    public float count(float current, float limit) {
        if(current >= limit) {
            return current;
        } 
        return current + delta;
    }
    public bool sufficeTime(float current, float limit) {
        if(current >= limit) {
            return true;
        }
        return false;
    }
    public void succeed() {
        succeeding = true; 
        if(!locked) { 
            sSignal = BSIGNAL.PASS;
            stage = STAGE.EXIT;
        } 
    }
    public void fail() {
        succeeding = false;
        if(!locked) {
            sSignal = BSIGNAL.FAIL;
            stage = STAGE.EXIT;
        }
    }
}
public enum STAGE {
    ENTER, UPDATE, EXIT
}

//Documentation
//Normally the state machine can be changed at any time by setNextState()
//However the animation player may set enforceUpdate to true to prevent nextState from happening.
//If the animation player is doing this, 
//then it must also set finalFrame to true at the end of the animation.
//That way the animation escapes through the update virtual instead with final frame = true.

//The behavior tree does not use setNextState() unless the previous bTree node's state machine
//returned passing or failed. So in this case there is no need to worry about enforce update,
//Unless the node's conditions want to manipulate the state machine that is.
//For example if the enemy is doing an attack and then they get staggered by the player.
//The conditional behavior tree will interupt the dominant behavior tree to run stagger.
//In this case it can run cancel then run stagger (I think, need to test it.)

//Extra thought: I don't think that anything other than anim player should touch enforce update for now.
//Other things are allowed to turn enfocr update off through cancel() though.

//Old Code
// if (stage == EVENT.ENTER) Enter(); //Enter will change stage to update
// 	    if (stage == EVENT.UPDATE) Update(); //Update runs mandatory, once per frame-
// 	    if (stage == EVENT.EXIT) {//If update is missed due to stage = Exit-
// 		    Exit(); //Update will be called again right after exit on next line. 
// 		    nextState.process(_delta);//Stage is set to Exit by observer pattern-
//             return nextState;//or behavior tree. Exit never happens from within
//         }//the machine else the .process()frame Update recursion may cause a stack error.
//         return this;//Returns either the current running state or the new state in Exit().