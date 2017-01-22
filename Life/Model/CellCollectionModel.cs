using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Life.Model
{
    /// <summary>
    /// Model - отвечает за хранение прикладных данных.
    /// </summary>
    class CellCollectionModel
    {       
        #region Fields
        
        private List<CellNeighborhood> needToUpdateList = new List<CellNeighborhood>();
        private CellNeighborhood[,] cellArray;

        #endregion

        #region Properties

        private List<CellNeighborhood> NeedToUpdateList
        { get { return needToUpdateList; } set { needToUpdateList = value; } }
        public CellNeighborhood[,] CellArray
        {
            get { return cellArray; }
            private set
            {
                cellArray = value;
            }
        }
        public int RowCount
        { get; private set; }
        public int ColumnCount
        { get; private set; }      

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangleSidesLength">длина стороны квадрата</param>
        /// <param name="rowCount">количество строк</param>
        /// <param name="columnCount">количество столбцов</param>
        /// <param name="liveBrush">цвет живых клеток</param>
        /// <param name="deadBrush">цвет мертвых клеток</param>
        /// <param name="rndFill">заполнить случайно</param>
        /// <param name="deadToOneLive">сколько (примерно) мертвых клеток должно приходиться на одну живую в случае случайного заполнения</param>
        public CellCollectionModel(int rectangleSidesLength, int rowCount, int columnCount, Brush liveBrush, Brush deadBrush, bool rndFill = true, int deadToOneLive = 5)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            CellArray = CellNeighborhood.CreateCellArray(rectangleSidesLength, rowCount, columnCount, liveBrush, deadBrush);
            if (rndFill)
                RandomizeCellArray(deadToOneLive);
        }        

        #endregion

        #region Private methods

        /// <summary>
        /// Проверяет, должна ли клетка изменить состояние на следующем ходу.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool NeedToUpdate(CellNeighborhood cell)
        {
            int liveNeighborhoodCount = 0;
            foreach (Cell neighbor in cell.Neighborhood)
            {
                if (neighbor.Live)
                    liveNeighborhoodCount++;
            }
            //оживает только в случае трех живых соседей
            if (!cell.CurrentCell.Live)
            {
                if (liveNeighborhoodCount == 3)
                {
                    return true;
                }
            }
            //умирает, если не 2 и не 3 живых соседа
            else
            {
                if (liveNeighborhoodCount != 2 && liveNeighborhoodCount != 3)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Public methods
               
        /// <summary>
        /// Производит вычисление клеток, которым необходимо изменить состояние
        /// на следующем ходу и обновляет их.
        /// </summary>
        public void OneStep()
        {
            for (int row = 0; row < this.RowCount; row++)
            {
                for (int column = 0; column < this.ColumnCount; column++)
                {
                    CellNeighborhood c = this.CellArray[row, column];
                    if (NeedToUpdate(c))
                        NeedToUpdateList.Add(c);
                }
            }
            foreach (CellNeighborhood c in NeedToUpdateList)
                c.CurrentCell.Live = !c.CurrentCell.Live;

            NeedToUpdateList.Clear();
        }        

        /// <summary>
        /// Убивает все клетки.
        /// </summary>
        public void ClearCellArray()
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int column = 0; column < ColumnCount; column++)
                {
                    CellNeighborhood cell = CellArray[row, column];
                    cell.CurrentCell.Live = false;
                }
            }
        }

        /// <summary>
        /// Помещает живые и мертвые клетки случайно в массиве.
        /// </summary>
        /// <param name="deadToOneLive">сколько (примерно) мертвых клеток должно приходиться на одну живую</param>
        public void RandomizeCellArray(int deadToOneLive)
        {
            Random rand = new Random();
            for(int row = 0; row < RowCount; row++)
            {
                for(int column = 0; column < ColumnCount; column++)
                {
                    CellNeighborhood cell = CellArray[row, column];

                    if (rand.Next(deadToOneLive) == 0)
                        cell.CurrentCell.Live = true;
                    else
                        cell.CurrentCell.Live = false;
                }
            }
        }

        #endregion

        #region Nested types

        public class CellNeighborhood
        {
            #region Fields

            private Cell currentCell;
            private List<Cell> neighborhood;

            #endregion

            #region Properties

            public Cell CurrentCell { get { return currentCell; } set { currentCell = value; } }
            public List<Cell> Neighborhood { get { return neighborhood; } set { neighborhood = value; } }

            #endregion

            #region Constructors

            private CellNeighborhood() { }

            #endregion

            #region Public methods

            /// <summary>
            /// Составляет массив из новых клеток и связывает границы.
            /// </summary>
            /// <param name="rectangleSidesLength">длина стороны квадрата</param>
            /// <param name="rowCount">количество строк</param>
            /// <param name="columnCount">количество столбцов</param>
            /// <param name="liveBrush">цвет живых клеток</param>
            /// <param name="deadBrush">цвет мертвых клеток</param>
            /// <param name="deadToOneLive">сколько (примерно) мертвых клеток должно приходиться на одну живую в случае случайного заполнения</param>
            /// <returns></returns>
            public static CellNeighborhood[,] CreateCellArray(int rectangleSidesLength, int rowCount, int columnCount, Brush liveBrush, Brush deadBrush, int deadToOneLive = 5)
            {
                CellNeighborhood[,] cna = new CellNeighborhood[rowCount, columnCount];

                // добавление клеток
                for (int row = 0; row < rowCount; row++)
                {
                    for (int column = 0; column < columnCount; column++)
                    {
                        CellNeighborhood cell = new CellNeighborhood();
                        cell.CurrentCell = new Cell(rectangleSidesLength, row, column, liveBrush, deadBrush);
                        cna[row, column] = cell;
                    }
                }
                //привязка соседей
                for (int row = 0; row < rowCount; row++)
                {
                    for (int column = 0; column < columnCount; column++)
                    {
                        CellNeighborhood cell = cna[row, column];
                        cell.Neighborhood = new List<Cell>();
                        foreach (KeyValuePair<int, int> neighborID in cell.CurrentCell.RowColumnNeighborhood)
                        {
                            if (neighborID.Key >= 0 && neighborID.Value >= 0 &&
                                neighborID.Key < rowCount && neighborID.Value < columnCount)
                            {
                                cell.Neighborhood.Add(cna[neighborID.Key, neighborID.Value].CurrentCell);
                            }

                            //связка границ массива
                            else
                            {
                                int neighborRow = 0;
                                int neighborColumn = 0;
                                bool rowWasChanged = false;
                                bool columnWasChanged = false;

                                if (neighborID.Key == -1)
                                {
                                    neighborRow = rowCount - 1;
                                    rowWasChanged = true;
                                }
                                if (neighborID.Key == rowCount)
                                {
                                    neighborRow = 0;
                                    rowWasChanged = true;
                                }
                                if (neighborID.Value == -1)
                                {
                                    neighborColumn = columnCount - 1;
                                    columnWasChanged = true;
                                }
                                if (neighborID.Value == columnCount)
                                {
                                    neighborColumn = 0;
                                    columnWasChanged = true;
                                }


                                if (rowWasChanged && columnWasChanged)
                                    cell.Neighborhood.Add(cna[neighborRow, neighborColumn].CurrentCell);
                                else if (rowWasChanged && !columnWasChanged)
                                    cell.Neighborhood.Add(cna[neighborRow, neighborID.Value].CurrentCell);
                                else if(!rowWasChanged && columnWasChanged)
                                    cell.Neighborhood.Add(cna[neighborID.Key, neighborColumn].CurrentCell);
                                else
                                    throw new Exception("!changedRow && !changedColumn");
                            }

                        }
                    }
                }
                return cna;
            }

            #endregion
        }

        public class Cell
        {
            #region Fields

            private bool live = false;
            private Rectangle rectCell = null;
            private int rowID;
            private int columnID;
            private List<KeyValuePair<int, int>> rowColumnNeighborhood = new List<KeyValuePair<int, int>>(8);
            private Brush deadBrush;
            private Brush liveBrush;

            #endregion

            #region Properties

            public bool Live
            {
                get { return live; }
                set
                {
                    if (value)
                    {
                        RectCell.Fill = liveBrush;
                    }
                    else
                    {
                        RectCell.Fill = deadBrush;
                    }

                    live = value;
                }
            }
            public Rectangle RectCell
            {
                get
                {
                    return rectCell;
                }
                private set
                {
                    rectCell = value;
                }
            }
            public int RowID
            { get { return rowID; } private set { rowID = value; } }
            public int ColumnID
            { get { return columnID; } private set { columnID = value; } }
            public List<KeyValuePair<int, int>> RowColumnNeighborhood
            { get { return rowColumnNeighborhood; } private set { rowColumnNeighborhood = value; } }

            #endregion

            #region Constructors

            public Cell(int rectangleSidesLength, int row, int column, Brush liveBrush, Brush deadBrush, bool live = false)
            {
                RectCell = new Rectangle();
                RectCell.Width = rectangleSidesLength;
                RectCell.Height = rectangleSidesLength;
                this.liveBrush = liveBrush;
                this.deadBrush = deadBrush;
                Live = live;
                SavePlace(row, column);
            }

            #endregion

            #region Private methods

            private void SavePlace(int row, int column)
            {
                this.RowID = row;
                this.ColumnID = column;

                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row, column + 1));
                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row, column - 1));
                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row + 1, column));
                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row - 1, column));
                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row + 1, column + 1));
                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row - 1, column - 1));
                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row - 1, column + 1));
                this.RowColumnNeighborhood.Add(new KeyValuePair<int, int>(row + 1, column - 1));
            }

            #endregion
        }

        #endregion
    }


}
