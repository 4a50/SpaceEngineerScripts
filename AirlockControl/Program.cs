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
      string outerDoorName = "Sliding Door Airlock Outer";
      string doorInnerName = "Sliding Door Airlock Inner";
      string sideDoorName = "Offset Door Passage LQ";
      string airVent = "Air Vent Airlockpro";

      var doorOuter = GridTerminalSystem.GetBlockWithName(outerDoorName) as IMyDoor;
      var doorInner = GridTerminalSystem.GetBlockWithName(doorInnerName) as IMyDoor;
      IMyDoor sideDoor = GridTerminalSystem.GetBlockWithName(sideDoorName) as IMyDoor;
      var airVentObj = GridTerminalSystem.GetBlockWithName(airVent) as IMyAirVent;

      Echo($"{doorOuter.CustomName}: {doorOuter.Status}");
      Echo($"{doorOuter.CustomName}: {doorInner.Status}");
      Echo($"{sideDoor.CustomName}: {sideDoor.Status}");
      Echo($"{airVentObj.CustomName} Airtight: {airVentObj.Status}");
     // Outer Door Logic
      if (doorOuter.Status == DoorStatus.Open || doorOuter.Status == DoorStatus.Opening)
        {
          //if (doorinner.Status == DoorStatus.Open || doorinner.Status == DoorStatus.Opening)
          //{
            doorInner.CloseDoor();
            sideDoor.CloseDoor();
          //}
          doorInner.Enabled = false;
          sideDoor.Enabled = false;          
          doorOuter.Enabled = true;
        }
      // Inner Door Logic
        else if (doorInner.Status == DoorStatus.Open || doorInner.Status == DoorStatus.Opening)
        {
          //if (doorOuter.Status == DoorStatus.Open || doorOuter.Status == DoorStatus.Opening)
          //{
            doorOuter.CloseDoor();            
          //}
          doorInner.Enabled = true;
          sideDoor.Enabled = true;
          doorOuter.Enabled = false;
        }
      // Pressurizing Space Logic
        else if (doorOuter.Status == DoorStatus.Closed && doorInner.Status == DoorStatus.Closed && sideDoor.Status == DoorStatus.Closed&& airVentObj.Status == VentStatus.Pressurizing)
        {
          doorInner.CloseDoor();
          sideDoor.CloseDoor();
          doorOuter.CloseDoor();
          doorInner.Enabled = false;
          doorOuter.Enabled = false;
          sideDoor.Enabled = false;
        }

        else
        {
          doorInner.Enabled = true;
          doorOuter.Enabled = true;
          sideDoor.Enabled = true;
        }      
    }
  }
}
