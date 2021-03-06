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
      double levelElevation = 10000.00;
      double slowElevation = 8000;
      double stopVelocity = 1000;


      IMyRemoteControl remoteControl = GridTerminalSystem.GetBlockWithName("Remote Control") as IMyRemoteControl;
      IMyGyro gyroscope = GridTerminalSystem.GetBlockWithName("Gyroscope") as IMyGyro;
      IMyCockpit cockpit = GridTerminalSystem.GetBlockWithName("Control Seat") as IMyCockpit;
      IMyProgrammableBlock autoLevel = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Prog Block Auto Level");
      Echo(remoteControl.CustomName);
      Echo(gyroscope.CustomName);
      StringBuilder sb = new StringBuilder();

      double basicVel = remoteControl.GetShipSpeed();
      double elevation;
      remoteControl.TryGetPlanetElevation(MyPlanetElevation.Surface, out elevation);

      sb.AppendLine($"Vel: {basicVel}\n Elev: {elevation.ToString("0.00")}");
      
        if (stopVelocity <= elevation) sb.AppendLine($"Level Limit Reached");
        else if (slowElevation <= elevation) sb.AppendLine($"Level Limit Reached");
        else if (levelElevation <= elevation) sb.AppendLine($"Level Limit Reached");

        Write(sb.ToString(), cockpit);
      

    }
    void Write(string text, IMyCockpit cockpit, bool apnd = false, int screenNumber = 0)
    {
      //For Control Seat Screens:
      //    0 - Center
      //    1 - Top Left
      //    2 - Top Right
      //    3 - Bottom  Left
      //    4 - Top Left
      IMyTextSurface screen = ((IMyTextSurfaceProvider)cockpit).GetSurface(screenNumber);
      screen.WriteText(text);
    }
    
  }
}
