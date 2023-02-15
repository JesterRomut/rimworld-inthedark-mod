using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using static System.Net.Mime.MediaTypeNames;
using static HarmonyLib.Code;
using static UnityEngine.GraphicsBuffer;

namespace InTheDark
{
    public class VoidSpawnGroupManager : GameComponent
    {
        private static VoidSpawnGroupManager _instance;
        private List<VoidSpawnControlGroup> controlGroups = new List<VoidSpawnControlGroup>();
        public HashSet<Pawn> AllVoidSpawns
        {
            get
            {
                HashSet<Pawn> allVoidSpawns = new HashSet<Pawn>();
                foreach (VoidSpawnControlGroup controlGroup in controlGroups) {
                    allVoidSpawns.UnionWith(controlGroup.PawnsForReading);
                }
                return allVoidSpawns;
            }
        }
        public List<VoidSpawnControlGroup> ControlGroups
        {
            get { return controlGroups; }
        }

        public VoidSpawnGroupManager() {
            _instance = this;
            if (!controlGroups.Any())
            {
                controlGroups.Insert(0, new VoidSpawnControlGroup());
            }
        }

        public VoidSpawnGroupManager(Game game)
        : this()
        {
        }

        public static VoidSpawnGroupManager Main
        {
            get
            {
                if (_instance == null)
                {
                    throw new NullReferenceException("Accessing VoidSpawnGroupManager before it was constructed.");
                }
                return _instance;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            PurgeControlGroups();
            Scribe_Collections.Look(ref controlGroups, "controlGroups", LookMode.Deep, this);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (controlGroups == null)
                {
                    controlGroups = new List<VoidSpawnControlGroup>();
                }
            }
        }

        public VoidSpawnControlGroup GetControlGroup(Pawn pawn)
        {
            foreach (VoidSpawnControlGroup controlGroup in controlGroups)
            {
                if (controlGroup.PawnsForReading.Contains(pawn))
                {
                    return controlGroup;
                }
            }
            return null;
        }

        public void PurgeControlGroups()
        {
            controlGroups.RemoveAll((VoidSpawnControlGroup group) => !group.assignedVoidSpawns.Any() && group.GetIndex() != 0);
        }

        public void AddToControlGroups(VoidSpawnControlGroup group)
        {
            PurgeControlGroups();
            controlGroups.Add(group);
        }
    }
    public class AssignedVoidSpawn : IExposable
    {
        public Pawn pawn;


        public AssignedVoidSpawn()
        {
        }

        public AssignedVoidSpawn(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
        }
    }

    public class VoidSpawnControlGroup : IExposable
    {
        public HashSet<AssignedVoidSpawn> assignedVoidSpawns = new HashSet<AssignedVoidSpawn>();
        public VoidSpawnGroupManager component = VoidSpawnGroupManager.Main;
        public string Name {
            get {
                int index = GetIndex();
                if (index == -1)
                {
                    return "Error";
                }
                if (index == 0)
                {
                    return "LabelVoidSpawnGroupTemplate".Translate("LabelVoidSpawnGroupDefault".Translate(index));
                }
                return "LabelVoidSpawnGroupTemplate".Translate(index); 
            }
        }
        public VoidSpawnControlGroup()
        {

        }
        public VoidSpawnControlGroup(VoidSpawnGroupManager component)
        {
            this.component = component;
        }
        public HashSet<Pawn> PawnsForReading
        {
            get
            {
                HashSet<Pawn> assignedForReading = new HashSet<Pawn>();
                foreach (AssignedVoidSpawn voidSpawn in assignedVoidSpawns)
                {
                    assignedForReading.Add(voidSpawn.pawn);
                }
                return assignedForReading;
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref assignedVoidSpawns, "assignedVoidSpawns", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                assignedVoidSpawns.RemoveWhere((AssignedVoidSpawn x) => x.pawn == null);
            }
        }
        public void Assign(Pawn pawn)
        {
            foreach (VoidSpawnControlGroup controlGroup in VoidSpawnGroupManager.Main.ControlGroups)
            {
                controlGroup.TryUnassign(pawn);
            }
            assignedVoidSpawns.Add(new AssignedVoidSpawn(pawn));
        }
        public bool TryUnassign(Pawn pawn)
        {
            VoidSpawnGroupManager.Main.PurgeControlGroups();
            return assignedVoidSpawns.RemoveWhere((AssignedVoidSpawn x) => x.pawn == pawn) > 0;
        }
        public int GetIndex()
        {
            return VoidSpawnGroupManager.Main.ControlGroups.IndexOf(this);
        }
    }
    public class VoidSpawnControlGroupGizmo : Command
    {
        public VoidSpawnControlGroup controlGroup = null;
        public Pawn pawn = null;

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions => pawn != null ? GetControlGroupOptions(pawn) : null;

        public VoidSpawnControlGroupGizmo(Pawn pawn)
        {
            if (pawn != null)
            {
                this.pawn = pawn;
                controlGroup = VoidSpawnGroupManager.Main.GetControlGroup(pawn);
                this.icon = Startup.Textures.VoidSpawnControlGroupGizmo;
                this.defaultLabel = controlGroup.Name;
                this.defaultDesc = "CommandVoidSpawnGroupingDesc".Translate();
            }
            
        }
        public override void ProcessInput(Event ev)
        { // 0 left 1 right
            base.ProcessInput(ev);
            if (ev.button == 0)
            {
                Find.Selector.ClearSelection();
                foreach (Pawn voidSpawn in controlGroup.PawnsForReading)
                {
                    Find.Selector.Select(voidSpawn);
                }
            }
            Log.Message(string.Concat(ev.button, " ", ev.type, " ", ev.character));
        }

        public override bool GroupsWith(Gizmo other)
        {
            if (!(other is VoidSpawnControlGroupGizmo controlGroupGizmo))
            {
                return false;
            }
            if (controlGroupGizmo.controlGroup == controlGroup)
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<FloatMenuOption> GetControlGroupOptions(Pawn pawn)
        {
            VoidSpawnGroupManager.Main.PurgeControlGroups();
            foreach (VoidSpawnControlGroup cg in VoidSpawnGroupManager.Main.ControlGroups)
            {
                if (cg == VoidSpawnGroupManager.Main.GetControlGroup(pawn))
                {
                    yield return new FloatMenuOption("CommandVoidSpawnGroupCannotTemplate".Translate(cg.Name) + ": " + "CommandVoidSpawnGroupAlreadyAssigned".Translate(), null);
                    continue;
                }
                
                FloatMenuOption tmpSelectGroup = new FloatMenuOption("CommandVoidSpawnGroupSelectTemplate".Translate(cg.Name), delegate
                {
                    cg?.Assign(pawn);
                });
                //floatMenuOption.tooltip = new TipSignal(wm.description, cg.GetIndex() ^ 0xDFE6666);
                yield return tmpSelectGroup;
            }
            if (Find.Selector.SelectedPawns.Count <= 1) { 
                FloatMenuOption tmpCreateNewGroup = new FloatMenuOption("CommandVoidSpawnGroupCreateTemplate".Translate(), delegate
                {
                    VoidSpawnControlGroup cg = new VoidSpawnControlGroup();
                    cg.Assign(pawn);
                    VoidSpawnGroupManager.Main.AddToControlGroups(cg);
                });
                yield return tmpCreateNewGroup;
            }
        }
        //public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        //{
        //    HashSet<Pawn> voidSpawnsForReading = controlGroup.PawnsForReading;

        //    Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        //    Rect rect2 = rect.ContractedBy(6f);
        //    bool flag = Mouse.IsOver(rect2);
        //    if (controlGroup == null)
        //    {
        //        Widgets.Label(rect2, "not in group");
        //        return new GizmoResult(flag ? GizmoState.Mouseover : GizmoState.Clear);
        //    }
        //    GUI.color = (parms.lowLight ? Command.LowLightBgColor : Color.white);
        //    Material material = ((disabled || parms.lowLight || !controlGroup.assignedVoidSpawns.Any()) ? TexUI.GrayscaleGUI : null);
        //    GenUI.DrawTextureWithMaterial(rect, parms.shrunk ? Command.BGTexShrunk : Command.BGTex, material);
        //    GUI.color = Color.white;
        //    Text.Font = GameFont.Small;
        //    Text.Anchor = TextAnchor.UpperLeft;
        //    Rect rect3 = rect2;
        //    TaggedString str = controlGroup.Name;
        //    str = str.Truncate(rect2.width);
        //    Vector2 vector = Text.CalcSize(str);
        //    rect3.width = vector.x;
        //    rect3.height = vector.y;
        //    Widgets.Label(rect3, str);
        //    if (Mouse.IsOver(rect3))
        //    {
        //        Widgets.DrawHighlight(rect3);
        //        if (Widgets.ButtonInvisible(rect3))
        //        {
        //            Find.Selector.ClearSelection();
        //            foreach (Pawn voidSpawn in voidSpawnsForReading)
        //            {
        //                Find.Selector.Select(voidSpawn);
        //            }
        //        }
        //    }
        //    Rect rect6 = new Rect(rect2.x, rect2.y + 26f + 4f, rect2.width, rect2.height - 26f - 4f);
        //    float num = rect6.height;
        //    float num2 = Mathf.FloorToInt(rect6.width / num);
        //    float num3 = Mathf.FloorToInt(rect6.height / num);
        //    float num5 = (rect6.width - (float)num2 * num) / 2f;
        //    float num6 = (rect6.height - (float)num3 * num) / 2f;
        //    Rect rect7 = new Rect(rect6.x + num5, rect6.y + num + num6, num, num);
        //    RenderTexture image = PortraitsCache.Get(pawn, rect7.size, Rot4.East, default(Vector3), pawn.kindDef.controlGroupPortraitZoom);
        //    return new GizmoResult(flag ? GizmoState.Mouseover : GizmoState.Clear);
        //}
        //public override float GetWidth(float maxWidth)
        //{
        //    return 130f;
        //}
    }
    public class ThoughtWorker_VoidSpawnThoughtSync : ThoughtWorker
    {
        private float? average = null;
        //float? strippedMood = null;
        private float GetStrippedMood(Pawn p)
        {
            List<Thought> thoughts = new List<Thought>();
            //p.needs.mood.thoughts.situational.AppendMoodThoughts(thoughts);
            p.needs.mood.thoughts.GetAllMoodThoughts(thoughts);
            float totalMood = 0f;
            foreach (Thought t in thoughts)
            {
                if (t.def == VoidSpawnThoughtDefOf.VoidSpawnThoughtSync)
                {
                    continue;
                }
                totalMood += t.MoodOffset();
            }
            //if (p.IsColonist || p.IsPrisonerOfColony)
            //{
            //    totalMood += Find.Storyteller.difficulty.colonistMoodOffset;
            //}
            return totalMood / 100f;
        }

        private float CalculateAverage(Pawn p, HashSet<Pawn> voidSpawns)
        {
            float total = 0f;
            float count = 0f;
            foreach (Pawn siren in voidSpawns)
            {
                total += GetStrippedMood(siren);
                count++;
            }
            return total / count;
        }

        public override string PostProcessLabel(Pawn p, string label)
        {
            return string.Concat(base.PostProcessLabel(p, label), " (", VoidSpawnGroupManager.Main.GetControlGroup(p).Name, ")");
        }
        //public override string PostProcessDescription(Pawn p, string description)
        //{
        //    return base.PostProcessDescription(p, description) + string.Concat(" In collectionclass: ", VoidSpawnCollectionClass.void_spawns.Contains(p), " all in collection: ", string.Join(",", VoidSpawnCollectionClass.void_spawns));
        //}
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (p.def != VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                return ThoughtState.Inactive;
            }
            VoidSpawnControlGroup group = VoidSpawnGroupManager.Main.GetControlGroup(p);
            if (group == null)
            {
                return ThoughtState.Inactive;
            }
            HashSet<Pawn> set = group.PawnsForReading;
            //Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(def.hediff);
            if (set.Count() > 1 && set.Contains(p))
            {
                return true;

            }
            return ThoughtState.Inactive;
        }
        public override float MoodMultiplier(Pawn p)
        {
            VoidSpawnControlGroup group = VoidSpawnGroupManager.Main.GetControlGroup(p);
            if (group == null)
            {
                return 0f;
            }

            average = CalculateAverage(p, group.PawnsForReading);
            return average.Value - GetStrippedMood(p);
        }

    }

    public class ThoughtWorker_VoidSpawnSocialBase : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (p.def != VoidSpawnThingDefOf.VoidSpawn_Race)
            {
                return false;
            }
            if (p.def != other.def)
            {
                return false;
            }
            
            return true;
        }
    }
    public class ThoughtWorker_VoidSpawnGestalt: ThoughtWorker_VoidSpawnSocialBase
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!base.CurrentSocialStateInternal(p, other).Active)
            {
                return false;
            }
            if (VoidSpawnGroupManager.Main.GetControlGroup(p) != VoidSpawnGroupManager.Main.GetControlGroup(other))
            {
                return false;
            }
            return true;
        }
    }
}
