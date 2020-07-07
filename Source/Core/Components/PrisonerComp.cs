using System;
using HugsLib;
using PrisonLabor.Core.AI.WorkGivers;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;
using Verse.Noise;

namespace PrisonLabor.Core.Components
{
    public partial class PrisonerComp : ThingComp
    {
        private bool initliazed = false;
        private bool derefrenced = false;

        private bool isWarden = false;
        private bool isPrisoner = false;

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

        public void TickSwitch()
        {
            if (isPrisoner)
                TickLongPrisoner();
            else
                TickLongWarden();
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

            isWarden = true;
        }

        private void RegisterPrisoner()
        {
            lock (Tracked.LOCK_WARDEN)
            {
                pawn.UpdateInspiration();

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
            this.isPrisoner = true;
        }

        private void Reregister(bool unregister = false)
        {
            lock (Tracked.LOCK_WARDEN)
            {
                Tracked.index[id] = -1;

                if (pawn.IsPrisoner)
                    this.isWarden = false;
                else if (pawn.IsFreeColonist)
                    this.isPrisoner = false;

                this.CleanUp();
#if TRACE
                Log.Message("Unregisterd pawn: " + pawn.Name.ToStringFull);
#endif
            }
        }

        private void Unregister(bool unregister = false)
        {
            lock (Tracked.LOCK_WARDEN)
            {
                this.CleanUp();
                if (pawn?.Dead ?? true || pawn == null)
                {
                    Tracked.index.Remove(id);

                    derefrenced = true;

                    if (parent != null)
                        parent.AllComps.RemoveAll(x => x == this);

                    if (HugsLibController.Instance.DistributedTicker.IsRegistered(parent))
                        HugsLibController.Instance.DistributedTicker.UnregisterTickability(parent);
                }
#if TRACE
                Log.Message("Unregisterd pawn: " + pawn.Name.ToStringFull);
#endif                
            }
        }

        private void CleanUp()
        {
            foreach (int roomid in Tracked.Prisoners.Keys)
                Tracked.Prisoners[roomid].RemoveAll(x => x == id);

            foreach (int roomid in Tracked.Wardens.Keys)
                Tracked.Wardens[roomid].RemoveAll(x => x == id);
        }

        public override void CompTickRare()
        {
            if (derefrenced)
                return;

            if (this.pawn == null)
                this.pawn = (Pawn)this.parent;

            if (this.pawn.Dead)
            {
                this.Unregister();
            }
            else
            {
                if (!initliazed)
                {
                    if (this.pawn.RaceProps.Animal || !this.pawn.RaceProps.Humanlike)
                    {
                        derefrenced = true; return;
                    }
                    else
                    {
                        initliazed = true;
                        HugsLibController.Instance.DistributedTicker.RegisterTickability(TickSwitch, 500, pawn);
                    }
                }

                if (pawn.IsPrisonerOfColony)
                    this.RegisterPrisoner();
                else if (pawn.IsFreeColonist)
                    this.RegisterWarden();

                if (isPrisoner && isWarden)
                    this.Reregister();
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
    }
}
