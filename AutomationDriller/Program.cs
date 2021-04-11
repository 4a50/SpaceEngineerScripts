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
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }   

    public void Main(string argument, UpdateType updateSource)
    {
      
      IMyPistonBase PistonOne = GridTerminalSystem.GetBlockWithName("Piston") as IMyPistonBase;
      IMyPistonBase PistonTwo = GridTerminalSystem.GetBlockWithName("Piston 2") as IMyPistonBase;
      IMyPistonBase PistonThree = GridTerminalSystem.GetBlockWithName("Piston 3") as IMyPistonBase;
      IMyCargoContainer StorageBlock = GridTerminalSystem.GetBlockWithName("Large Cargo Container") as IMyCargoContainer;
      IMyMotorAdvancedStator Rotor = GridTerminalSystem.GetBlockWithName("Advanced Rotor") as IMyMotorAdvancedStator;
      IMyBlockGroup Drills = GridTerminalSystem.GetBlockGroupWithName("Drillers");
      IMyProgrammableBlock programmableBlock = GridTerminalSystem.GetBlockWithName("Programmable block") as IMyProgrammableBlock;
      IMyTextSurface screen = ((IMyTextSurfaceProvider)programmableBlock).GetSurface(0);
      screen.WriteText("1234");

      if (Drills == null)
      {
        Echo("Drillers group not found");
        return; 
      }
      List<IMyTerminalBlock> listBlocks = new List<IMyTerminalBlock>();
      Drills.GetBlocks(listBlocks);
      
      Echo($"{PistonOne.CustomName}");
      Echo($"{PistonTwo.CustomName}");
      Echo($"{PistonThree.CustomName}");
      Echo($"{StorageBlock.CustomName}");
      Echo($"{Rotor.CustomName}");
      Echo($"Number of Drills: {listBlocks.Count}");
      Echo($"Current Mass of {StorageBlock.CustomName}: {StorageBlock.Mass}");
      var storageBlock = StorageBlock.GetInventory();
      MyFixedPoint storageBlockVolume = storageBlock.CurrentVolume;
      float volume = (float)storageBlockVolume;
      float maxVolume = (float)storageBlock.MaxVolume;
      Echo($"Current Volume: {volume}");      

      if (Rotor == null)
      {
        Echo("Rotor Block Not Found");
        return;
      }
      if (volume >= maxVolume)
      {
        Echo("[WARNING] Cargo Full");
        Rotor.RotorLock = true;
        foreach (var drill in listBlocks)
        {
          var d = drill as IMyShipDrill;
          d.Enabled = false;
        }
      }
      else
      {
        Rotor.RotorLock = false;
        foreach (var drill in listBlocks)
        {
          var d = drill as IMyShipDrill;
          d.Enabled = true;
        }
      }
      //180/piRads
      double rotorAngle = Rotor.Angle * (180 / Math.PI);
      //double rotorAngle = 180 / (Math.PI * rotor.Angle);
      Echo("Rotor Angle: " + Math.Round(rotorAngle, 2));
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
        else if (PistonThree.CurrentPosition < 10)
        {
          PistonThree.MaxLimit += 1;
        }
      }
    }
  }
}
