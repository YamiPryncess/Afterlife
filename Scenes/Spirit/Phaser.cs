using Godot;
public class Phaser : Area {
    public float phaseVal {set; get;} = 0;
    public float phaseInterval {set; get;} = 0;
    public int phaseState {set; get;} = 0;
    public float phasable {set; get;} = .5f;
    public void phase(float delta, Spirit spirit) {
        if(Input.IsActionJustPressed(spirit.pad("phase")) && phaseState == 0) {
            phaseState = 1;
            phaseInterval = 0;    
        }
        if(phaseState == 1) {
                phaseInterval += delta;
                if(phaseInterval >= .20f) {
                    phaseInterval = 0;
                    phaseVal += .10f;
                } if(phaseVal >= 1) phaseState = 2;
        } else if(phaseState == 2) {
            phaseInterval += delta;
            if(phaseInterval >= .20f) {
                phaseInterval = 0;
                phaseVal -= .10f;
            } if(phaseVal <= 0) phaseState = 0;
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