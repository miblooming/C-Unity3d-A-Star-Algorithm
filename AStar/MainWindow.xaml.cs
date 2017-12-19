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

namespace AStar
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] map = {
            "**************",
            "*##          *",
            "*## ### ## # *",
            "*## ### ## # *",
            "*    ##S   # *",
            "* ##    ## # *",
            "* ## ##### # *",
            "* ## ##### # *",
            "*           T*",
            "**************",
         };

        List<int> list = new List<int>();
        int start = 0;
        int target = 0;
        int w = 0;
        int h = 0;
        const int CELL_W = 20;
        const int CELL_H = 20;
        const int MAX_W = 500 / 20;
        const int MAX_H = 500 / 20;

        public MainWindow()
        {
            InitializeComponent();
            render();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            map = randMap();
            render();

        }

        private void render()
        {
            render(-1, -1);
        }

        private void render(int pstart, int ptarget)
        {
            ResetCanvas();
            list = new List<int>();
            h = map.Length;

            foreach (string line in map)
            {
                w = line.Length;
                for (int i = 0; i < w; i++)
                {
                    char c = line.ElementAt(i);
                    if (c == '*' || c == '#')
                    {
                        list.Add(1);
                    }
                    else
                    {
                        list.Add(0);
                        if (c == 'S')
                        {
                            start = list.Count - 1;
                        }
                        else if (c == 'T')
                        {
                            target = list.Count - 1;
                        }
                    }
                }
            }

            if (pstart != -1)
            {
                start = pstart;
            }
            if (ptarget != -1)
            {
                target = ptarget;
            }

            DrawMap();
            DrawObj();

            DrawPath(AStar4Direction.Search(list.ToArray(), w, h, start, target));
        }

        int change_type = 0;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            lbTips.Content = "点击地图空格，修改起点";
            change_type = 0;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            lbTips.Content = "点击地图空格，修改终点";
            change_type = 1;
        }

        private string[] randMap()
        {
            Random r = new Random();
            w = 10 + r.Next(MAX_W - 10);
            h = 10 + r.Next(MAX_H - 10);
            string[] map = new string[h];
            start = r.Next(w*h);
            target = r.Next(w * h);
            for (int j = 0; j < h; j++)
            {
                string line = "";
                for(int i = 0; i < w; i++)
                {
                    if (j * h + i == start)
                    {
                        line += "S";
                    }else if (j * h + i == target)
                    {
                        line += "T";
                    }
                    else
                    {
                        int n = r.Next(3);
                        if (n > 0)
                        {
                            line += " ";
                        }
                        else
                        {
                            line += "*";
                        }
                    }
                }
                map[j] = line;
            }
            return map;
        }


        private void DrawMap()
        {
            for(int i=0;i< list.Count; i++)
            {
                int p = list[i];
                int x = i % w;
                int y = i / w;
                int border = (p == 1) ? 1 : 0;
                Color c = (p == 1)?Colors.DarkSlateGray: Colors.Gray;

                UIElement e = DrawRectangle(c, x*CELL_W, y*CELL_H, CELL_W, CELL_H, border,Colors.Honeydew);
                my_canvas_dic.Add(e, i);
            }
        }

        private void DrawObj()
        {
            int cw = 10;
            int ch = 10;

            int p = list[start];
            int x = start % w;
            int y = start / w;
            Color c = Colors.Red;
            DrawRectangle(c, x * CELL_W + cw / 2, y * CELL_H + ch / 2, cw, ch, 0, Colors.Yellow);

            p = list[target];
            x = target % w;
            y = target / w;

            c = Colors.White;
            DrawRectangle(c, x * CELL_W + cw / 2, y * CELL_H + ch / 2, cw, ch, 1, Colors.Red);
        }

        private void DrawPath(int[] path)
        {
            if (path != null)
            {
                for (int i = 1; i < path.Length; i++)
                {
                    int from = path[i - 1];
                    int to = path[i];
                    int x1 = (from % w) * CELL_W + CELL_W / 2;
                    int y1 = (from / w) * CELL_H + CELL_H / 2;
                    int x2 = (to % w) * CELL_W + CELL_W / 2;
                    int y2 = (to / w) * CELL_H + CELL_H / 2;
                    DrawLine(x1, y1, x2, y2, Colors.Black, 3);
                }
            }
        }

        private void DrawLine(int x1, int y1, int x2, int y2,Color c,int board)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(c);

            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;

            line.StrokeThickness = 2;
            my_canvas.Children.Add(line);
        }

        Dictionary<UIElement, int> my_canvas_dic = new Dictionary<UIElement, int>();
        private UIElement DrawRectangle(Color c,int x,int y,int w,int h,int border,Color bc)
        {
            Rectangle rect = new Rectangle();
            rect.Width = w;
            rect.Height = h;
            // Create a SolidColorBrush and use it to
            // paint the rectangle.
            SolidColorBrush myBrush = new SolidColorBrush(c);
            rect.Stroke = new SolidColorBrush(bc);
            rect.StrokeThickness = border;
            rect.Fill = myBrush;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            my_canvas.Children.Add(rect);
            return rect;
        }

        private void ResetCanvas()
        {
            my_canvas.Children.Clear();
            my_canvas_dic.Clear();
        }

        private void my_canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //选中控件
            UIElement targetElement = Mouse.DirectlyOver as UIElement;
            if (targetElement != null && my_canvas_dic.ContainsKey(targetElement))
            {
                int pos = my_canvas_dic[targetElement];
                if (change_type == 0)
                {
                    start = pos;
                }
                else
                {
                    target = pos;
                }
                render(start, target);


            }
        }
    }
}
