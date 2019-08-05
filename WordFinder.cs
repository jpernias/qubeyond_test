using System;
using System.Collections.Generic;
using System.Linq;
using qubeyond_test.Domain;

namespace qubeyond_test
{
    public class WordFinder
    {
        private int _wordSize = 0;
        private string[,] _matrix;
        private string[,] _matrixInverse;
        private List<string> _wordOcurrenceList = new List<string>();
        private IEnumerable<string> _wordStream;

        private int _maxDeep = 20;
        public WordFinder(IEnumerable<string> matrix)
        {
            GenerateMatrix(matrix);
        }
        public IEnumerable<string> Find(IEnumerable<string> wordStream)
        {

            if (wordStream != null && wordStream.Any())
            {
                if (_matrix == null && _matrix.Length == 0)
                {
                    throw new Exception("Matrix is empty!");
                }

                //All string contain the same number of characters.
                _wordSize = wordStream.First().Count();

                int height = _matrix.GetLength(0);
                int width = _matrix.GetLength(1);

                _wordStream = wordStream;

                int level = 0;
                int colIndex = 0;
                int rowIndex = 0;
                bool end = false;
                Explore(ref rowIndex, ref colIndex, null, ref level, ref end);
            }
            return _wordOcurrenceList;
        }

        private void GenerateMatrix(IEnumerable<string> matrix)
        {
            if (matrix != null && matrix.Any())
            {
                //The matrix size doesn't exceed 64x64
                short maxSize = 64 * 64;
                if (maxSize < string.Join("", matrix).Count())
                {
                    throw new Exception("Max matrix size should be 64x64");
                }

                //All string contain the same number of characters.
                //Assumption: each string is a row of the matrix
                int rows = matrix.Count();
                int columns = matrix.First().Count();
                _matrix = new string[rows, columns];
                _matrixInverse = new string[rows, columns];
                //Fill the matrix
                var timer = System.Diagnostics.Stopwatch.StartNew();
                int y = 0;
                foreach (string row in matrix)
                {
                    int x = 0;
                    foreach (char item in row.ToCharArray())
                    {
                        _matrix[y, x] = item.ToString();
                        _matrixInverse[rows - 1 - y, columns - 1 - x] = item.ToString();
                        x++;
                    }

                    y++;
                }
                timer.Stop();
                Console.WriteLine("Total time matrix creation: " + timer.ElapsedMilliseconds.ToString());
            }
        }

        private void Explore(ref int rowIndex, ref int columnIndex, Direction? direction, ref int level, ref bool end)
        {
            try
            {

                if (!direction.HasValue)
                {
                    foreach (string x in Enum.GetNames(typeof(Direction)).ToList())
                    {
                        level = 0;
                        end = false;
                        columnIndex = 0;
                        rowIndex = 0;

                        while (level < _maxDeep)
                        {
                            Explore(ref rowIndex, ref columnIndex, (Direction)Enum.Parse(typeof(Direction), x), ref level, ref end);
                            if (end)
                            {
                                if (rowIndex + _wordSize >= _matrix.GetLength(0) && columnIndex + _wordSize >= _matrix.GetLength(1))
                                {
                                    break;
                                }
                                else
                                {
                                    level = 0;
                                }
                            }
                        }
                        List<WordOcurrence> result = _wordOcurrenceList.GroupBy(f => f).Select(f => new WordOcurrence
                        {
                            Word = f.Key,
                            Ocurrence = f.Count()
                        }).OrderByDescending(f => f.Ocurrence).ToList();
                    }
                }
                else
                {
                    string word = string.Empty;
                    string[,] localMatrix;

                    switch ((int)direction.Value)
                    {
                        #region 
                        case (int)Direction.Left:
                        case (int)Direction.Right:

                            if ((int)Direction.Left == (int)direction.Value)
                            {
                                localMatrix = _matrix;
                            }
                            else
                            {
                                localMatrix = _matrixInverse;
                            }

                            for (int x = columnIndex; x < columnIndex + _wordSize; x++)
                            {
                                if ((columnIndex + _wordSize) - 1 >= localMatrix.GetLength(1) )
                                {
                                    if (rowIndex < localMatrix.GetLength(0) - 1)
                                    {
                                        rowIndex++;
                                        columnIndex = 0;
                                        end = true;
                                        level = 0;
                                        Explore(ref rowIndex, ref columnIndex, direction, ref level, ref end);
                                    }
                                }
                                else
                                {
                                    word = word + (localMatrix[rowIndex, x]);
                                }

                            }

                            if ((word.Length == _wordSize) && _wordStream.Any(x => x == word))
                            {
                                _wordOcurrenceList.Add(word);

                            }

                            columnIndex++;

                            if (level <= _maxDeep)
                            {
                                level++;
                                Explore(ref rowIndex, ref columnIndex, direction, ref level, ref end);
                            }
                            else
                            {
                                level = 0;
                                end = true;
                            }

                            break;
                        #endregion
                        case (int)Direction.Up:
                        case (int)Direction.Down:
                            if ((int)Direction.Up == (int)direction.Value)
                            {
                                localMatrix = _matrixInverse;
                            }
                            else
                            {
                                localMatrix = _matrix;
                            }

                            for (int x = rowIndex; x < rowIndex + _wordSize; x++)
                            {
                                if ((rowIndex + _wordSize) - 1 >= localMatrix.GetLength(0))
                                {
                                    if (columnIndex < localMatrix.GetLength(1) - 1)
                                    {
                                        columnIndex++;
                                        end = true;
                                        level = 0;
                                        rowIndex = 0;
                                        Explore(ref rowIndex, ref columnIndex, direction, ref level, ref end);
                                    }
                                }
                                else
                                {
                                    word = word + (localMatrix[x, columnIndex]);
                                }
                            }


                            if ((word.Length == _wordSize) && _wordStream.Any(i => i == word))
                            {
                                _wordOcurrenceList.Add(word);

                            }

                            rowIndex++;

                            if (level <= _maxDeep)
                            {
                                level++;
                                Explore(ref rowIndex, ref columnIndex, direction, ref level, ref end);
                            }
                            else
                            {
                                level = 0;
                                end = true;
                            }

                            break;
                    }
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                end = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}