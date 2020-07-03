﻿using System;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;

namespace PrisonLabor.Core.Components
{
    public class PrisonerProperties : CompProperties
    {
        public PrisonerProperties()
        {
            this.compClass = typeof(PrisonerComp);
        }
    }

    public class PrisonerComp : ThingComp
    {
        private Pawn pawn;

        public readonly int id;

        private static int idCounter = 0;

        private static readonly Object LOCK_ID = new Object();

        public Thing Parent => parent;

        public PrisonerComp()
        {
            lock (LOCK_ID)
            {
                id = idCounter;
                idCounter++;
            }

            Tracked.pawnComps[id] = this;
            Tracked.index.Add(id, -1);
        }


        private void RegisterWarden()
        {
            lock (Tracked.LOCK_WARDEN)
            {
                if (Tracked.index[id] != -1)
                    Tracked.Wardens[Tracked.index[id]].Remove(id);

                var room = parent.GetRoom();

                if (room == null)
                    Tracked.index[id] = -1;
                else
                {
                    Tracked.Wardens[room.ID].Add(id);
                    Tracked.index[id] = room.ID;
                }
            }
        }

        private void RegisterPrisoner()
        {
            lock (Tracked.LOCK_WARDEN)
            {
                if (Tracked.index[id] != -1)
                    Tracked.Prisoners[Tracked.index[id]].Remove(id);

                var room = parent.GetRoom();

                if (room == null)
                    Tracked.index[id] = -1;
                else
                {
                    Tracked.Prisoners[room.ID].Add(id);
                    Tracked.index[id] = room.ID;
                }
            }
        }

        private void Unregister(bool unregister = false)
        {
            lock (Tracked.LOCK_WARDEN)
            {
                if (Tracked.index[id] == -1)
                {
                    Tracked.index.Remove(id);
                    Tracked.pawnComps.Remove(id);
                }
                else
                {
                    var roomid = Tracked.index[id];

                    if (Tracked.Wardens.ContainsKey(roomid))
                        Tracked.Wardens[roomid].Remove(id);

                    if (Tracked.Prisoners.ContainsKey(roomid))
                        Tracked.Prisoners[roomid].Remove(id);

                    Tracked.index.Remove(id);
                    Tracked.pawnComps.Remove(id);
                }
            }
        }

        public override void CompTickRare()
        {
            if (this.pawn == null)
                this.pawn = (Pawn)this.parent;

            if (pawn.Dead == true || pawn.Corpse != null)
            {
                this.Unregister();
                this.parent.AllComps.Remove(this);
            }
            else
            {
                if (pawn.IsPrisonerOfColony)
                    this.RegisterPrisoner();
                else if (pawn.IsFreeColonist)
                    this.RegisterWarden();
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
        }
    }
}
