using System.Text;

namespace GINR;
public class Table
{
    private List<object> _columns { get; set; }
    private List<object[]> _rows { get; set; }

    public Table(params string[] columns) {
        if (columns == null || columns.Length == 0) {
            throw new System.ArgumentException("Parameter cannot be null nor empty", "columns");
        }

        _columns = new List<object>(columns);
        _rows = new List<object[]>();
    }

    private List<int> GetColumnsMaximumStringLengths() {
        List<int> columnsLength = new List<int>();

        for (int i = 0; i < _columns.Count; i++) {
            List<object> columnRow = new List<object>();
            int max = 0;

            columnRow.Add(_columns[i]);

            for (int j = 0; j < _rows.Count; j++) {
                columnRow.Add(_rows[j][i]);
            }

            for (int n = 0; n < columnRow.Count; n++) {
                int len = columnRow[n].ToString().Length;

                if (len > max) {
                    max = len;
                }
            }

            columnsLength.Add(max);
        }

        return columnsLength;
    }

    public void AddRow(params object[] values) 
    {
        if (values == null) {
            throw new System.ArgumentException("Parameter cannot be null", "values");
        }

        if (values.Length != _columns.Count) {
            throw new Exception("The number of values in row does not match columns count.");
        }

        _rows.Add(values);
    }  

    public override string ToString() {
        StringBuilder tableString = new StringBuilder();
        List<int> columnsLength = GetColumnsMaximumStringLengths();

        var rowStringFormat = Enumerable
            .Range(0, _columns.Count)
            .Select(i => " | {" + i + ",-" + columnsLength[i] + "}")
            .Aggregate((total, nextValue) => total + nextValue) + " |";

        string columnHeaders = string.Format(rowStringFormat, _columns.ToArray());
        List<string> results = _rows.Select(row => string.Format(rowStringFormat, row)).ToList();

        int maximumRowLength = Math.Max(0, _rows.Any() ? _rows.Max(row => string.Format(rowStringFormat, row).Length) : 0);
        int maximumLineLength = Math.Max(maximumRowLength, columnHeaders.Length);

        string dividerLine = string.Join("", Enumerable.Repeat("-", maximumLineLength - 1));
        string divider = $" {dividerLine} ";

        tableString.AppendLine(divider);
        tableString.AppendLine(columnHeaders);

        foreach (var row in results) {
        tableString.AppendLine(divider);
        tableString.AppendLine(row);
        }

        tableString.AppendLine(divider);

        return tableString.ToString();
    }

    public void Print() 
    {
        Console.WriteLine(ToString());
    }
}
