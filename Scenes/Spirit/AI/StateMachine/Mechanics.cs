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
        s.events["AttackPressed"].addCondition(STATE.IDLE, STATE.ATTACK);
        s.events["AttackPressed"].addCondition(STATE.MOVE, STATE.ATTACK);
        s.events["AttackPressed"].addCondition(STATE.ATTACK, STATE.NULL); //Player may move in states like attack
        
        s.events.Add("AttackEnd", new Event("AttackEnd", new Dictionary<STATE, STATE>()));
        s.events["AttackEnd"].addCondition(STATE.ATTACK, STATE.IDLE);
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