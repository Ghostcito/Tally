using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SoftWC.Migrations
{
    /// <inheritdoc />
    public partial class MigraciónInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Apellido = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Correo = table.Column<string>(type: "text", nullable: true),
                    TipoCliente = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Servicio",
                columns: table => new
                {
                    ServicioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreServicio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoServicio = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    PrecioBase = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DuracionEstimada = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicio", x => x.ServicioId);
                });

            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    TurnoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreTurno = table.Column<string>(type: "text", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.TurnoId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sede",
                columns: table => new
                {
                    SedeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Nombre_local = table.Column<string>(type: "text", nullable: true),
                    Direccion_local = table.Column<string>(type: "text", nullable: true),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Provincia = table.Column<string>(type: "text", nullable: true),
                    Latitud = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Longitud = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Radio = table.Column<decimal>(type: "numeric", nullable: false),
                    estadoSede = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sede", x => x.SedeId);
                    table.ForeignKey(
                        name: "FK_Sede_Cliente_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Cliente",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Apellido = table.Column<string>(type: "text", nullable: true),
                    DNI = table.Column<string>(type: "text", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NivelAcceso = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Salario = table.Column<decimal>(type: "numeric", nullable: true),
                    ServicioId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Servicio_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicio",
                        principalColumn: "ServicioId");
                });

            migrationBuilder.CreateTable(
                name: "Asistencia",
                columns: table => new
                {
                    IdAsistencia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HoraSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric", nullable: false),
                    Presente = table.Column<bool>(type: "boolean", nullable: false),
                    Observacion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencia", x => x.IdAsistencia);
                    table.ForeignKey(
                        name: "FK_Asistencia_AspNetUsers_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    IdEvaluacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<string>(type: "text", nullable: false),
                    TipoEmpleado = table.Column<string>(type: "text", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Responsabilidad = table.Column<int>(type: "integer", nullable: false),
                    Puntualidad = table.Column<int>(type: "integer", nullable: false),
                    CalidadTrabajo = table.Column<int>(type: "integer", nullable: false),
                    UsoMateriales = table.Column<int>(type: "integer", nullable: false),
                    Actitud = table.Column<int>(type: "integer", nullable: false),
                    EvaluadorId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.IdEvaluacion);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_AspNetUsers_EvaluadorId",
                        column: x => x.EvaluadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Evaluaciones_AspNetUsers_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    IdPermiso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpleado = table.Column<string>(type: "text", nullable: true),
                    HoraSalida = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraRetorno = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.IdPermiso);
                    table.ForeignKey(
                        name: "FK_Permisos_AspNetUsers_IdEmpleado",
                        column: x => x.IdEmpleado,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Supervision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SupervisorId = table.Column<string>(type: "text", nullable: false),
                    EmpleadoId = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Supervision_AspNetUsers_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Supervision_AspNetUsers_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioSede",
                columns: table => new
                {
                    SedesSedeId = table.Column<int>(type: "integer", nullable: false),
                    UsuariosId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSede", x => new { x.SedesSedeId, x.UsuariosId });
                    table.ForeignKey(
                        name: "FK_UsuarioSede_AspNetUsers_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioSede_Sede_SedesSedeId",
                        column: x => x.SedesSedeId,
                        principalTable: "Sede",
                        principalColumn: "SedeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioTurno",
                columns: table => new
                {
                    UsuarioTurnoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<string>(type: "text", nullable: false),
                    TurnoId = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioTurno", x => x.UsuarioTurnoId);
                    table.ForeignKey(
                        name: "FK_UsuarioTurno_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioTurno_Turno_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "Turno",
                        principalColumn: "TurnoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_IdEmpleado",
                table: "Asistencia",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ServicioId",
                table: "AspNetUsers",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_EvaluadorId",
                table: "Evaluaciones",
                column: "EvaluadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdEmpleado",
                table: "Evaluaciones",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_IdEmpleado",
                table: "Permisos",
                column: "IdEmpleado");

            migrationBuilder.CreateIndex(
                name: "IX_Sede_ClienteId",
                table: "Sede",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Supervision_EmpleadoId",
                table: "Supervision",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Supervision_SupervisorId",
                table: "Supervision",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSede_UsuariosId",
                table: "UsuarioSede",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioTurno_TurnoId",
                table: "UsuarioTurno",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioTurno_UsuarioId",
                table: "UsuarioTurno",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencia");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Evaluaciones");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Supervision");

            migrationBuilder.DropTable(
                name: "UsuarioSede");

            migrationBuilder.DropTable(
                name: "UsuarioTurno");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Sede");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Turno");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Servicio");
        }
    }
}
