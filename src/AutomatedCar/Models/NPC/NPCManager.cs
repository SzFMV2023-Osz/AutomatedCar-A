﻿namespace AutomatedCar.Models.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NPCManager : GameBase
    {
        private List<INPC> npcList;

        public NPCManager()
        {
            this.npcList = new List<INPC>();
        }

        public void AddNPC(INPC npc)
        {
            this.npcList.Add(npc);
        }

        protected override void Tick()
        {
            foreach (var npc in this.npcList)
            {
                npc.Move();
            }
        }
    }
}