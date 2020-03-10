using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GroundCreature : Creature {

    public static GroundCreature Create(Vector2 position, int birthDay) {
        var creature = Creature.Create<GroundCreature>(position, birthDay);

        creature.InitProperties();

        return creature;
    }

    protected void InitProperties() {
        this.movement = new GroundMovement(this);

        // this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
        //     this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.1f, 0.4f),
        //     this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.0f, 0.15f),
        //     0f);
    }

}
