using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Soft64WPF
{
    public sealed class ProgramCounterArrowGroup : List<ProgramCounterArrow>
    {
        private static Dictionary<String, ProgramCounterArrowGroup> s_Groups;

        static ProgramCounterArrowGroup()
        {
            s_Groups = new Dictionary<string, ProgramCounterArrowGroup>();
        }

        public static void AddToGroup(String name, ProgramCounterArrow arrow)
        {
            if (!s_Groups.ContainsKey(name))
                s_Groups.Add(name, new ProgramCounterArrowGroup());

            s_Groups[name].Add(arrow);
        }

        public static void RemoveFromGroup(String name, ProgramCounterArrow arrow)
        {
            s_Groups[name].Remove(arrow);

            if (s_Groups[name].Count < 1)
                s_Groups.Remove(name);
        }

        public static void SetActive(String name, Int32 index)
        {
            for (Int32 i = 0; i < s_Groups[name].Count; i++)
            {
                ProgramCounterArrow arrow = s_Groups[name][i];
                Boolean enabled = i == index;

                arrow.Dispatcher.InvokeAsync(() =>
                {
                   arrow.Active = enabled;
                   arrow.InvalidateVisual();
                }, DispatcherPriority.Render);
            }
        }
    }

    public class ProgramCounterArrow : Shape
    {
        private static Geometry m_Geo;
        private Boolean m_Active;
        private String m_GroupName;

        static ProgramCounterArrow()
        {
            m_Geo = Geometry.Parse("F1 M 26.8469,10.7846L 12.8223,0L 12.8223,6.8204L 0,6.8204L 0,14.7495L 12.8223,14.7495L 12.8223,21.5697L 26.8469,10.7846 Z ");
        }

        public ProgramCounterArrow()
        {
            Loaded += ProgramCounterArrow_Loaded;
            Unloaded += ProgramCounterArrow_Unloaded;
        }

        void ProgramCounterArrow_Unloaded(object sender, RoutedEventArgs e)
        {
            ProgramCounterArrowGroup.RemoveFromGroup(GroupName, this);
        }

        void ProgramCounterArrow_Loaded(object sender, RoutedEventArgs e)
        {
            ProgramCounterArrowGroup.AddToGroup(GroupName, this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (m_Active)
                drawingContext.DrawGeometry(Fill, null, m_Geo);
        }

        public String GroupName
        {
            get { return m_GroupName; }
            set { m_GroupName = value; }
        }

        public Boolean Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        protected override Geometry DefiningGeometry
        {
            get { return m_Geo; }
        }
    }


}
