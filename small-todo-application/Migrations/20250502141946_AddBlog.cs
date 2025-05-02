using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace small_todo_application.Migrations
{
    /// <inheritdoc />
    public partial class AddBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


			migrationBuilder.CreateTable(
		 name: "BlogPosts",
		 columns: table => new
		 {
			 Id = table.Column<int>(type: "int", nullable: false)
				 .Annotation("SqlServer:Identity", "1, 1"),
			 Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
			 CreatedByUserId = table.Column<int>(type: "int", nullable: false),
			 Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
			 ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
			 ImageMimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
			 CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
		 },
		 constraints: table =>
		 {
			 table.PrimaryKey("PK_BlogPosts", x => x.Id);
		 });
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.CreateIndex(
                name: "IX_TaskList_CreatedByUserId",
                table: "TaskList",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskList_Registers_CreatedByUserId",
                table: "TaskList",
                column: "CreatedByUserId",
                principalTable: "Registers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
