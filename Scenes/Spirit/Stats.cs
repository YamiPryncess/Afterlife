using System;
using Godot;

public class Stats {
    //Core Attributes
    public int life {set; get;} = 0; //Total Health Points
    public int spirit {set; get;} = 0; //Total Spirit Points
    public int grace {set; get;} = 0; //Total Damage with Dexterous Weapons and Arts
    public int might {set; get;} = 0; //Total Damage with Strength Weapons and Arts
    public int wisdom {set; get;} = 0; //Total Damage with Spirit Arts
    
    //Sub Attributes
    public float lifeMax {set; get;} = 100;
    public float auraMax {set; get;} = 100;
    public float stanceMax {set; get;}
    public int speed {set; get;}

    //State Attributes
    public float lifePoints {set; get;} = 100;
    public float auraPoints {set; get;} = 100;
    public float phasePoints {set; get;} = 0;
    public float focusPoints {set; get;} = 0;
    public float stancePoints {set; get;} = 100;

    public void modify(STATS name, float value) {
        switch(name) {
            case STATS.LIFE:
                lifePoints = Mathf.Clamp(lifePoints + value, 0, lifeMax);
                break;
            case STATS.AURA:
                lifePoints = Mathf.Clamp(auraPoints + value, 0, auraMax);
                break;
            case STATS.PHASE:
                phasePoints = Mathf.Clamp(phasePoints + value, 0, 1);
                break;
            case STATS.STANCE:
                stancePoints = Mathf.Clamp(stancePoints + value, 0, stanceMax);
                break;
        }
    }
    public void levelUp() {

    }
}

public enum STATS {
    LIFE, AURA, PHASE, STANCE, FOCUS
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

//Old Code
    //public Dictionary<string, float[]> stats {set; get;} = new Dictionary<string, float[]>();
    //0=current, 1=maximum, 2=alterations
    //Status Aliments will be handled else where
    // float[] defaultVal = new float[3]{10,10,0};
    //     stats.Add("Health", defaultVal);
    //     stats.Add("Focus", defaultVal);
    //     stats.Add("Hollow", defaultVal);
    //     stats.Add("Power", defaultVal);
    //     stats.Add("Speed", defaultVal);
    // public float getStat(string index){
    //     return stats[index][0];
    // }
    // public float setStat(string index, int change) {
    //     stats[index][0] = Mathf.Min(stats[index][0] + change,
    //         stats[index][1] + stats[index][2]);
    //     return stats[index][0];
    // }
    // public float getMax(string index) {
    //     return stats[index][1] + stats[index][2];
    // }
    // public float alterMax(string index, float alteration) {
    //     stats[index][2] += alteration;
    //     return getMax(index);
    // }