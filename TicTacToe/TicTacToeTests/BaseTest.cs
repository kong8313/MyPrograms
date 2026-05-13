using TicTacToe;

namespace TicTacToeTests
{
    public abstract class BaseTest
    {
        protected const int RowsCnt = 20;
        protected const int ColumnsCnt = 20;

        protected readonly FieldConverter Converter;

        protected BaseTest()
        {
            Converter = new FieldConverter();
        }
    }
}