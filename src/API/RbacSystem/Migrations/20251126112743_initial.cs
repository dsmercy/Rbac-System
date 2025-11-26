using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RbacSystem.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    groupid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    groupname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.groupid);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    permissionid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    permissionname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.permissionid);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    roleid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    rolename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.roleid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    isactive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "grouproles",
                columns: table => new
                {
                    grouproleid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    groupid = table.Column<long>(type: "bigint", nullable: false),
                    roleid = table.Column<long>(type: "bigint", nullable: false),
                    assignedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grouproles", x => x.grouproleid);
                    table.ForeignKey(
                        name: "FK_grouproles_groups_groupid",
                        column: x => x.groupid,
                        principalTable: "groups",
                        principalColumn: "groupid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_grouproles_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "roleid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rolepermissions",
                columns: table => new
                {
                    rolepermissionid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    roleid = table.Column<long>(type: "bigint", nullable: false),
                    permissionid = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rolepermissions", x => x.rolepermissionid);
                    table.ForeignKey(
                        name: "FK_rolepermissions_permissions_permissionid",
                        column: x => x.permissionid,
                        principalTable: "permissions",
                        principalColumn: "permissionid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rolepermissions_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "roleid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usergroups",
                columns: table => new
                {
                    usergroupid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    userid = table.Column<long>(type: "bigint", nullable: false),
                    groupid = table.Column<long>(type: "bigint", nullable: false),
                    assignedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usergroups", x => x.usergroupid);
                    table.ForeignKey(
                        name: "FK_usergroups_groups_groupid",
                        column: x => x.groupid,
                        principalTable: "groups",
                        principalColumn: "groupid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usergroups_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userroles",
                columns: table => new
                {
                    userroleid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    userid = table.Column<long>(type: "bigint", nullable: false),
                    roleid = table.Column<long>(type: "bigint", nullable: false),
                    assignedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userroles", x => x.userroleid);
                    table.ForeignKey(
                        name: "FK_userroles_roles_roleid",
                        column: x => x.roleid,
                        principalTable: "roles",
                        principalColumn: "roleid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userroles_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_grouproles_groupid_roleid",
                table: "grouproles",
                columns: new[] { "groupid", "roleid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_grouproles_roleid",
                table: "grouproles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_groups_groupname",
                table: "groups",
                column: "groupname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_permissionname",
                table: "permissions",
                column: "permissionname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rolepermissions_permissionid",
                table: "rolepermissions",
                column: "permissionid");

            migrationBuilder.CreateIndex(
                name: "IX_rolepermissions_roleid_permissionid",
                table: "rolepermissions",
                columns: new[] { "roleid", "permissionid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_rolename",
                table: "roles",
                column: "rolename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usergroups_groupid",
                table: "usergroups",
                column: "groupid");

            migrationBuilder.CreateIndex(
                name: "IX_usergroups_userid_groupid",
                table: "usergroups",
                columns: new[] { "userid", "groupid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userroles_roleid",
                table: "userroles",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_userroles_userid_roleid",
                table: "userroles",
                columns: new[] { "userid", "roleid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "grouproles");

            migrationBuilder.DropTable(
                name: "rolepermissions");

            migrationBuilder.DropTable(
                name: "usergroups");

            migrationBuilder.DropTable(
                name: "userroles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
