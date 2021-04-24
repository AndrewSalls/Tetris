using System;
using System.Drawing;

namespace Tetris
{
    public class Grid
    {
        public static readonly int WIDTH = 10;
        public static readonly int HEIGHT = 22;
        public static readonly int MAX_MOVES = 15;

        private readonly char[] _blockOrder;
        private int _remainingInBlockOrder;

        private int _rowsCleared;
        private int _level;
        private bool _lastWasRotate;
        
        private readonly Color?[,] _grid;
        private int _moveCount;
        private bool _lockDelay, _swapped, _lastWasMoveDown;
        private Block _current, _saved, _next;

        public Grid()
        {
            _lastWasRotate = false;
            _rowsCleared = 0;
            _level = 1;
            _grid = new Color?[WIDTH, HEIGHT];
            _blockOrder = new char[] { 'j', 'l', 's', 'z', 'i', 'o', 't' };
            ResetRandomOrder();
            _current = RandomBlock();
            _next = RandomBlock();
            _saved = null;
        }

        public void ResetRandomOrder()
        {
            _remainingInBlockOrder = _blockOrder.Length;
            Random randomizer = new Random();
            for (int i = 0; i < _blockOrder.Length; i++)
            {
                char temp = _blockOrder[i];
                int rand = randomizer.Next(7);
                _blockOrder[i] = _blockOrder[rand];
                _blockOrder[rand] = temp;
            }
        }

        public void QuickDrop()
        {
            while (_current.CanMoveDown(_grid))
                _current.MoveDown();

            _lastWasRotate = false;
            ResetBlockData();
        }

        public Block RandomBlock()
        {
            if (_remainingInBlockOrder == 0)
                ResetRandomOrder();

            _remainingInBlockOrder -= 1;
            Block output = Block.FromName(_blockOrder[_remainingInBlockOrder]);
            output.RotateLeft();
            return output;
        }

        public void RotateLeft()
        {
            if (_current.CanRotateLeft(_grid)) {
                _current.RotateLeft();
            	_lastWasRotate = true;
            	_lastWasMoveDown = false;
                
                if(_lockDelay)
                	_moveCount += 1;
            }
        }
        public void RotateRight()
        {
            if (_current.CanRotateRight(_grid)) {
                _current.RotateRight();
            	_lastWasRotate = true;
            	_lastWasMoveDown = false;
            
            if(_lockDelay)
                	_moveCount += 1;
            }
        }
        public void MoveLeft()
        {
            if (_current.CanMoveLeft(_grid)) {
                _current.MoveLeft();
            	_lastWasRotate = false;
            	_lastWasMoveDown = false;
                
                if(_lockDelay)
                	_moveCount += 1;
            }
        }
        public void MoveRight()
        {
            if (_current.CanMoveRight(_grid)) {
                _current.MoveRight();
            	_lastWasRotate = false;
           		_lastWasMoveDown = false;
                
                if(_lockDelay)
                	_moveCount += 1;
            }
        }
        public void MoveDown()
        {
            if (_current.CanMoveDown(_grid)) {
                _current.MoveDown();
                _lastWasRotate = false;
                _lastWasMoveDown = true;
                _moveCount = 0;
            }
        	else if (_lastWasMoveDown || (_moveCount > MAX_MOVES && _lockDelay))
                ResetBlockData();
        	else{
            	_lockDelay = true;
            	_lastWasMoveDown = true;
        	}
        }

        public void ResetBlockData()
        {
            _lockDelay = false;
            _swapped = false;
            _lastWasMoveDown = false;
            _lastWasRotate = false;
            _moveCount = 0;

            if (ValidTSpin())
                AddRows(4);

            _current.AddTo(_grid);
            _current = _next;

            _next = RandomBlock();
            ClearRows();
        }

        public void ClearRows()
        {
            short numberCleared = 0;
            for (int row = 0; row < HEIGHT; row += 1)
            {
                int sum = 0;
                for (int col = 0; col < WIDTH; col += 1)
                {
                    if (_grid[col, row] != null)
                        sum += 1;
                }

                if (sum == WIDTH) {
                    numberCleared += 1;
                    MoveRowsDown(row);
                }
            }

            switch (numberCleared)
            {
                case 4:
                    AddRows(8);
                    break;
                case 3:
                    AddRows(5);
                    break;
                case 2:
                    AddRows(3);
                    break;
                case 1:
                    AddRows(1);
                    break;
            }
        }

        private void MoveRowsDown(int row)
        {
            for (int copy = row; copy > 0; copy -= 1)
            {
                for (int pos = 0; pos < WIDTH; pos += 1)
                {
                    _grid[pos, copy] = _grid[pos, copy - 1];
                }
            }
        }

        public void HoldBlock()
        {
            if (!_swapped)
            {
            	if(_saved == null){
            		_saved = Block.FromName(_current.GetLetter());
            		_saved.RotateLeft();
            		_current = RandomBlock();
            	}
    			else{
                	Block temp = Block.FromName(_current.GetLetter());
                	temp.RotateLeft();
                	_current = _saved;
                	_saved = temp;
            	}
            	
            	_swapped = true;
            }
        }

        private void AddRows(int value){
        	_rowsCleared += value;
        }

        public int GetRows(){
        	return _rowsCleared;
        }

        public int GetLevel(){
        	return _level;
        }

        public void IncrementLevel(){
        	_rowsCleared -= 5 * _level;
        	_level += 1;
        }
        
        public void SetLevel(int value){
        	_level = value;
        }

        public bool ValidTSpin(){
        	return _current.GetLetter() == 't' && _lastWasRotate && _current.HasThreeCorners(_grid);
        }

        public bool IsGameOver(){
        	return _current.Intersects(_grid);
        }
        
        public bool CanMoveDown(){
        	return _current.CanMoveDown(_grid);
        }
        
        public Color? GetTile(int x, int y){
        	return _grid[x,y];
        }

        public Block GetSaved(){
        	return _saved;
        }

        public Block GetNext(){
        	return _next;
        }
        
        public Block GetCurrent(){
        	return _current;
        }

        public Block GetGhost(){
        	return _current.GetGhostBlock(_grid);
        }
    }
}