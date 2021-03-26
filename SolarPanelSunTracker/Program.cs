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
      // The constructor, called only once every session and
      // always before any other method is called. Use it to
      // initialize your script. 
      //     
      // The constructor is optional and can be removed if not
      // needed.
      // 
      // It's recommended to set Runtime.UpdateFrequency 
      // here, which will allow your script to run itself without a 
      // timer block.
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

    void Main()
    {

      var rotor = GridTerminalSystem.GetBlockWithName("Rotor 2") as IMyMotorStator;
      IMyTextSurface surface = GridTerminalSystem.GetBlockWithName("LCD Panel") as IMyTextSurface;
      IMyBatteryBlock battery = GridTerminalSystem.GetBlockWithName("Battery") as IMyBatteryBlock;
      float pwrNow;
      float pwrLast;
      double rotorAngle = (rotor.Angle * 180) / 3.14; //1rad × 180/π = 57.296°

      if (Storage == null)
      {
        Echo("Storage Empty");
        pwrLast = 0;
      }
      else
      {
        pwrLast = float.Parse(Storage);
      }
      float currentInput = (battery.CurrentInput) * 1000;
      Math.Round(currentInput, 2);
      pwrNow = battery.CurrentInput;
      pwrNow = currentInput;
      if (pwrNow == 0)
      {
        if ((rotorAngle < 270 && rotorAngle > 0) && (rotorAngle > 280 && rotorAngle < 360))
        {
          rotor.TargetVelocityRPM = 0;
        }
        else
        {
          rotor.TargetVelocityRPM = 1;

        }

      }
      else if (pwrNow > pwrLast || pwrNow >= 250)
      {
        rotor.SetValueFloat("Velocity", 0);
        rotor.TargetVelocityRPM = 0;
        Storage = pwrNow.ToString();
      }
      else if (pwrNow < pwrLast)
      {
        //rotor.ApplyAction("Reverse");
        if (rotor.TargetVelocityRPM == 0)
        {
          rotor.SetValueFloat("Velocity", .25F);
        }
      }
      surface.WriteText("Rotor Angle: " + rotorAngle + "\nPower Input: " + currentInput.ToString() + "\nPwrLast: " + pwrLast.ToString()
      + "kW\npwrNow: " + pwrNow.ToString() + "kW\n");
    }
  }
}
