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
      string rotorName = "Advanced Rotor Driller";
      IMyPistonBase PistonOne = GridTerminalSystem.GetBlockWithName("P1") as IMyPistonBase;
     // IMyPistonBase PistonTwo = GridTerminalSystem.GetBlockWithName("Pi2") as IMyPistonBase;
      //IMyPistonBase PistonThree = GridTerminalSystem.GetBlockWithName("P3") as IMyPistonBase;
      
      IMyMotorAdvancedStator rotor;

      rotor = GridTerminalSystem.GetBlockWithName(rotorName) as IMyMotorAdvancedStator;
      if (rotor == null)
      {
        Echo("Rotor Block Not Found");
        return;
      }
      //180/piRads
      double rotorAngle = rotor.Angle * (180 / Math.PI);
      //double rotorAngle = 180 / (Math.PI * rotor.Angle);
      Echo("Rotor Angle: " + rotorAngle);
      if (rotorAngle < 40 && rotorAngle > 39.9)
      {
        if (PistonOne.CurrentPosition < 10)
        {
          PistonOne.MaxLimit += 1;
        }
        else if (PistonTwo.CurrentPosition < 10)
        {
          PistonTwo.MaxLimit += 1;
        }
        //else if (PistonThree.CurrentPosition < 10)
        //{
        //  PistonThree.MaxLimit += 1;
        //}
      }
    }
  }
}
