using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipas.Personel.API.Migrations
{
    public partial class LinkEmployeeToDepartment : Migration
    {
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            // Önce nullable olarak eklenir.
            // Mevcut personellere doğrudan 0 verilmesi önlenir.
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "int",
                nullable: true);

            // Departmanı boş olan eski personel kayıtları için
            // güvenli bir geçici departman oluşturulur.
            migrationBuilder.Sql(
                """
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM [Departments]
                    WHERE [Name] = N'Atanmamış'
                )
                BEGIN
                    INSERT INTO [Departments]
                    (
                        [Name],
                        [Description],
                        [IsActive],
                        [CreatedAt]
                    )
                    VALUES
                    (
                        N'Atanmamış',
                        N'Eski personel kayıtlarının geçici departmanı.',
                        0,
                        SYSUTCDATETIME()
                    );
                END;
                """);

            // Eski Employees.Department değerlerinden
            // henüz bulunmayan departmanlar oluşturulur.
            migrationBuilder.Sql(
                """
                INSERT INTO [Departments]
                (
                    [Name],
                    [Description],
                    [IsActive],
                    [CreatedAt]
                )
                SELECT DISTINCT
                    LTRIM(RTRIM([employee].[Department])),
                    NULL,
                    1,
                    SYSUTCDATETIME()
                FROM [Employees] AS [employee]
                WHERE
                    LTRIM(
                        RTRIM(
                            ISNULL(
                                [employee].[Department],
                                N''
                            )
                        )
                    ) <> N''
                    AND NOT EXISTS
                    (
                        SELECT 1
                        FROM [Departments] AS [department]
                        WHERE
                            [department].[Name] =
                            LTRIM(
                                RTRIM(
                                    [employee].[Department]
                                )
                            )
                    );
                """);

            // Personeller eski departman adına karşılık gelen
            // yeni departman kayıtlarına bağlanır.
            migrationBuilder.Sql(
                """
                UPDATE [employee]
                SET
                    [employee].[DepartmentId] =
                    [department].[Id]
                FROM [Employees] AS [employee]
                INNER JOIN [Departments] AS [department]
                    ON [department].[Name] =
                       LTRIM(
                           RTRIM(
                               [employee].[Department]
                           )
                       )
                WHERE
                    LTRIM(
                        RTRIM(
                            ISNULL(
                                [employee].[Department],
                                N''
                            )
                        )
                    ) <> N'';
                """);

            // Departmanı boş veya eşleşmeyen personeller
            // Atanmamış departmanına bağlanır.
            migrationBuilder.Sql(
                """
                UPDATE [employee]
                SET
                    [employee].[DepartmentId] =
                    [department].[Id]
                FROM [Employees] AS [employee]
                CROSS JOIN
                (
                    SELECT TOP (1) [Id]
                    FROM [Departments]
                    WHERE [Name] = N'Atanmamış'
                    ORDER BY [Id]
                ) AS [department]
                WHERE [employee].[DepartmentId] IS NULL;
                """);

            // Bütün mevcut kayıtlar eşleştirildikten sonra
            // DepartmentId zorunlu hâle getirilir.
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Eski metin sütunu ancak veri taşındıktan sonra silinir.
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            // Geri dönüşte eski Department sütunu
            // önce nullable olarak oluşturulur.
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Departman adları eski metin sütununa geri yazılır.
            migrationBuilder.Sql(
                """
                UPDATE [employee]
                SET
                    [employee].[Department] =
                    [department].[Name]
                FROM [Employees] AS [employee]
                INNER JOIN [Departments] AS [department]
                    ON [department].[Id] =
                       [employee].[DepartmentId];
                """);

            migrationBuilder.Sql(
                """
                UPDATE [Employees]
                SET [Department] = N''
                WHERE [Department] IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Employees");
        }
    }
}