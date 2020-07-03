using System;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;

namespace PrisonLabor.Core.Components
{
    public class PrisonerComp : ThingComp
    {
        private bool derefrenced = false;

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

#if TRACE
                Log.Message("Unregisterd pawn: " + pawn.Name.ToStringFull);
#endif
            }
        }

        public override void CompTickRare()
        {
            if (derefrenced)
                return;

            if (this.pawn == null)
                this.pawn = (Pawn)this.parent;

            if (this.pawn.RaceProps.Animal || !this.pawn.RaceProps.Humanlike)
            {
                derefrenced = true;
                return;
            }

            if (this.pawn.RaceProps.Animal || !this.pawn.RaceProps.Humanlike)
            {
                derefrenced = true;
                return;
            }

            if (pawn.IsPrisoner)
                this.RegisterPrisoner();
            else if (pawn.IsColonist)
                this.RegisterWarden();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            this.Unregister();
        }
    }
}
