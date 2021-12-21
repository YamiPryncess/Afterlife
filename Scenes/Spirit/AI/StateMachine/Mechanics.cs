using Godot;
using System.Collections.Generic;
public class Mechanics : Object {
    public Mechanics() {} //Methods like basics and alexia should not share event names.
    public void basics(Spirit s) {
        s.events.Add(MECHEVENT.INPUTDIR, new Event(MECHEVENT.INPUTDIR, new Dictionary<STATE, STATE>()));
        s.events[MECHEVENT.INPUTDIR].addCondition(STATE.IDLE, STATE.WALK);
        s.events[MECHEVENT.INPUTDIR].addCondition(STATE.WALK, STATE.NULL); //States like Attack allows player to move without transitioning to Move State.
        
        s.events.Add(MECHEVENT.NODIR, new Event(MECHEVENT.NODIR, new Dictionary<STATE, STATE>()));
        s.events[MECHEVENT.NODIR].addCondition(STATE.WALK, STATE.IDLE);
        s.events[MECHEVENT.NODIR].addCondition(STATE.IDLE, STATE.NULL);
        
        s.events.Add(MECHEVENT.ATTACKPRESS, new Event(MECHEVENT.ATTACKPRESS, new Dictionary<STATE, STATE>()));
        s.events[MECHEVENT.ATTACKPRESS].addCondition(STATE.IDLE, STATE.ATTACK);//Idle can be set back by the state itself.
        s.events[MECHEVENT.ATTACKPRESS].addCondition(STATE.WALK, STATE.ATTACK);//Switching inside of state might be more dominant than in mechanics though. Not sure yet, need to think and test.
        s.events[MECHEVENT.ATTACKPRESS].addCondition(STATE.ATTACK, STATE.ATTACK); //Player may move in states like attack
                                                //Attack -> Attack repeats attack but in the future it may go to Attack 2
        s.events.Add(MECHEVENT.ANIMEND, new Event(MECHEVENT.ANIMEND, new Dictionary<STATE, STATE>()));
        s.events[MECHEVENT.ANIMEND].addCondition(STATE.ATTACK, STATE.IDLE);

        s.events.Add(MECHEVENT.STANCEPRESS, new Event(MECHEVENT.STANCEPRESS, new Dictionary<STATE, STATE>()));
        s.events[MECHEVENT.STANCEPRESS].addCondition(STATE.IDLE, STATE.STANCE);
        s.events[MECHEVENT.STANCEPRESS].addCondition(STATE.WALK, STATE.STANCE);
        s.events[MECHEVENT.STANCEPRESS].addCondition(STATE.RUN, STATE.BREAK);

        s.events.Add(MECHEVENT.JUMPPRESS, new Event(MECHEVENT.JUMPPRESS, new Dictionary<STATE, STATE>()));
        s.events[MECHEVENT.JUMPPRESS].addCondition(STATE.IDLE, STATE.JUMP);
        s.events[MECHEVENT.JUMPPRESS].addCondition(STATE.WALK, STATE.JUMP);

        s.events.Add(MECHEVENT.DROP, new Event(MECHEVENT.DROP, new Dictionary<STATE, STATE>()));
        s.events[MECHEVENT.DROP].addCondition(STATE.JUMP, STATE.DROP);
        s.events[MECHEVENT.DROP].addCondition(STATE.IDLE, STATE.DROP);
        s.events[MECHEVENT.DROP].addCondition(STATE.WALK, STATE.DROP);
        s.events[MECHEVENT.DROP].addCondition(STATE.RUN, STATE.DROP);

    }
    public void crimson() {

    }
    public void leon() {

    }
    public void alexia() {

    }
    public void selen() {

    }
}

public enum MECHEVENT {
    INPUTDIR, NODIR, ATTACKPRESS, ANIMEND, STANCEPRESS, DASHPRESS, JUMPPRESS, DROP, NONE
}