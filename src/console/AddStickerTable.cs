using FluentMigrator;

namespace console{
    [Migration(2018052002)]
    public class AddStickerTable : Migration
    {
        public override void Down()
        {
            Delete.Table("Stickers");
        }

        public override void Up()
        {
            Create.Table("Stickers")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Number").AsInt32()
                .WithColumn("PlayerName").AsString()
                .WithColumn("Country").AsString();
        }
    }
}