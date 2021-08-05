using Godot;
using System.Collections.Generic;
public class Mechanics : Object {
    public Mechanics() {} //Methods like basics and alexia should not share event names.
    public void basics(Spirit s) {
        s.events.Add("InputDirection", new Event("InputDirection", new Dictionary<STATE, STATE>()));
        s.events["InputDirection"].addCondition(STATE.IDLE, STATE.MOVE);
        s.events["InputDirection"].addCondition(STATE.MOVE, STATE.NULL); //States like Attack allows player to move without transitioning to Move State.
        
        s.events.Add("NoDirection", new Event("NoDirection", new Dictionary<STATE, STATE>()));
        s.events["NoDirection"].addCondition(STATE.MOVE, STATE.IDLE);
        s.events["NoDirection"].addCondition(STATE.IDLE, STATE.NULL);
        
        s.events.Add("AttackPressed", new Event("AttackPressed", new Dictionary<STATE, STATE>()));
        s.events["AttackPressed"].addCondition(STATE.IDLE, STATE.ATTACK);//Idle can be set back by the state itself.
        s.events["AttackPressed"].addCondition(STATE.MOVE, STATE.ATTACK);//Switching inside of state might be more dominant than in mechanics though. Not sure yet, need to think and test.
        s.events["AttackPressed"].addCondition(STATE.ATTACK, STATE.ATTACK); //Player may move in states like attack
                                                //Attack -> Attack repeats attack but in the future it may go to Attack 2
        string animEnd = "AnimEnd";
        s.events.Add(animEnd, new Event(animEnd, new Dictionary<STATE, STATE>()));
        s.events[animEnd].addCondition(STATE.ATTACK, STATE.IDLE);
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