using Godot;
using System.Collections.Generic;

public class StateMachine {
    public STATE name {set; get;}
    public EVENT stage {set; get;}
    public BSIGNAL sSignal {set; get;} = BSIGNAL.FAIL;
    public StateMachine nextState {set; get;}
    public Spirit player {set; get;}
    public float delta {set; get;} = 0;
    public int frames {set; get;} = 0;
    //public bool update = false;
    public StateMachine() {
        stage = EVENT.ENTER;
    }
    public EVENT getStage() {
        return stage;
    }
    public void setNextState(StateMachine _nextState) {
        stage = EVENT.EXIT;
        nextState = _nextState;
    }
    public StateMachine enumToState(STATE newState) {
        switch (newState){
            case STATE.MOVE:
                return new Move(player);
            case STATE.IDLE:
                return new Idle(player);
            case STATE.ATTACK:
                return new Attack(player);
            case STATE.NULL:
                return null;
            default: 
                return null;
      }
    }
    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; } 
    public StateMachine process(float _delta) { //Here are the if statements that tell all process functions to run
	    delta = _delta;
        if (stage == EVENT.ENTER) Enter(); //Enter will change stage to update
	    if (stage == EVENT.UPDATE) Update(); //Update runs mandatory, once per frame-
	    if (stage == EVENT.EXIT && frames <= 0) {//If update is missed due to Exit-
		    Exit(); //Update will be called again right after exit on next line. 
		    nextState.process(_delta);//Stage is set to Exit by observer pattern-
            return nextState;//or behavior tree. Exit never happens from within
        }//the machine else the .process()frame Update recursion may cause a stack error.
        if(frames > 0) frames--;//Disables exit unless frames drop to zero or are canceled.
        return this;//Returns either the current running state or the new state in Exit().
    }
}

public enum STATE {
    IDLE, MOVE, RUN, ATTACK, JUMP, FALL, NULL
}
public enum EVENT {
    ENTER, UPDATE, EXIT
}