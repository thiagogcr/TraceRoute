using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RouteEngine;
using System.IO;

namespace Gui
{
    public partial class Form1 : Form
    {
        List<GuiLocation> _guiLocations = new List<GuiLocation>();
        List<Connection> _connections = new List<Connection>();
        GuiLocation _selectedGuiLocation=null;
        
        public Form1()
        {
            InitializeComponent();

            StreamReader sr = new StreamReader("arq.txt");
            var linha = sr.ReadLine();
            while (linha != null)
            {
                var cordenadas = linha.Split(',');
                GuiLocation _guiLocation = new GuiLocation();
                _guiLocation.Identifier = _guiLocations.Count().ToString();
                _guiLocation.X = int.Parse(cordenadas[0]);
                _guiLocation.Y = int.Parse(cordenadas[1]);
                _guiLocations.Add(_guiLocation);
                linha = sr.ReadLine();
            }
            sr.Close();

            PaintGui();
            
            sr = new StreamReader("arq2.txt");
            linha = sr.ReadLine();
            while (linha != null)
            {
                var cordenadas = linha.Split(',');
                Location a = new Location();
                a = getLocaltion(cordenadas[0]);
                Location b = new Location();
                b = getLocaltion(cordenadas[1]);
                Connection connection = new Connection(a, b, 5);
                _connections.Add(connection);
                linha = sr.ReadLine();
            }
            sr.Close();

        }

        private Location getLocaltion(string id) 
        {
            foreach (var item in _guiLocations)
                if (item.Identifier == id)
                    return item;
            return null;
        }

        private void pnlView_MouseDown(object sender, MouseEventArgs e)
        {
            GuiLocation _guiLocation = getGuiLocationAtPoint(e.X, e.Y);
            if (_guiLocation != null)
            {
                if (_selectedGuiLocation != null)
                {
                    int weight = 5;
                    Connection connection = new Connection(_selectedGuiLocation, _guiLocation, weight);
                    
                    _connections.Add(connection);
                    _selectedGuiLocation.Selected = false;

                    _selectedGuiLocation = null;
                }
                else
                {
                    _guiLocation.Selected = true;
                    _selectedGuiLocation = _guiLocation;
                }
            }
            PaintGui();
        }

        GuiLocation getGuiLocationAtPoint(int x, int y)
        {
            foreach (GuiLocation _guiLocation in _guiLocations)
            {
                int x2=x-_guiLocation.X;
                int y2=y-_guiLocation.Y;
                int xToCompare = _guiLocation.Width / 2;
                int yToCompare = _guiLocation.Width / 2;

                if (x2 >= xToCompare * -1 && x2 < xToCompare && y2 > yToCompare * -1 && y2 < yToCompare)
                {
                    return _guiLocation;
                }
            }
            return null;
        }

        private void pnlView_Paint(object sender, PaintEventArgs e)
        {
            PaintGui();
        }

        void PaintGui()
        {
            Brush _brushRed = new  SolidBrush(Color.Orange);
            Brush _brushWhite = new SolidBrush(Color.Black);
            Font _font = new Font(FontFamily.GenericSansSerif, 12);
            Pen _penRed = new Pen(_brushRed);

            foreach (GuiLocation _guiLocation in _guiLocations)
            {
                int _x = _guiLocation.X - _guiLocation.Width / 2;
                int _y = _guiLocation.Y - _guiLocation.Width / 2;
                pnlView.CreateGraphics().DrawString(_guiLocation.Identifier, _font, _brushWhite, _x, _y);
            }

            foreach (Connection _connection in _connections)
            {
                Point point1 = new Point(((GuiLocation)_connection.A).X, ((GuiLocation)_connection.A).Y);
                Point point2 = new Point(((GuiLocation)_connection.B).X, ((GuiLocation)_connection.B).Y); 
                Point Pointref = Point.Subtract(point2, new Size(point1));
                double degrees = Math.Atan2(Pointref.Y, Pointref.X);
                double cosx1 = Math.Cos(degrees);
                double siny1 = Math.Sin(degrees);
                double cosx2 = Math.Cos(degrees + Math.PI);
                double siny2 = Math.Sin(degrees + Math.PI);
                int newx = (int)(cosx1 * (float)((GuiLocation)_connection.A).Width + (float)point1.X);
                int newy = (int)(siny1 * (float)((GuiLocation)_connection.A).Width + (float)point1.Y);
                int newx2 = (int)(cosx2 * (float)((GuiLocation)_connection.B).Width + (float)point2.X);
                int newy2 = (int)(siny2 * (float)((GuiLocation)_connection.B).Width + (float)point2.Y);

                if (_connection.Selected)
                {
                    pnlView.CreateGraphics().DrawLine(_penRed, new Point(newx, newy), new Point(newx2, newy2));
                    pnlView.CreateGraphics().FillEllipse(_brushRed, newx - 4, newy - 4, 8, 8);
                }
            }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            RouteEngine.RouteEngine _routeEngine = new RouteEngine.RouteEngine();
            foreach (Connection connection in _connections)
                _routeEngine.Connections.Add(connection);

            foreach (Location _location in _guiLocations)
                _routeEngine.Locations.Add(_location);
            Dictionary<Location, Route> _shortestPaths = _routeEngine.CalculateMinCost(getLocaltion(textBox1.Text));
            List<Location> _shortestLocations = (List<Location>)(from s in _shortestPaths
                                                                 orderby s.Value.Cost
                                                                 where s.Key.Identifier == textBox2.Text
                                                                 select s.Key).ToList();
            foreach (Connection _connection in _connections)
                _connection.Selected = false;
            foreach (Connection _connection in _shortestPaths[getLocaltion(textBox2.Text)].Connections)
                _connection.Selected = true;
            PaintGui();

        }
    }
}
