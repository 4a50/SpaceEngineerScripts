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
      IMyGasTank ht4 = GridTerminalSystem.GetBlockGroupWithName("Ship Hydrogen Tank 4") as IMyGasTank;
      List<IMyCargoContainer> cargo = new List<IMyCargoContainer>();
      GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);
      cargo[0].
      IMyTextSurface display = GridTerminalSystem.GetBlockWithName("Tank LCD Control") as IMyTextSurface;
      StringBuilder sb = new StringBuilder();
      IMyBlockGroup hydrogenTankBlocks = GridTerminalSystem.GetBlockGroupWithName("Ship Hydrogen Tanks");
      List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
      hydrogenTankBlocks.GetBlocksOfType<IMyGasTank>(hydrogenTanks);
      sb.AppendLine("Hydrogen Tanks Status\n----------");

      // Echo($"Capacity: {ht4.Capacity}");
      // Echo($"FillRatio: {ht4.FilledRatio}");
      double[] tankValues = HydrogenTankDisplay(hydrogenTanks);
      Echo($"Len tankValues: {tankValues.Length}");
      for (int i = 0; i < tankValues.Length; i++)
      {
        if (i != tankValues.Length - 1)
        {
          Echo($"Hydrogen Tank {i + 1}: {Math.Round(tankValues[i] * 100, 2)}%");
          sb.AppendLine($"Tank {i + 1}: {Math.Round(tankValues[i] * 100, 2)}%");
        }
        else
        {
          Echo($"Total Tanks: {Math.Round(tankValues[i] * 100, 2)}%");
          sb.AppendLine($"Total Tanks: {Math.Round(tankValues[i] * 100, 2)}%");
        }
        display.WriteText(sb.ToString());
      }
    }
    double[] HydrogenTankDisplay(List<IMyGasTank> hydTank)
    {
      double[] tankValues = new double[hydTank.Count + 1];
      double totalCapacity = hydTank[0].Capacity * hydTank.Count;
      double cumulativeCapacity = 0;
      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < hydTank.Count; i++)
      {
        tankValues[i] = hydTank[i].FilledRatio;
        cumulativeCapacity += hydTank[i].FilledRatio * hydTank[i].Capacity;
      }
      tankValues[hydTank.Count] = cumulativeCapacity / totalCapacity;

      return tankValues;
    }
  }
}
