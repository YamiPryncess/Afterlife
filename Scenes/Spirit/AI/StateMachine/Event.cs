using Godot;
using System.Collections.Generic;
public class Event {
    //Events switches the states, not state code.
    //So state code doesn't need to specify anything about 
    //switching to a specific state. It should be able to however
    //using nextstate variable and it can also make mandatory
    //requirements for switching to any state generally too.
    //I'd prefer to make groups based on event types.
    //So one group of events may interupt while another may queue
    //A state to the next action. Then maybe the state can check
    //for itself if it wants to allow that type of event & process it.
    public MECHEVENT name {set; get;} = MECHEVENT.NONE;
    public EVENTTYPE eventType {set; get;}
    public Dictionary<STATE, STATE> condition {set; get;} = 
    new Dictionary<STATE, STATE>(); //Only accessed by mechanics, nothing else.
    public Event(MECHEVENT _mechEvent, Dictionary<STATE, STATE> _condition) {
        name = _mechEvent;
        condition = _condition;
    }
    public void addCondition(STATE curCondition, STATE nextTransition) {
        condition.Add(curCondition, nextTransition);
    }
    public void validate(Spirit player) {
        StateMachine sm = player.sm;
        STATE curStateEnum = sm.currentState.name;
        if(condition.ContainsKey(curStateEnum)
            && condition[curStateEnum] != STATE.NULL) { //If transitioning to same state, NULL stops it.
            State newState = sm.enumToState(condition[curStateEnum]);
            if(newState != null) {
                sm.setNextState(newState);
            }//In the future states may also have subordinate states.
        }//Attack for example may have move and jump as subordinates in its class.
    }//They can do side processes and be called by validate() too.
}

public enum EVENTTYPE {
    QUEUE, IMMEDIATE, SIMULTANEOUS
}