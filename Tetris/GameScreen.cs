using System;
using System.Drawing;
using System.Windows.Forms;
namespace Tetris {
    public class GameScreen : Form
    {
        public static readonly int BLOCK_SIZE = 24;
        public static readonly int GRID_SPACING = 1;
        public static readonly int GRID_BORDER_THICKNESS = 6;
        public static readonly int GHOST_TRANSPARENCY = 100;//Out of 255

        private Grid _gameData;
        private Clock _timer;
        
        private bool _hideText;

        public GameScreen()
        {
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            
            MaximizeBox = false;
            ClientSize = new Size((Grid.WIDTH + 10) * BLOCK_SIZE + GRID_SPACING * (Grid.WIDTH - 1) + 2 * GRID_BORDER_THICKNESS, (Grid.HEIGHT + 2) * BLOCK_SIZE + GRID_SPACING * (Grid.HEIGHT - 3) + 2 * GRID_BORDER_THICKNESS);
            Location = new Point(0, 0);        
            
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;
            
            BackColor = Color.LightSlateGray;
            Icon = Properties.Resources.T;
            Text = "Tetris";
            InitializeGameScreen();
        }

        public void InitializeGameScreen()
        {
            _gameData = new Grid();
            _hideText = false;
            Invalidate();
            _timer = new Clock(1, ElapsedEvent_Update);
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new GameScreen());
        }

        public void QuickDrop()
        {
            _gameData.QuickDrop();
            
            if(_gameData.IsGameOver())
            	EndGame();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
        	if(_hideText)
            	return base.ProcessCmdKey(ref msg, keyData);
        	
            if (keyData == Keys.Down || keyData == Keys.S)
                QuickDrop();
            else if (keyData == Keys.A || keyData == Keys.Left)
                _gameData.MoveLeft();
            else if (keyData == Keys.D || keyData == Keys.Right)
                _gameData.MoveRight();
            else if (keyData == Keys.Q || keyData == Keys.PageUp)
                _gameData.RotateRight();
            else if (keyData == Keys.E || keyData == Keys.PageDown)
                _gameData.RotateLeft();
            else if (keyData == Keys.F || keyData == Keys.RControlKey)
                _gameData.HoldBlock();

            CheckForLevelUp();
            InvalidateSectors();
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        public void CheckForLevelUp()
        {
            if(_gameData.GetRows() > _gameData.GetLevel() * 5)
            {
            	_gameData.IncrementLevel();
                _gameData.ClearRows();
                if (_gameData.GetLevel() < 20)
                    _timer.UpdateInterval(_gameData.GetLevel());
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Brush brush = new SolidBrush(ControlPaint.Dark(Color.DarkGray));
            e.Graphics.FillRectangle(brush, new Rectangle(2 * BLOCK_SIZE, 2 * BLOCK_SIZE, BLOCK_SIZE * Grid.WIDTH + GRID_SPACING * (Grid.WIDTH - 1) + 2 * GRID_BORDER_THICKNESS, BLOCK_SIZE * (Grid.HEIGHT - 2) + GRID_SPACING * (Grid.HEIGHT - 3) + 2 * GRID_BORDER_THICKNESS));
            e.Graphics.FillRectangle(brush, new Rectangle(2 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 4 * BLOCK_SIZE + GRID_BORDER_THICKNESS + 2 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS, 3 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS));
            e.Graphics.FillRectangle(brush, new Rectangle(2 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 10 * BLOCK_SIZE + GRID_BORDER_THICKNESS + 7 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS, 3 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS));

            brush = new SolidBrush(ControlPaint.Dark(Color.DarkGray, 50));
            e.Graphics.FillRectangle(brush, new Rectangle(3 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 4 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS + 2 * GRID_SPACING, 3 * BLOCK_SIZE, 3 * BLOCK_SIZE));
            e.Graphics.FillRectangle(brush, new Rectangle(3 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 10 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS + 7 * GRID_SPACING, 3 * BLOCK_SIZE, 3 * BLOCK_SIZE));

            
        	brush = new SolidBrush(Color.Black);
        	e.Graphics.FillRectangle(brush, new Rectangle(2 * BLOCK_SIZE + GRID_BORDER_THICKNESS, 2 * BLOCK_SIZE + GRID_BORDER_THICKNESS, Grid.WIDTH * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING - 2, (Grid.HEIGHT - 2) * BLOCK_SIZE + (Grid.HEIGHT - 3) * GRID_SPACING));
        	if(!_hideText){
            	e.Graphics.DrawString("Next:", new Font("Times New Roman", 16, FontStyle.Bold, GraphicsUnit.Pixel), brush, 2 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 3 * BLOCK_SIZE + GRID_BORDER_THICKNESS + 2 * GRID_SPACING);
            	e.Graphics.DrawString("Saved:", new Font("Times New Roman", 16, FontStyle.Bold, GraphicsUnit.Pixel), brush, 2 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 9 * BLOCK_SIZE + GRID_BORDER_THICKNESS + 7 * GRID_SPACING);
            	e.Graphics.DrawString("Level: " + _gameData.GetLevel(), new Font("Times New Roman", 16, FontStyle.Bold, GraphicsUnit.Pixel), brush, 2 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 14 * BLOCK_SIZE + GRID_BORDER_THICKNESS + 7 * GRID_SPACING);
            }
            
            brush = new SolidBrush(ControlPaint.Dark(Color.DarkGray));
            for (int rows = 0; rows < Grid.WIDTH - 1; rows += 1)
                e.Graphics.FillRectangle(brush, new Rectangle(3 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * rows, 2 * BLOCK_SIZE + GRID_BORDER_THICKNESS, GRID_SPACING, (Grid.HEIGHT - 2) * (BLOCK_SIZE + GRID_SPACING) - GRID_SPACING));
            for (int cols = 0; cols < Grid.HEIGHT - 3; cols += 1)
                e.Graphics.FillRectangle(brush, new Rectangle(2 * BLOCK_SIZE + GRID_BORDER_THICKNESS, 3 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * cols, Grid.WIDTH * (BLOCK_SIZE + GRID_SPACING) - GRID_SPACING, GRID_SPACING));
            
            DrawSpecialBlock(e, new Rectangle(3 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 4 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS + 2 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING), _gameData.GetNext());
            DrawSpecialBlock(e, new Rectangle(3 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 10 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS + 7 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING), _gameData.GetSaved());

            for (int i = 0; i < Grid.WIDTH; i += 1) {
                for (int j = 2; j < Grid.HEIGHT; j += 1) {
                    if (_gameData.GetTile(i, j) != null) {
            			brush = new SolidBrush((Color)_gameData.GetTile(i, j));
                        e.Graphics.FillRectangle(brush, new Rectangle(2 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * i, 2 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * (j - 2), BLOCK_SIZE, BLOCK_SIZE));
                    }
                }
            }

            brush = new SolidBrush(_gameData.GetCurrent().GetColor());
            for (int i = 0; i < _gameData.GetCurrent().GetBlockLocations().GetLength(0); i += 1) {
                if (_gameData.GetCurrent().GetBlockLocations()[i].Item2 >= 2)
                    e.Graphics.FillRectangle(brush, new Rectangle(2 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * _gameData.GetCurrent().GetBlockLocations()[i].Item1, 2 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * (_gameData.GetCurrent().GetBlockLocations()[i].Item2 - 2), BLOCK_SIZE, BLOCK_SIZE));
            }

            brush = new SolidBrush(Color.FromArgb(GHOST_TRANSPARENCY, _gameData.GetGhost().GetColor()));
            for (int i = 0; i < _gameData.GetGhost().GetBlockLocations().GetLength(0); i += 1) {
                if(_gameData.GetGhost().GetBlockLocations()[i].Item2 >= 2)
                    e.Graphics.FillRectangle(brush, new Rectangle(2 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * _gameData.GetGhost().GetBlockLocations()[i].Item1, 2 * BLOCK_SIZE + GRID_BORDER_THICKNESS + (BLOCK_SIZE + GRID_SPACING) * (_gameData.GetGhost().GetBlockLocations()[i].Item2 - 2), BLOCK_SIZE, BLOCK_SIZE));
            }

            brush.Dispose();
        }
        
        public void InvalidateSectors()
        {
            Invalidate(new Rectangle(3 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 4 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS + 2 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING));
            Invalidate(new Rectangle(3 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 10 * BLOCK_SIZE + 2 * GRID_BORDER_THICKNESS + 7 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING, 3 * BLOCK_SIZE + 2 * GRID_SPACING));
            Invalidate(new Rectangle(2 * BLOCK_SIZE + GRID_BORDER_THICKNESS, 2 * BLOCK_SIZE + GRID_BORDER_THICKNESS, Grid.WIDTH * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING - 2, (Grid.HEIGHT - 2) * BLOCK_SIZE + (Grid.HEIGHT - 3) * GRID_SPACING));
            Invalidate(new Rectangle(2 * GRID_BORDER_THICKNESS + (Grid.WIDTH + 4) * BLOCK_SIZE + (Grid.WIDTH - 1) * GRID_SPACING, 14 * BLOCK_SIZE + GRID_BORDER_THICKNESS + 7 * GRID_SPACING, BLOCK_SIZE * 4, BLOCK_SIZE * 2));
        }

        public void DrawSpecialBlock(PaintEventArgs e, Rectangle boundaries, Block type){
        	if(type != null){
        		Brush color = new SolidBrush(type.GetColor());
        		switch(type.GetLetter()){
        			case 't':
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + 2 * BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				break;
        			case 'j':
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + 2 * BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				break;
        			case 'l':
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + 2 * BLOCK_SIZE, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + 2 * BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				break;
        			case 'o':
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE / 2, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE * 2, BLOCK_SIZE * 2));
        				break;
        			case 's':
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + 2 * BLOCK_SIZE, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				break;
        			case 'z':
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left + 2 * BLOCK_SIZE, boundaries.Top + 3 * BLOCK_SIZE / 2, BLOCK_SIZE, BLOCK_SIZE));
        				break;
        			case 'i':
        				e.Graphics.FillRectangle(color, new Rectangle(boundaries.Left, boundaries.Top + BLOCK_SIZE * 9 / 8, BLOCK_SIZE * 3, BLOCK_SIZE * 3 / 4));
        				break;
        		}
                color.Dispose();
        	}
        }

        public void EndGame()
        {
            _timer.End();
            BackColor = ControlPaint.Dark(Color.DarkRed);
            _hideText = true;
            Invalidate();
        }
        
        public void ElapsedEvent_Update(object state)
        {
        	_gameData.MoveDown();
        	
        	if (_gameData.IsGameOver())
                EndGame();
        	
            InvalidateSectors();
        }
    }
}