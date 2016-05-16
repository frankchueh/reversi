public class Data
    {
        int row;
        int column;
        public Data(int r, int c)
        {
            row = r;
            column = c;
        }
        public int getRow()
        {
            return row;
        }
        public int getColumn()
        {
            return column;
        }
        public int[] get()
        {
            return new int[] { row, column };
        }
    }