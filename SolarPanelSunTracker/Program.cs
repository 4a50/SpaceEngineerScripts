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
    void Main()
    {

      IMyTextSurface screen = GridTerminalSystem.GetBlockWithName("LCD Solar Array") as IMyTextSurface;
      IMyMotorStator rotorRight = GridTerminalSystem.GetBlockWithName("Rotor Solar Right") as IMyMotorStator;
      IMyMotorStator rotorLeft = GridTerminalSystem.GetBlockWithName("Rotor Solar Left") as IMyMotorStator;
      IMyBlockGroup solarArrayRightBlocks = GridTerminalSystem.GetBlockGroupWithName("Solar Array Right");
      IMyBlockGroup solarArrayLeftBlocks = GridTerminalSystem.GetBlockGroupWithName("Solar Array Left");
      if (solarArrayRightBlocks == null) Echo("[Error] Right Solar Array Group Null");
      if (solarArrayLeftBlocks == null) Echo("[Error] Left Solar Array Group Null");
      //IMyTextSurface surface = GridTerminalSystem.GetBlockWithName("LCD Panel") as IMyTextSurface;
      IMyBatteryBlock battery = GridTerminalSystem.GetBlockWithName("Battery") as IMyBatteryBlock;
      //float pwrNow;
      //float pwrLast;
      double rotorRightAngle = Math.Round((rotorRight.Angle * 180) / 3.14, 2); //1rad × 180/π = 57.296°
      double rotorLeftAngle = Math.Round((rotorLeft.Angle * 180) / 3.14); //1rad × 180/π = 57.296°
      List<IMySolarPanel> solarArrayRight = new List<IMySolarPanel>();
      List<IMySolarPanel> solarArrayLeft = new List<IMySolarPanel>();
      solarArrayRightBlocks.GetBlocksOfType<IMySolarPanel>(solarArrayRight);
      solarArrayLeftBlocks.GetBlocksOfType<IMySolarPanel>(solarArrayLeft);
      float solarRightInPower = (float)Math.Round(solarArrayRight[0].CurrentOutput * 1000, 2); //kW
      float solarLeftInPower = (float)Math.Round(solarArrayLeft[0].CurrentOutput * 1000); //kW      
      float lastPowerReadingLeft = 0;
      float lastPowerReadingRight = 0;
      bool leftRotating = false;
      bool rightRotating = true;
      StringBuilder sb = new StringBuilder();

      sb.AppendLine($"Total Power Generated: {GetTotalPower(solarArrayLeft, solarArrayRight)} kW");
      //sb.AppendLine($"Right Array Solar Count: {solarArrayRight.Count}");
      sb.AppendLine($"Left Array Solar Count: {solarArrayLeft.Count}");
      //sb.AppendLine($"Right Panel Sample Charge: {Math.Round(solarRightInPower, 2)} kw");
      sb.AppendLine($"Left Panel Sample Charge: {Math.Round(solarLeftInPower, 2)} kw");
      sb.AppendLine($"Left Rotor Angle: {rotorLeftAngle}");
      sb.AppendLine($"Left Rotor Angle Rounded: {Math.Round(rotorLeftAngle)}");
      if (Storage == null)
      {
        sb.AppendLine("Storage Empty");
        lastPowerReadingLeft = 0;
        lastPowerReadingRight = 0;
      }
      else
      {
        string[] retrievedFromStorage = Storage.Split(',');
        sb.AppendLine($"Length Of Storage: {retrievedFromStorage.Length}");
        sb.AppendLine($"StorageLeft Reading: {retrievedFromStorage[0]} isRotatingLeft: {retrievedFromStorage[2]}");
        if (retrievedFromStorage.Length < 4)
        {
          sb.AppendLine("Storage String Cannot be Parsed");
        }
        else
        {
          sb.AppendLine($"{retrievedFromStorage[0]} {retrievedFromStorage[1]} {retrievedFromStorage[2]} {retrievedFromStorage[3]}");
          lastPowerReadingLeft = float.Parse(retrievedFromStorage[0]);
          lastPowerReadingRight = float.Parse(retrievedFromStorage[1]);
          leftRotating = bool.Parse(retrievedFromStorage[2]);
          rightRotating = bool.Parse(retrievedFromStorage[3]);
        }
      }
      //If no sun return to 0 degrees
      if (solarLeftInPower == 0)
      {
        sb.AppendLine("Setting Left Array");
        if (rotorLeftAngle == 45)
        {
          rotorLeft.TargetVelocityRPM = 0;
          leftRotating = false;
        }
        else if (rotorLeftAngle > 180) { rotorLeft.TargetVelocityRPM = 1; }
        else { rotorLeft.TargetVelocityRPM = -1; }
      }
      else if (solarLeftInPower < lastPowerReadingLeft)
      {
        sb.AppendLine($"LeftIn Power < lastPower Reading");
        if (leftRotating)
        {
          rotorLeft.ApplyAction("Reverse");
        }
        else
        {
          rotorLeft.TargetVelocityRPM = -.25f;
        }
      }
      else
      {
        sb.AppendLine($"LeftIn Power >= lastPower Reading");
        sb.AppendLine("Stopped Left Array");
        rotorLeft.TargetVelocityRPM = 0;
      }
      string storageString = $"{solarLeftInPower},{solarRightInPower},{leftRotating.ToString()},{rightRotating.ToString()}";
      Storage = storageString;
      screen.WriteText(sb.ToString());
    }

    float GetTotalPower (List<IMySolarPanel> left, List<IMySolarPanel> right)
    {
      float totalPower = 0;
      foreach (IMySolarPanel sp in left) 
      {
        Echo(sp.CustomName);
        totalPower += sp.CurrentOutput; 
      }
      foreach (IMySolarPanel sp in right) { totalPower += sp.CurrentOutput; }
    
      return totalPower;
    }
    public class SolarArray
    {
      IMyMotorStator Rotor { get; set; }  
      double RotorAngleDegrees { get; set; }
      List<IMySolarPanel> SolarArrayPanelList { get; set; }
      float SolarInPower { get; set; }
      float LastPowerReading { get; set; }
      bool IsRotating { get; set; }
      StringBuilder sb = new StringBuilder();

    }
  }
}
