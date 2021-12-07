using Godot;
public class Phaser : Area {
    public float phaseVal {set; get;} = 0;
    private float phaseInterval = 0;
    public int phaseState  {set; get;}= 0;
    public int frame {set; get;} = 0;
    private int prevFrame = 0;
    [Export] public float phasable {set; get;} = .5f;
    [Export] public float peak {set; get;} = .8f;
    [Export] public int threshold {set; get;} = 15;
    [Export] public float incline {set; get;} = .20f;
    [Export] public float decline {set; get;} = .025f; //Might want to change this based on threshold so they get their aura faster
    [Export] public float cancelFrame {set; get;} = 45; //Or give the player a cancel option but they'd lose their aura which may not be good...
    public void phase(float delta, Spirit spirit) {
        if(Input.IsActionJustPressed(spirit.pad("phase")) && phaseState == 0) {
            phaseState = 1;
            phaseInterval = 0;
            frame = 0;
            prevFrame = frame;
        }
        if(phaseState > 0) {
            phaseInterval += delta;
            if(phaseInterval >= .033f) {
                frame++;
                phaseInterval = 0;
            }
            if(frame > prevFrame) {
                prevFrame = frame;
                if(phaseState == 1) {
                    phaseVal += incline;
                    if(phaseVal >= peak) {
                        phaseState = 2;
                        phaseVal = peak;
                    }   
                } else if(phaseState == 2) {
                    phaseVal = peak;
                    if(frame >= threshold) phaseState = 3;
                } else if(phaseState == 3) { 
                    phaseVal -= decline;
                    if(phaseVal <= 0) { phaseState = 0; }    
                }
            }
        }
        phaseVal = Mathf.Clamp(phaseVal, 0, 1);
        if(phaseVal >= phasable && (spirit.GetCollisionLayerBit(0) || spirit.GetCollisionMaskBit(0))) {
            spirit.SetCollisionLayerBit(0, false);
            spirit.SetCollisionMaskBit(0, false);
        }
        spirit.hud.phaseProgress.Value = phaseVal * 100;
    }
    public void solidify(Spirit spirit) {
        if(phaseVal < phasable && !isOverlapping()
            && !(spirit.GetCollisionLayerBit(0) || spirit.GetCollisionMaskBit(0))) {
            spirit.SetCollisionLayerBit(0, true);
            spirit.SetCollisionMaskBit(0, true);
        }
    }
    public bool isOverlapping() {
        Godot.Collections.Array bodies = GetOverlappingBodies();
        bool result = false;
        for(int i = 0; i < bodies.Count; i++) {
            if(bodies[i] is PhysicsBody body && body.GetCollisionLayerBit(0) == true) {
                result = true;
            }
        }
        return result;
    }
}

//Old Code
//Physics process based start up i frames
            // if(frame >= 0 && frame <= 2) phaseVal = 0;
            // else if(frame > 2 && frame <= 12) phaseVal = .95f;
            // else if(frame > 12) { phaseVal = .50f; phaseState = 2; }
//Quick Start & Slow Fade out
        // if(phaseState > 0) {
        //     phaseInterval += delta;
        //     if(phaseInterval >= .033f) {
        //         frame++;
        //         phaseInterval = 0;
        //     }
        //     if(frame > prevFrame) {
        //         prevFrame = frame;
        //         if(phaseState == 1) {
        //             phaseVal += .20f;
        //             if(phaseVal >= 1) phaseState = 2;    
        //         }
        //         else if(phaseState == 2) { 
        //             phaseVal -= .025f;
        //             if(phaseVal <= 0) { phaseState = 0; }    
        //         }
        //     }
//Symmetrical Phase
//        if(phaseState == 1) {
//                 phaseInterval += delta;
//                 if(phaseInterval >= .20f) {
//                     phaseInterval = 0;
//                     phaseVal += .10f;
//                 } if(phaseVal >= 1) phaseState = 2;
//         } else if(phaseState == 2) {
//             phaseInterval += delta;
//             if(phaseInterval >= .20f) {
//                 phaseInterval = 0;
//                 phaseVal -= .10f;
//             } if(phaseVal <= 0) phaseState = 0;
//         }


// public void phase(float delta, Spirit spirit) {
//         phaseInterval += delta;
//         if(Input.IsActionPressed(spirit.pad("phase"))) {
//             if(phaseBool == false) {
//                 phaseBool = true;
//                 phaseInterval = 0;
//             }
//             if(phaseInterval >= .20f) {
//                 phaseVal += .10f;
//                 phaseInterval = 0;
//             }
//         } else {
//             if(phaseBool == true) {
//                 phaseBool = false;
//                 phaseInterval = 0;
//             }
//             if(phaseInterval >= .20f) {
//                 phaseVal -= .10f;
//                 phaseInterval = 0;
//             }
//         }
//         if(phaseVal <= 0) {
//             phaseVal = 0;
//         } else if(phaseVal >= 1) {
//             phaseVal = 1;
//         }
//         if(phaseVal >= phasable && (spirit.GetCollisionLayerBit(0) || spirit.GetCollisionMaskBit(0))) {
//             spirit.SetCollisionLayerBit(0, false);
//             spirit.SetCollisionMaskBit(0, false);
//         }
//     }