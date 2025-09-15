using System.Collections.Generic;
using System.Collections;
using Random = System.Random;
using UnityEngine;

namespace Sigmoid.Puzzles
{
    /// <summary>
    /// Allows you to play a game of tic-tac-toe against a semi-smart robot
    /// </summary>
	public class TicTacToe : Puzzle
	{
        [SerializeField] private XOCell[] cells;
        public bool CanPlay { get; private set; }

        private Random rng;
		private CellState[,] board;
        public override void Initialise(int seed)
        {
            rng = new Random(seed);
            board = new CellState[3, 3];
            CanPlay = true;
        }

        /// <summary>
        /// Places either an X or an O at the given (x, y) coordinate<br/>
        /// (0, 0) is the top left and (2, 2) is the bottom right
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isX"></param>
        public bool Place(Vector2Int coord, bool isX)
        {
            if(coord.x < 0 || coord.x > 2 || coord.y < 0 || coord.y > 2
            || board[coord.x, coord.y] != CellState.Empty) return false;

            board[coord.x, coord.y] = isX ? CellState.X : CellState.O;
            cells[coord.y * 3 + coord.x].Place(isX);

            if(IsWinner(isX ? CellState.X : CellState.O, out int[] winningIndices))
            {
                if(isX) Fail();
                else Succeed();

                for(int i = 0; i < winningIndices.Length; i++)
                    cells[winningIndices[i]].Glow(isX);

                CanPlay = false;
                return true;
            }
            return false;
        }

        public void PlayRobot() => StartCoroutine(PlayRobotAsync());
        private IEnumerator PlayRobotAsync()
        {
            CanPlay = false;
            float thinkTime = 0.1f * rng.Next(10, 15);
            yield return new WaitForSeconds(thinkTime);

            bool foundMove = ChooseRobotMove(out Vector2Int move);
            if(!foundMove) yield break;

            bool robotWins = Place(move, true);
            if(!robotWins) CanPlay = true;
        }

        /// <summary>
        /// Attempts to win or block the player, or chooses a random move if neither is possible
        /// </summary>
        /// <returns></returns>
        private bool ChooseRobotMove(out Vector2Int move)
        {
            if(TryFindWinningMove(CellState.X, out move)  //try to win first
            || TryFindWinningMove(CellState.O, out move)) //then try blocking the player
                return true;

            List<Vector2Int> emptyCells = new List<Vector2Int>();
            for(int x = 0; x < 3; x++)
                for(int y = 0; y < 3; y++)
                    if(board[x, y] == CellState.Empty)
                        emptyCells.Add(new Vector2Int(x, y));

            //this means a draw was reached
            if(emptyCells.Count == 0)
            {
                Fail();
                CanPlay = false;
                return false;
            }

            int randomIndex = rng.Next(emptyCells.Count);
            move = emptyCells[randomIndex];
            return true;
        }

        /// <summary>
        /// Checks all squares and tests if placing something there results in a win
        /// </summary>
        /// <param name="player"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        private bool TryFindWinningMove(CellState player, out Vector2Int move)
        {
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 3; y++)
                {
                    if(board[x, y] != CellState.Empty) continue;

                    //update the board, check if it works, then revert it
                    board[x, y] = player;
                    if(IsWinner(player, out int[] _))
                    {
                        board[x, y] = CellState.Empty;
                        move = new Vector2Int(x, y);
                        return true;
                    }
                    board[x, y] = CellState.Empty;
                }
            }

            move = Vector2Int.zero;
            return false;
        }

        /// <summary>
        /// Searches for winning combinations along the rows, columns, and both diagonals
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool IsWinner(CellState player, out int[] cells)
        {
            //columns
            for(int x = 0; x < 3; x++)
            {
                if(board[x, 0] == player && board[x, 1] == player && board[x, 2] == player)
                {
                    cells = new int[]{ x, x + 3, x + 6 };
                    return true;
                }
            }

            //rows
            for(int y = 0; y < 3; y++)
            {
                if(board[0, y] == player && board[1, y] == player && board[2, y] == player)
                {
                    cells = new int[]{ 3 * y, 3 * y + 1, 3 * y + 2 };
                    return true;
                }
            }

            //diagonals
            if(board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            {
                cells = new int[]{ 0, 4, 8 };
                return true;
            }
            if(board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            {
                cells = new int[]{ 6, 4, 2 };
                return true;
            }

            cells = new int[0];
            return false;
        }

        /// <summary>
        /// Represents a single ternary value on the board, either Empty, X or O
        /// </summary>
        public enum CellState
        {
            Empty,
            X,
            O
        }
	}
}
