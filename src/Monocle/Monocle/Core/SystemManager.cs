﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle.Utils;

namespace Monocle.Core
{
    public interface ISystemManager
    {
        void AddLogicSystem(ISystem system);
        void AddRenderSystem(IRenderSystem system);

        void Update(Time time);
        void Draw(Time time);
    }

    public class SystemManager : ISystemManager
    {
        private readonly List<ISystem> logicSystems;
        private readonly List<IRenderSystem> renderSystems;

        public SystemManager()
        {
            logicSystems = new List<ISystem>();
            renderSystems = new List<IRenderSystem>();
        }

        public void AddLogicSystem(ISystem system)
        {
            if (system == null)
                throw new ArgumentNullException("system");

            logicSystems.Add(system);
        }

        public void AddRenderSystem(IRenderSystem system)
        {
            if (system == null)
                throw new ArgumentNullException("system");
        }

        public void Update(Time time)
        {
            //TODO: Add code for multithreading here. 
            foreach (var system in logicSystems)
            {
                if(system.Enabled)
                    system.Exceute(time);
            }
        }

        public void Draw(Time time)
        {
            throw new NotImplementedException();
        }
    }
}