using System;
using System.Drawing;

namespace Tetris
{
    public class Block
    {
        private readonly char _letter;
        public readonly bool[,] gridLayout;
        private readonly Color _color;
        private int _xLocation, _yLocation;

        public Block(bool[,] layout, Color c, int xStart, int yStart, char l)
        {
            if (layout.GetLength(0) != layout.GetLength(1))
                throw new System.ArgumentException("Layout must be square.");
            gridLayout = layout;
            _color = c;
            _xLocation = xStart;
            _yLocation = yStart;
            _letter = l;
        }

        public char GetLetter()
        {
        	return _letter;
        }

        public void RotateRight()
        {
            bool[,] temp = (bool[,])gridLayout.Clone();
            Reset(gridLayout);

            for (int x = 0; x < gridLayout.GetLength(0); x += 1)
            {
                for (int y = 0; y < gridLayout.GetLength(1); y += 1)
                    gridLayout[y, gridLayout.GetLength(0) - x - 1] = temp[x, y];
            }
        }
        public void RotateLeft()
        {
            bool[,] temp = (bool[,])gridLayout.Clone();
            Reset(gridLayout);

            for (int x = 0; x < gridLayout.GetLength(0); x += 1)
            {
                for (int y = 0; y < gridLayout.GetLength(1); y += 1)
                    gridLayout[gridLayout.GetLength(1) - y - 1, x] = temp[x, y];
            }
        }

        protected static void Reset(bool[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i += 1)
            {
                for (int j = 0; j < grid.GetLength(1); j += 1)
                {
                    grid[i, j] = false;
                }
            }
        }

        public int LeftmostIngridLayout()
        {
        	int leftmost = gridLayout.GetLength(0);
            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && i < leftmost)
                        leftmost = i;
                }
            }

            return leftmost;
        }
        public int RightmostIngridLayout()
        {
            int rightmost = 0;
            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && i > rightmost)
                        rightmost = i;
                }
            }

            return rightmost;
        }
        public int BottommostIngridLayout()
        {
            int bottommost = 0;
            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && j > bottommost)
                        bottommost = j;
                }
            }

            return bottommost;
        }

        public void MoveLeft(){
        	_xLocation -= 1;
        }
        public void MoveRight(){
        	_xLocation += 1;
        }
        public void MoveDown(){
        	_yLocation += 1;
        }

        public bool CanMoveLeft(Color?[,] grid)
        {
            if (_xLocation + LeftmostIngridLayout() == 0)
                return false;

            for (int i = LeftmostIngridLayout(); i <= RightmostIngridLayout(); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && grid[_xLocation + i - 1, _yLocation + j] != null)
                        return false;
                }
            }

            return true;
        }

        public bool CanMoveRight(Color?[,] grid)
        {
        	if(_xLocation + RightmostIngridLayout() == Grid.WIDTH - 1)
                return false;

        	for (int i = LeftmostIngridLayout(); i <= RightmostIngridLayout(); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && grid[_xLocation + i + 1, _yLocation + j] != null)
                        return false;
                }
            }

            return true;
        }
        public bool CanMoveDown(Color?[,] grid)
        {
        	if (_yLocation + BottommostIngridLayout() == Grid.HEIGHT - 1)
                return false;

            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
            	for (int j = 0; j <= BottommostIngridLayout(); j += 1)
                {
                    if (gridLayout[i, j] && grid[_xLocation + i, _yLocation + j + 1] != null)
                        return false;
                }
            }

            return true;
        }

        public bool CanRotateLeft(Color?[,] grid)
        {
            RotateLeft();
            if(LeftmostIngridLayout() + _xLocation < 0 || RightmostIngridLayout() + _xLocation >= Grid.WIDTH || _yLocation + BottommostIngridLayout() >= Grid.HEIGHT){
            	RotateRight();
            	return false;
            }
            
            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && grid[_xLocation + i, _yLocation + j] != null)
                    {
                        RotateRight();
                        return false;
                    }
                }
            }

            RotateRight();
            return true;
        }
        public bool CanRotateRight(Color?[,] grid)
        {
            RotateRight();
            if(LeftmostIngridLayout() + _xLocation < 0 || RightmostIngridLayout() + _xLocation >= Grid.WIDTH || _yLocation + BottommostIngridLayout() >= Grid.HEIGHT){
            	RotateLeft();
            	return false;
            }
            
            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && grid[_xLocation + i, _yLocation + j] != null)
                    {
                        RotateLeft();
                        return false;
                    }
                }
            }

            RotateLeft();
            return true;
        }

        public static Block FromName(char c)
        {
            switch (c)
            {
                case 'j':
            		return new Block(new bool[,] { { true, false, false }, { true, true, true }, { false, false, false } }, Color.Blue, 3, 0, 'j');
                case 'l':
            		return new Block(new bool[,] { { false, false, true }, { true, true, true }, { false, false, false } }, Color.Orange, 3, 0, 'l');
                case 'o':
                    return new Block(new bool[,] { { true, true }, { true, true } }, Color.Yellow, 4, 0, 'o');
                case 's':
                    return new Block(new bool[,] { { false, true, true }, { true, true, false }, { false, false, false } }, Color.LimeGreen, 3, 0, 's');
                case 'z':
                    return new Block(new bool[,] { { true, true, false }, { false, true, true }, { false, false, false } }, Color.Red, 3, 0, 'z');
                case 't':
                    return new Block(new bool[,] { { false, true, false }, { true, true, true }, { false, false, false } }, Color.MediumPurple, 3, 0, 't');
                case 'i':
                    return new Block(new bool[,] { { false, false, false, false }, { true, true, true, true }, { false, false, false, false }, { false, false, false, false } }, Color.Cyan, 3, 0, 'i');
                default:
                    throw new System.ArgumentException("Invalid block letter representation.");
            }
        }

        public void AddTo(Color?[,] grid)
        {
            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j])
                        grid[_xLocation + i, _yLocation + j] = _color;
                }
            }
        }

        public bool HasThreeCorners(Color?[,] grid)
        {
            int sum = 0;
            if(grid[_xLocation, _yLocation + gridLayout.GetLength(1)] != null)
                sum += 1;
            if (grid[_xLocation + gridLayout.GetLength(0), _yLocation + gridLayout.GetLength(1)] != null)
                sum += 1;
            if (grid[_xLocation, _yLocation] != null)
                sum += 1;
            if (grid[_xLocation + gridLayout.GetLength(0), _yLocation] != null)
                sum += 1;

            return sum >= 3;
        }

        public Block GetGhostBlock(Color?[,] grid)
        {
            Block ghost = new Block(gridLayout, _color, _xLocation, _yLocation, _letter);
            while (ghost.CanMoveDown(grid))
                ghost.MoveDown();

            return ghost;
        }

        public bool Intersects(Color?[,] grid)
        {
            for (int i = 0; i < gridLayout.GetLength(0); i += 1)
            {
                for (int j = 0; j < gridLayout.GetLength(1); j += 1)
                {
                    if (gridLayout[i, j] && grid[_xLocation + i, _yLocation + j] != null)
                        return true;
                }
            }
			
            return false;
        }

        public Color GetColor(){
        	return _color;
        }

        public Tuple<int, int>[] GetBlockLocations()
        {
            Tuple<int, int>[] output = new Tuple<int, int>[4];
            int pos = 0;
            for(int x = 0; x < gridLayout.GetLength(0); x += 1)
            {
                for (int y = 0; y < gridLayout.GetLength(1); y += 1)
                {
                    if (gridLayout[x, y]) {
                        output[pos] = new Tuple<int, int>(x + _xLocation, y + _yLocation);
                        pos += 1;
                    }
                }
            }

            return output;
        }
    }
}