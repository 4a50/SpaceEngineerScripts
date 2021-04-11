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
      IMyTextSurface hydDisplay = GridTerminalSystem.GetBlockWithName("Tank LCD Control") as IMyTextSurface;
      IMyTextSurface statDisp = GridTerminalSystem.GetBlockWithName("LCD Stats One") as IMyTextSurface;
      IMyTextSurface[] buttonScreen = ButtonPanelTextAssignments("Button Panel One");
      IMyShipConnector lowerConnector = GridTerminalSystem.GetBlockWithName("Connector Ship Lower") as IMyShipConnector;
      //Left to Right
      buttonScreen[0].WriteText("Close\nAll\nShip\nDoors");
      buttonScreen[3].WriteText($"Lower\nConnector\n{lowerConnector.Status.ToString()}");


      IMyBlockGroup airVents = GridTerminalSystem.GetBlockGroupWithName("Air Vents Ship ALL");
      IMyBlockGroup hydrogenTankBlocks = GridTerminalSystem.GetBlockGroupWithName("Ship Hydrogen Tanks");

      string hydStat = HydrogenTankDisplay(hydrogenTankBlocks);
      string airVentStat = AirTightCheck(airVents, statDisp);

      hydDisplay.WriteText(hydStat);
      statDisp.WriteText(airVentStat);
    }
    string HydrogenTankDisplay(IMyBlockGroup hydrogenTankBlocks)
    {
      List<IMyCargoContainer> cargo = new List<IMyCargoContainer>();
      GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);

      StringBuilder sb = new StringBuilder();

      List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
      hydrogenTankBlocks.GetBlocksOfType<IMyGasTank>(hydrogenTanks);




      sb.AppendLine("Hydrogen Tanks Status\n----------");
      double[] tankValues = new double[hydrogenTanks.Count + 1];
      double totalCapacity = hydrogenTanks[0].Capacity * hydrogenTanks.Count;
      double cumulativeCapacity = 0;


      for (int i = 0; i < hydrogenTanks.Count; i++)
      {
        tankValues[i] = hydrogenTanks[i].FilledRatio;
        cumulativeCapacity += hydrogenTanks[i].FilledRatio * hydrogenTanks[i].Capacity;
      }
      tankValues[hydrogenTanks.Count] = cumulativeCapacity / totalCapacity;

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

      }
      return sb.ToString();

    }
    string AirTightCheck(IMyBlockGroup ventBlocks, IMyTextSurface disp)
    {
      List<IMyAirVent> airVents = new List<IMyAirVent>();
      ventBlocks.GetBlocksOfType<IMyAirVent>(airVents);

      Color fontColor = new Color(0, 150, 0);
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Air Integrity Status");
      sb.AppendLine("--------------------");
      foreach (IMyAirVent av in airVents)
      {
        VentStatus ventStatus = av.Status;

        sb.AppendLine($"{av.CustomName}: {ventStatus.ToString()}");
        if (ventStatus == VentStatus.Depressurized || ventStatus == VentStatus.Depressurizing)
        {
          disp.FontColor = Color.Red;
        }
        else { disp.FontColor = fontColor; }
      }
      return (sb.ToString());
    }

    IMyTextSurface[] ButtonPanelTextAssignments(string lcdPanelName)
    {
      IMyButtonPanel bp = GridTerminalSystem.GetBlockWithName("Button Panel One") as IMyButtonPanel;
      int numButtonScreens = ((IMyTextSurfaceProvider)bp).SurfaceCount;
      IMyTextSurface[] screens = new IMyTextSurface[numButtonScreens];
      for (int i = 0; i < screens.Length; i++)      
      {
        screens[i] = ((IMyTextSurfaceProvider)bp).GetSurface(i);
        screens[i].BackgroundColor = new Color(0, 80, 255);
        screens[i].FontColor = new Color(255, 255, 255);
        screens[i].Alignment = TextAlignment.CENTER;
        screens[i].FontSize = 2.5f;

      }
      return screens;
    }
  }
}
