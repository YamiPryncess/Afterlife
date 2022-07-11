using Godot;
using System.Linq;
using System.Collections.Generic;

public class Meeting : Spatial {
    public Vector3 target {set; get;} = new Vector3();
    public float largestDist {set; get;} = 1;
    public List<Spirit> players {set; get;} = new List<Spirit>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        foreach(var child in GetChildren().OfType<Spirit>()){
            child.AddToGroup("Players");
            players.Add(child);
        }
    }

    public override void _Process(float delta) {
        target = playerCenter();
        largestDist = playerDist();
    }

    public Vector3 playerCenter() {
        Vector3 accumilator = new Vector3();
        for(int i = 0; i < players.Count; i++) {
            if(IsInstanceValid(players[i]))
            accumilator += players[i]
                .GetNode<Spatial>("Target").GlobalTransform.origin;
        }
        return accumilator / players.Count;
    }
    public float playerDist() {
        float largestDist = 0;
        for(int i = 0; i < players.Count; i++) {
            for(int j = 0; j < players.Count; j++){
                float distance = players[i].GetNode<Spatial>("Target")
                .GlobalTransform.origin.DistanceTo(players[j]
                .GetNode<Spatial>("Target").GlobalTransform.origin);
                if(distance > largestDist) {
                    largestDist = distance;
                }
            }
        }
        return largestDist > 1 ? largestDist : 1;
    }
}