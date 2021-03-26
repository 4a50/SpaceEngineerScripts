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
    // ======= ARCHON'S AUTOLEVEL SCRIPT ======  
    /* Use instructions:  
    1. Ensure that the ship has a remote control (if there are multiple, you can use the REMOTE_CONTROL_NAME variable to select a specific controller) and at least one gyroscope  
    1.1 Ensure that the remote control is correctly orientated (the round light should be facing "forwards" relative to your ship 
    2. Change the script variables to suit your preferences :  
    2.1 - CTRL_COEFF is used to define the turn speed, if your ship is overshooting, reduce this, if it is turning too slowly increase it  
    2.2 - LIMIT_GYROS is used to define a maximum number of gyroscopes for the script to use (must be >0).   
        If you want to be able to turn the ship whilst the script is running, set this to a lower value than the number of gyroscopes on your ship  
    2.3 - stopWhenLevel is used to decide whether the script should stop running once the ship is level (true) or continue to run (false)  
    2.4 - If you want the ship to still be controllable (i.e. to allow you to turn - you can set cockpitFeaturesEnabled to true 
    3. Run the script with any argument other than an empty one (e.g. "r", "start", "go" but don't leave the box empty) 
    4. If stopWhenLevel is false, you can stop the script by running it again with the same non-empty argument as described in (3) 

    Note - Please let me know in the comments on the script if you run into any issues and I'll get them fixed as soon as I can  
    */

    string REMOTE_CONTROL_NAME = ""; //SETS THE NAME OF THE REMOTE CONTROL TO ORIENT ON (e.g. "Remote Control 1") 
                                     //LEAVE AS "" TO USE FIRST REMOTE CONTROL FOUND 
    double CTRL_COEFF = 0.3; //SETS THE TURN SPEED, LOWER IF YOU OVERSHOOT 
                             //RAISE IF TURNING TOO SLOWLY 
    int LIMIT_GYROS = 999; //SET THIS TO A NUMBER LOWER THAN 
                           //THE NUMBER OF GYROS YOU HAVE IF YOU 
                           //WANT TO BE ABLE TO ROTATE THE SHIP WHILST LEVELLING 
    bool stopWhenLevel = false; //SET THIS AS TRUE IF YOU WANT THE SCRIPT TO STOP WHEN THE SHIP IS LEVEL 

    bool cockpitFeaturesEnabled = false; //SET THIS AS TRUE IF YOU WANT THE SCRIPT TO TEMPORARILY DISABLE 
                                         //SO YOU CAN TURN THE SHIP 

    //============================== 
    // DO NOT EDIT THE CODE BELOW 
    //============================== 

    bool running = false;
    public Program()
    {
      Runtime.UpdateFrequency = UpdateFrequency.None;
      //First time setup 
      setup();
    }

    IMyShipController cockpit;
    IMyRemoteControl remoteControl;
    List<IMyGyro> gyroList;
    double ang;

    void Main(string argument)
    {
      //If there is no remote control or reset is run then setup params 
      if (remoteControl == null) setup();
      //If running and argument is passed then stop 
      else if (running && argument != "") stop();
      //If stop when level is enabled and level then stop 
      else if (running && ang < 0.01 && stopWhenLevel) stop();
      //If cockpit controls are enabled and movement is detected then stop temporarily 
      else if (running && (cockpitFeaturesEnabled && (cockpit.MoveIndicator.Sum != 0 || cockpit.RollIndicator != 0 || cockpit.RotationIndicator.X + cockpit.RotationIndicator.Y != 0))) tempStop();
      //Otherwise setup and level the ship 
      else
      {
        setup();
        level();
      }
    }

    void level()
    {
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
      //Get orientation from remoteControl  
      Matrix orientation;
      remoteControl.Orientation.GetMatrix(out orientation);
      Vector3D down = orientation.Down;
      Vector3D grav = remoteControl.GetNaturalGravity();
      grav.Normalize();
      running = true;

      for (int i = 0; i < gyroList.Count; ++i)
      {
        var g = gyroList[i];

        g.Orientation.GetMatrix(out orientation);
        var localDown = Vector3D.Transform(down, MatrixD.Transpose(orientation));

        var localGrav = Vector3D.Transform(grav, MatrixD.Transpose(g.WorldMatrix.GetOrientation()));

        var rot = Vector3D.Cross(localDown, localGrav);
        ang = rot.Length();
        ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));
        double ctrl_vel = g.GetMaximum<float>("Yaw") * (ang / Math.PI) * CTRL_COEFF;

        ctrl_vel = Math.Min(g.GetMaximum<float>("Yaw"), ctrl_vel);
        ctrl_vel = Math.Max(0.01, ctrl_vel); //Gyros don't work well at very low speeds 
        rot.Normalize();
        rot *= ctrl_vel;
        g.SetValueFloat("Pitch", (float)rot.GetDim(0));
        g.SetValueFloat("Yaw", -(float)rot.GetDim(1));
        g.SetValueFloat("Roll", -(float)rot.GetDim(2));

        g.SetValueFloat("Power", 1.0f);
        g.SetValueBool("Override", true);
      }
    }

    void stop()
    {
      Echo("Stopping");
      for (int i = 0; i < gyroList.Count; ++i)
      {
        var g = gyroList[i];
        g.SetValueBool("Override", false);
        running = false;
        Runtime.UpdateFrequency = UpdateFrequency.None;
      }
    }

    void tempStop()
    {
      for (int i = 0; i < gyroList.Count; ++i)
      {
        var g = gyroList[i];
        g.SetValueBool("Override", false);
      }
    }


    void setup()
    {

      var list = new List<IMyTerminalBlock>();

      remoteControl = (IMyRemoteControl)GridTerminalSystem.GetBlockWithName(REMOTE_CONTROL_NAME);
      if (remoteControl == null)
      {
        GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(list, x => x.CubeGrid == Me.CubeGrid);
        if (list.Count == 0)
        {
          Me.CustomData = Me.CustomData + "\nError: No Remote Control Blocks Found on the current ship";
        }
        remoteControl = (IMyRemoteControl)list[0];
      }

      GridTerminalSystem.GetBlocksOfType<IMyGyro>(list, x => x.CubeGrid == Me.CubeGrid);
      if (list.Count == 0)
      {
        Me.CustomData = Me.CustomData + "\nError: No Gyroscope Blocks Found on the current ship";
      }
      gyroList = list.ConvertAll(x => (IMyGyro)x);
      if (gyroList.Count > LIMIT_GYROS)
        gyroList.RemoveRange(LIMIT_GYROS, gyroList.Count - LIMIT_GYROS);

      //Find a main cockpit to enable the  
      List<IMyShipController> controllers = new List<IMyShipController>();
      GridTerminalSystem.GetBlocksOfType<IMyShipController>(controllers, x => x.CubeGrid == Me.CubeGrid);
      for (int i = controllers.Count - 1; i >= 0; i--)
      {
        if (!controllers[i].IsUnderControl) controllers.RemoveAt(i);
      }
      if (controllers.Count > 1)
      {
        foreach (IMyShipController controller in controllers)
        {
          if (controller.IsMainCockpit) cockpit = controller;
        }
      }
      else if (controllers.Count == 1) cockpit = controllers[0];
      if (cockpit == null)
      {
        Me.CustomData = Me.CustomData + "\nWarning: Either there is no cockpit on the ship or there are multiple " +
            "occupied cockpits with no Main Cockpit assigned, cockpit control features have been disabled";
        cockpitFeaturesEnabled = false;
        return;
      }
    }
  }
}
