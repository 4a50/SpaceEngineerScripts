using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
  partial class Program : MyGridProgram
  {   
    public Program()
    {
      //Runs the programmable block every 10 cycles (timer block not required)
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
    } 

    public void Main(string argument, UpdateType updateSource)
    {
      //Names of the Doors for the airlock.
      //Comment out the AirVents items if one is not present.
      string doorOne = "AirlockOne";
      string doorTwo = "AirlockTwo";
      string airVent = "AirVentOne";

      var doorOneObj = GridTerminalSystem.GetBlockWithName(doorOne) as IMyDoor;
      var doorTwoObj = GridTerminalSystem.GetBlockWithName(doorTwo) as IMyDoor;
      var airVentObj = GridTerminalSystem.GetBlockWithName(airVent) as IMyAirVent;

      Echo($"{doorOneObj.CustomName}: {doorOneObj.Status}");
      Echo($"{doorOneObj.CustomName}: {doorTwoObj.Status}");
      Echo($"{airVentObj.CustomName} Airtight: {airVentObj.Status}");
      // If overriding airlock not desired.
      if (argument.ToUpper() != "OVERRIDE AIRLOCKS")
      {
        if (doorOneObj.Status == DoorStatus.Open)
        {
          if (doorTwoObj.Status == DoorStatus.Open || doorTwoObj.Status == DoorStatus.Opening)
          {
            doorTwoObj.CloseDoor();
          }
          doorTwoObj.Enabled = false;
          doorOneObj.Enabled = true;
        }
        else if (doorTwoObj.Status == DoorStatus.Open)
        {
          if (doorOneObj.Status == DoorStatus.Open || doorOneObj.Status == DoorStatus.Opening)
          {
            doorOneObj.CloseDoor();
          }
          doorTwoObj.Enabled = true;
          doorOneObj.Enabled = false;
        }
        else if (doorOneObj.Status == DoorStatus.Closed && doorTwoObj.Status == DoorStatus.Closed && airVentObj.Status == VentStatus.Pressurizing)
        {
          doorTwoObj.Enabled = false;
          doorOneObj.Enabled = false;
        }
        else
        {
          doorTwoObj.Enabled = true;
          doorOneObj.Enabled = true;
        }
      }
    }
  }
}
