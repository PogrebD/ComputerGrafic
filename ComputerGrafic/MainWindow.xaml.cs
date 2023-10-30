using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.WPF;

namespace ComputerGrafic
{
    public partial class MainWindow : Window
    {
        public static float[] redColor = { 1.0f, 0.0f, 0.0f };
        public static float[] greenColor = { 0.0f, 1.0f, 0.0f };
        public static int greenInt = 0;
        public static float[] blueColor = { 0.0f, 0.0f, 1.0f };
        public static float[] yellowColor = { 1.0f, 1.0f, 0.0f };
        public static float[][] color = { greenColor, blueColor, yellowColor, redColor };
        public static float[] defColor = { 0.0f, 1.0f, 0.0f };
        public struct Point
        {
            public float x, y;
            public Point(float x_, float y_)
            {
                x = x_;
                y = y_;
            }
        };

        public struct GroupPoint
        {
            public List<Point> points;
            public int colorGroup;
            public GroupPoint(int colorGroup_)
            {
                colorGroup = colorGroup_;
                points = new List<Point>();
            }
        };

        public List<GroupPoint> GroupsPoints = new List<GroupPoint>();
        int countGroups = 0;
        int currentGroup = 0;
        OpenGL gl;
        public MainWindow()
        {
            InitializeComponent();
            GroupsPoints.Add(new GroupPoint(greenInt));
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            gl = args.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();

            gl.PointSize(3.0f);

            gl.Begin(BeginMode.Points);

            float[] currColor = { 1.0f, 0.0f, 0.0f };
            for (int i = 0; i < GroupsPoints.Count(); i++)
            {
                gl.Color(color[GroupsPoints[i].colorGroup][0], color[GroupsPoints[i].colorGroup][1], color[GroupsPoints[i].colorGroup][2]);
                if (i == currentGroup) continue;

                foreach (Point p in GroupsPoints[i].points)
                {
                    gl.Vertex(p.x, p.y);
                }
            }

            gl.End();
            gl.PointSize(5.0f);
            gl.Begin(BeginMode.Points);
            gl.Color(color[GroupsPoints[currentGroup].colorGroup][0], color[GroupsPoints[currentGroup].colorGroup][1], color[GroupsPoints[currentGroup].colorGroup][2]);
            foreach (Point p in GroupsPoints[currentGroup].points)
            {
                gl.Vertex(p.x, p.y);
            }
            gl.End();
            gl.Flush();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            float x;
            float y;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                x = (float)(e.GetPosition(OpenglControl_).X);
                y = (float)(e.GetPosition(OpenglControl_).Y);

                x = (2.0f * x / (float)OpenglControl_.ActualWidth) - 1.0f;
                y = (0.5f - (y / (float)OpenglControl_.ActualHeight)) * 2;

                Point p = new Point(x, y);

                GroupsPoints[countGroups].points.Add(p);
            }
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
        {
            args.OpenGL.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                countGroups += 1;
                currentGroup = countGroups;
                GroupsPoints.Add(new GroupPoint(greenInt));
            }
            else if (e.Key == Key.Right)
            {
                for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
                {
                    Point p = new Point(GroupsPoints[currentGroup].points[i].x + 0.01f, GroupsPoints[currentGroup].points[i].y);
                    GroupsPoints[currentGroup].points[i] = p;
                }
            }
            else if (e.Key == Key.Left)
            {
                for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
                {
                    Point p = new Point(GroupsPoints[currentGroup].points[i].x - 0.01f, GroupsPoints[currentGroup].points[i].y);
                    GroupsPoints[currentGroup].points[i] = p;
                }
            }
            else if (e.Key == Key.Up)
            {
                for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
                {
                    Point p = new Point(GroupsPoints[currentGroup].points[i].x, GroupsPoints[currentGroup].points[i].y + 0.01f);
                    GroupsPoints[currentGroup].points[i] = p;
                }
            }
            else if (e.Key == Key.Down)
            {
                for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
                {
                    Point p = new Point(GroupsPoints[currentGroup].points[i].x, GroupsPoints[currentGroup].points[i].y - 0.01f);
                    GroupsPoints[currentGroup].points[i] = p;
                }
            }
            else if (e.Key == Key.C)
            {
                GroupPoint group = new GroupPoint();
                group.points = GroupsPoints[currentGroup].points;

                if (GroupsPoints[currentGroup].colorGroup < color.Length - 1)
                    group.colorGroup = GroupsPoints[currentGroup].colorGroup + 1;
                else group.colorGroup = 0;
                GroupsPoints[currentGroup] = group;
            }
            else if (e.Key == Key.Q)
            {
                if (countGroups > 0)
                {
                    GroupsPoints.Remove(GroupsPoints[currentGroup - 1]);
                    countGroups--;
                    currentGroup--;
                }
            }
            else if (e.Key == Key.W)
            {
                GroupsPoints[currentGroup].points.Remove(GroupsPoints[currentGroup].points[GroupsPoints[currentGroup].points.Count - 1]);
            }
            else if (e.Key == Key.A)
            {
                if (currentGroup > 0)
                {
                    currentGroup--;
                }
            }
            else if (e.Key == Key.D)
            {
                if (currentGroup < countGroups)
                {
                    currentGroup++;
                }
            }
        }
    }
}
