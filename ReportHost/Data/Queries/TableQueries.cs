using System.Data.Entity;

using ReportHost.Data.Context;
using ReportHost.Data.Entities;

namespace ReportHost.Data.Queries;

public static class TableQueries
{
    public static IQueryable<Table> GetTables(this IQueryable<Table> tbls)
    {
        var qry = tbls.Include(x => x.Schema);
        qry = qry.Where(x => x.IsMSShipped == false);
        qry = qry.Where(x => x.ObjectType == "U");
        return qry;
    }

    public static async Task<IEnumerable<Column>> GetTableColumnsAsync(this IQueryable<Table> tbl, string tableName, string schemaName = "dbo")
    {
        var qry = tbl.Include(x => x.Columns);
        qry = qry.Include(x => x.Schema);
        qry = qry.Where(x => x.Name == tableName);
        qry = qry.Where(x => x.Schema.Name == schemaName);
        var table = await qry.FirstOrDefaultAsync();
        return table?.Columns;
    }

    public static IQueryable<Column> GetTableColumns(this ReportContext db, string tableName, string schemaName = "dbo")
    {
        var qry = db.Tables.GetTables();
        qry = qry.Where(x => x.Name == tableName);
        qry = qry.Where(x => x.Schema.Name == schemaName);
        var table = qry.FirstOrDefault();

        var columns = table.Columns;

        return columns.AsQueryable();
    }
}

