using FluentMigrator;

namespace SourceName.Migrator.Migrations;

[Migration(2025040601)]
public class AddToDosTable : Migration 
{
    public override void Up()
    {
        Create.Table("todos")
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("created_by_user_id").AsGuid().NotNullable()
            .WithColumn("title").AsString().NotNullable()
            .WithColumn("is_completed").AsBoolean().NotNullable()
            .WithColumn("display_order").AsInt32().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("updated_at").AsDateTime().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("ToDos");
    }
}
