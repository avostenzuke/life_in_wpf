using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Life.Model;

namespace Life.ViewModel
{

    /// <summary>        
    //ViewModel - отвечает за реакцию на события пользовательского интерфейса и взаимодействие с моделью.
    //ViewModel имеет ссылку на Model но не должна иметь ссылки на View.
    /// </summary>
    class MainWindowViewModel 
    {
        #region Fields

        public delegate void NextStepDelegate();

        private CellCollectionModel cellCollection;
        private List<Rectangle> rectangleList;
        private int rowCount;
        private int columnCount;
        private int updateSleepMs;

        private Command startStopCommand;
        private Command oneStepCommand; 
        private Command createNewRandCommand;
        private Command clearCommand;
  
        private int rectangleSidesLength;
        private int borderSize;

        private bool enabled;
        private bool userInput;
        private CellCollectionModel.Cell cellWasChanged;

        #endregion

        #region Properties

        public List<Rectangle> RectangleList
        {
            get { return rectangleList; }
            private set
            {
                rectangleList = value;
            }
        }
        public int HeightRectangleCollection
        {
            get 
            {
                return rowCount * rectangleSidesLength + rowCount * borderSize;
            }
        }
        public int WidthRectangleCollection
        {
            get
            {
                return columnCount * rectangleSidesLength + columnCount * borderSize; 
            }
        }
                      
        #region Commands

        public Command StartStopCommand
        {
            get
            {
                if (startStopCommand == null)
                    startStopCommand = new Command(act => StartStop());
                return startStopCommand;
            }
        }
        public Command OneStepCommand
        {
            get
            {
                if (oneStepCommand == null)
                    oneStepCommand = new Command(act => OneStep());
                return oneStepCommand;
            }
        }
        public Command CreateNewRandCommand
        {
            get
            {
                if (createNewRandCommand == null)
                    createNewRandCommand = new Command(act => CreateNewRand());
                return createNewRandCommand;
            }
        }
        public Command ClearCommand
        {
            get
            {
                if (clearCommand == null)
                    clearCommand = new Command(act => Clear());
                return clearCommand;
            }
        }
                
        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderSize">ширина пропуска между квадратами</param>
        /// <param name="rectangleSidesLength">длина стороны квадрата</param>
        /// <param name="rowCount">количество строк</param>
        /// <param name="columnCount">количество столбцов</param>
        /// <param name="liveBrush">цвет живых клеток</param>
        /// <param name="deadBrush">цвет мёртвых клеток</param>
        public MainWindowViewModel(int borderSize, int rectangleSidesLength, int rowCount, int columnCount, int updateSleepMs, Brush liveBrush, Brush deadBrush)
        {
            this.rectangleSidesLength = rectangleSidesLength;
            this.borderSize = borderSize;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.updateSleepMs = updateSleepMs;
            cellCollection = new CellCollectionModel(rectangleSidesLength, rowCount, columnCount, liveBrush, deadBrush);
            InitRectangleList();
        }

        #endregion

        #region Private methods
        
        /// <summary>
        /// Добавляет в RectangleList прямоугольники, которые являются свойствами клеток в cellCollection.CellArray.
        /// Устанавливает для них обработку событий мыши и координаты на Canvas.
        /// </summary>
        private void InitRectangleList()
        {
            RectangleList = new List<Rectangle>();
            foreach (CellCollectionModel.CellNeighborhood cell in cellCollection.CellArray)
            {
                Rectangle rectangle = cell.CurrentCell.RectCell;
              
                Canvas.SetTop(rectangle, cell.CurrentCell.RowID * (rectangle.Height + borderSize));
                Canvas.SetLeft(rectangle, cell.CurrentCell.ColumnID * (rectangle.Width + borderSize));
                RectangleList.Add(rectangle);
            
                rectangle.MouseLeftButtonDown += rectangle_MouseLeftButtonDown;
                rectangle.MouseLeftButtonUp += rectangle_MouseLeftButtonUp;
                rectangle.MouseEnter += rectangle_MouseEnter;
            }
        }

        private void rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
                if (!userInput)
                    return;
                if(e.LeftButton == MouseButtonState.Released)
                {
                    userInput = false;
                    return;
                }

                Rectangle r = sender as Rectangle;
                CellCollectionModel.Cell c = GetCellMatchWithRectangle(r);

                if (c != cellWasChanged)
                {
                    cellWasChanged = c;
                    c.Live = !c.Live;
                }
        }

        #region RectangleMouseEvents

        private void rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { 
            enabled = false;
            userInput = true;                     
        }

        private void rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle r = sender as Rectangle;
            CellCollectionModel.Cell c = GetCellMatchWithRectangle(r);

            if (userInput)
            {
                if (c != cellWasChanged)
                {
                    c.Live = !c.Live;
                }
            }
            else
            {
                c.Live = !c.Live;
            }

            cellWasChanged = null;
            userInput = false;
        }

        #endregion

        /// <summary>
        /// Находит в cellCollection.CellArray клетку, которая совпадает по координатам.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private CellCollectionModel.Cell GetCellMatchWithRectangle(Rectangle r)
        {
            int row = (int)Canvas.GetTop(r) / ((int)r.Height + borderSize);
            int column = (int)Canvas.GetLeft(r) / ((int)r.Width + borderSize);            
            return cellCollection.CellArray[row, column].CurrentCell;
        }
                
        private void Clear()
        {
            enabled = false;
            cellCollection.ClearCellArray();
        }

        private void CreateNewRand(int deadToOneLive = 5)
        {
            enabled = false;
            cellCollection.RandomizeCellArray(deadToOneLive);
        }

        private void OneStep()
        {
            enabled = false;
            cellCollection.OneStep();
        }

        private void StartStop()
        {
            enabled = !enabled;                       

            if (enabled)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                       new NextStepDelegate(Go));
            }
        }

        private void Go()
        {
            if (enabled)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                                        new NextStepDelegate(cellCollection.OneStep));
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                                        new NextStepDelegate(Go));
                DateTime stop = DateTime.Now.AddMilliseconds(updateSleepMs);
                while (DateTime.Now < stop) { }
            }
            else
                return;
        }

        #endregion

    }
}
