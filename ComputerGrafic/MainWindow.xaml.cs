using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using static ComputerGrafic.MainWindow;

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
        Point PointBuf = new Point();
        GroupPoint GroupBuf = new GroupPoint();
        int countGroups = 0;
        int currentGroup = 0;
        int currentPimitive = -1;
        OpenGL gl;

        public class ColorBox
        {
            public string col { get; set; } = "";
            public int idCol { get; set; }
            public override string ToString() => $"{col}";
        }

        public MainWindow()
        {
            InitializeComponent();
            GroupsPoints.Add(new GroupPoint(greenInt));

            colorComboBox.ItemsSource = new ColorBox[]
        {
            new ColorBox { col = "Зеленый", idCol = 0},
            new ColorBox { col = "Синий", idCol = 1},
            new ColorBox { col = "Желтый", idCol = 2},
            new ColorBox { col = "Красный", idCol = 3}
        };
            colorComboBox.SelectedIndex = 0;
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
                if (i == currentGroup) continue;
                gl.Color(color[GroupsPoints[i].colorGroup][0], color[GroupsPoints[i].colorGroup][1], color[GroupsPoints[i].colorGroup][2]);


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

            if (currentPimitive >= 0 && currentPimitive < GroupsPoints[currentGroup].points.Count())
            {
                gl.PointSize(5.0f);
                gl.Begin(BeginMode.Points);
                gl.Color(1.0f, 1.0f, 1.0f);
                gl.Vertex(GroupsPoints[currentGroup].points[currentPimitive].x, GroupsPoints[currentGroup].points[currentPimitive].y);

                gl.End();
            }
            gl.Flush();
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupBuf.points = GroupsPoints[currentGroup].points;
            if (colorComboBox.SelectedItem is ColorBox color_)
                GroupBuf.colorGroup = color_.idCol;
            GroupsPoints[currentGroup] = GroupBuf;
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
            currentGroup = countGroups;
            currentPimitive++;
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
                GroupAdd();
            }
            else if (e.Key == Key.Right)
            {
                GroupRight();
            }
            else if (e.Key == Key.Left)
            {
                GroupLeft();
            }
            else if (e.Key == Key.Up)
            {
                GroupUp();
            }
            else if (e.Key == Key.Down)
            {
                GroupDown();
            }
            else if (e.Key == Key.C)
            {
                GroupBuf.points = GroupsPoints[currentGroup].points;

                if (GroupsPoints[currentGroup].colorGroup < color.Length - 1)
                    GroupBuf.colorGroup = GroupsPoints[currentGroup].colorGroup + 1;
                else GroupBuf.colorGroup = 0;
                GroupsPoints[currentGroup] = GroupBuf;
                colorComboBox.SelectedIndex = GroupsPoints[currentGroup].colorGroup;
            }
            else if (e.Key == Key.Q)
            {
                GroupDelete();
            }
            else if (e.Key == Key.W)
            {
                PrimitiveDelete();
            }
            else if (e.Key == Key.A)
            {
                GroupBack();
            }
            else if (e.Key == Key.S)
            {
                GroupNext();
            }
            else if (e.Key == Key.D)
            {
                PrimitiveBack();
            }
            else if (e.Key == Key.F)
            {
                PrimitiveNext();
            }
        }

        public void GroupAdd()
        {
            if (GroupsPoints[countGroups].points.Count != 0)
            {
                countGroups += 1;
                currentGroup = countGroups;
                GroupsPoints.Add(new GroupPoint(greenInt));
                colorComboBox.SelectedIndex = 0;
                currentPimitive = -1;
            }
        }
        public void GroupRight()
        {
            for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
            {
                PointBuf.x = GroupsPoints[currentGroup].points[i].x + 0.01f;
                PointBuf.y = GroupsPoints[currentGroup].points[i].y;
                GroupsPoints[currentGroup].points[i] = PointBuf;
            }
        }
        public void GroupLeft()
        {
            for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
            {
                PointBuf.x = GroupsPoints[currentGroup].points[i].x - 0.01f;
                PointBuf.y = GroupsPoints[currentGroup].points[i].y;
                GroupsPoints[currentGroup].points[i] = PointBuf;
            }
        }
        public void GroupUp()
        {
            for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
            {
                PointBuf.x = GroupsPoints[currentGroup].points[i].x;
                PointBuf.y = GroupsPoints[currentGroup].points[i].y + 0.01f;
                GroupsPoints[currentGroup].points[i] = PointBuf;
            }
        }
        public void GroupDown()
        {
            for (int i = 0; i < GroupsPoints[currentGroup].points.Count; i++)
            {
                PointBuf.x = GroupsPoints[currentGroup].points[i].x;
                PointBuf.y = GroupsPoints[currentGroup].points[i].y - 0.01f;
                GroupsPoints[currentGroup].points[i] = PointBuf;
            }
        }
        public void GroupDelete()
        {
            if (countGroups > 0)
            {
                GroupsPoints.Remove(GroupsPoints[currentGroup]);
                if (currentGroup > 0)
                {
                    countGroups--;
                    currentGroup--;
                }
                else
                {
                    countGroups--;
                }
                if (countGroups == 0)
                    currentPimitive = -1;
                else
                    currentPimitive = 0;
            }
        }
        public void PrimitiveDelete()
        {
            if (currentPimitive >= 0 && GroupsPoints[currentGroup].points.Count > 0)
            {
                GroupsPoints[currentGroup].points.Remove(GroupsPoints[currentGroup].points[currentPimitive]);
                if (currentPimitive > 0)
                    currentPimitive--;
            }
        }

        public void PrimitiveBack()
        {
            if (currentPimitive > 0)
                currentPimitive--;
        }
        public void PrimitiveNext()
        {
            if (currentPimitive < GroupsPoints[currentGroup].points.Count() - 1)
                currentPimitive++;
        }

        public void GroupBack()
        {
            if (GroupsPoints[countGroups].points.Count != 0)
            {
                countGroups++;
                GroupsPoints.Add(new GroupPoint(greenInt));
                currentPimitive = 0;
            }
            if (currentGroup > 0)
            {
                currentGroup--;
                colorComboBox.SelectedIndex = GroupsPoints[currentGroup].colorGroup;
                currentPimitive = 0;
            }
        }
        public void GroupNext()
        {
            if (currentGroup < countGroups - 1)
            {
                currentGroup++;
                colorComboBox.SelectedIndex = GroupsPoints[currentGroup].colorGroup;
                currentPimitive = 0;
            }
        }

        private void GroupBackButton_Click(object sender, RoutedEventArgs e)
        {
            GroupBack();
        }

        private void GroupNextButton_Click(object sender, RoutedEventArgs e)
        {
            GroupNext();
        }

        private void GroupDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrimitiveDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroupDownButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroupRightButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroupLeftButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroupUpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroupCreateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
