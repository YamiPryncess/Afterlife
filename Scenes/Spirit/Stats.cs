using System;
using Godot;

public class Stats {
    //Core Attributes
    public int life {set; get;} //Total Health Points
    public int spirit {set; get;} //Total Spirit Points
    public int grace {set; get;} //Total Damage with Dexterous Weapons and Arts
    public int might {set; get;} //Total Damage with Strength Weapons and Arts
    public int wisdom {set; get;} //Total Damage with Spirit Arts
    
    //Sub Attributes
    public float lifeMax {set; get;}
    public float auraMax {set; get;}
    public float postureMax {set; get;}
    public int speed {set; get;}

    //State Attributes
    public float lifePoints {set; get;}
    public float auraPoints {set; get;}
    public float shiftPoints {set; get;}
    public float focusPoints {set; get;}
    public float posturePoints {set; get;}

    public void modify(STATS name, float value) {
        switch(name) {
            case STATS.LIFE:
                lifePoints = Mathf.Clamp(lifePoints + value, 0, lifeMax);
                break;
            case STATS.AURA:
                lifePoints = Mathf.Clamp(auraPoints + value, 0, auraMax);
                break;
            case STATS.SHIFT:
                shiftPoints = Mathf.Clamp(shiftPoints + value, 0, 1);
                break;
            case STATS.POSTURE:
                posturePoints = Mathf.Clamp(posturePoints + value, 0, postureMax);
                break;
        }
    }

    //Physical Abilities
    //Life + Grace = Attack Deflection which allows you to move while attacked
    //Life + Might = Attack Absortion which prevents Knock Back or Sliding
    
    //Spiritual Abilities - Focus and Phasing which have a consistent start up delay but the amount
    //you are focused or phased once it starts depends on Phasing and Focus Rate. They also effect
    //The rate of growth after the start up identically.
    //Spirit + Grace = Phasing Speed which the rate at which you phase.
    //Spirit + Might = Focus Speed which is the rate at which you focus.

    //Wisdom
    //Might & Wisdom = Passion
    //Grace & Wisdom = Charisma - Ability to realize skills that make people like you.
}

public enum STATS {
    LIFE, AURA, SHIFT, POSTURE, FOCUS
}