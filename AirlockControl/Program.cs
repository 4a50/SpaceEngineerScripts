﻿using Sandbox.Game.EntityComponents;
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
    // This file contains your actual script.
    //
    // You can either keep all your code here, or you can create separate
    // code files to make your program easier to navigate while coding.
    //
    // In order to add a new utility class, right-click on your project, 
    // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
    // category under 'Visual C# Items' on the left hand side, and select
    // 'Utility Class' in the main area. Name it in the box below, and
    // press OK. This utility class will be merged in with your code when
    // deploying your final script.
    //
    // You can also simply create a new utility class manually, you don't
    // have to use the template if you don't want to. Just do so the first
    // time to see what a utility class looks like.
    // 
    // Go to:
    // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
    //
    // to learn more about ingame scripts.

    public Program()
    {
      Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }

    public void Save()
    {
      // Called when the program needs to save its state. Use
      // this method to save your state to the Storage field
      // or some other means. 
      // 
      // This method is optional and can be removed if not
      // needed.
    }

    public void Main(string argument, UpdateType updateSource)
    {
      string doorOne = "AirlockOne";
      string doorTwo = "AirlockTwo";

      var doorOneObj = GridTerminalSystem.GetBlockWithName(doorOne) as IMyDoor;
      var doorTwoObj = GridTerminalSystem.GetBlockWithName(doorTwo) as IMyDoor;

      Echo($"{doorOneObj.CustomName}: {doorOneObj.Status}");
      Echo($"{doorOneObj.CustomName}: {doorTwoObj.Status}");
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
        else
        {
          doorTwoObj.Enabled = true;
          doorOneObj.Enabled = true;
        }
      }
    }
  }
}
