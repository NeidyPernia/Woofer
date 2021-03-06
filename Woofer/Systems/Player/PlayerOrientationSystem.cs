﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Util;
using GameBase;
using GameInterfaces.Input.GamePad;
using WooferGame.Input;

namespace WooferGame.Systems.Player
{
    [ComponentSystem("player_orientation_system")]
    class PlayerOrientationSystem : ComponentSystem
    {
        private double deadzone = 0.3;

        public PlayerOrientationSystem()
        {
            this.Watching = new string[] { Component.IdentifierOf<PlayerOrientation>() };
            this.InputProcessing = true;
            this.TickProcessing = true;
        }

        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            Vector2D thumbstick = inputMap.Orientation;

            if(thumbstick.Magnitude >= deadzone)
            {
                foreach(PlayerOrientation po in WatchedComponents)
                {
                    po.Unit = thumbstick.Unit();
                }
            }
        }

        public override void Tick() => base.Tick();
    }
}
