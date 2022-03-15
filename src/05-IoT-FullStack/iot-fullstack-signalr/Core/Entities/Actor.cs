using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    //public enum ActorName
    //{
    //    IotShellyPlug_Relay = 101,
    //}


    public class Actor : Item
    {
        public double SettedValue { get; set; }

        Action<Actor, double> SetActorAction { get; set; }

        public Actor(ItemEnum itemEnum, string unit, Action<Actor, double> setActorAction = null) : base(itemEnum, unit)
        {
            SetActorAction = setActorAction;
        }


        public Actor()  { }

        //public Actor(ItemEnum itemEnum, string unit = ""): base(ItemEnum itemEnum)
        //{
        //    Name = actorName.ToString();
        //}

        //public Actor(ActorName actorName, Action<Actor, double> setActorAction, string unit = "",  int persistenceInterval = Item.MAX_PERSISTENCE_INTERVAL)
        //        : base((int)actorName, unit, persistenceInterval)
        //{
        //    Name = actorName.ToString();
        //    SetActorAction = setActorAction;
        //}

        public override string ToString() => $"Actor: {Name}";

        public void SetActor(double value)
        {
            SetActorAction?.Invoke(this, value);
        }
    }
}
