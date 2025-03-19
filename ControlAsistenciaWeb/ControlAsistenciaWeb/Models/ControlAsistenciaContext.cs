using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ControlAsistenciaWeb.Models;

public partial class ControlAsistenciaContext : DbContext
{
    public ControlAsistenciaContext()
    {
    }

    public ControlAsistenciaContext(DbContextOptions<ControlAsistenciaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<HorasTrabajada> HorasTrabajadas { get; set; }

    public virtual DbSet<RegistrosAsistencium> RegistrosAsistencia { get; set; }

    public virtual DbSet<TotalesAsistencium> TotalesAsistencia { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-K5L4N1T;Database=ControlAsistencia;User Id=sa;Password=12345678;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Empleado__3214EC27F48C3B16");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Departamento).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<HorasTrabajada>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HorasTra__3214EC27E01F982E");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.HorasTotales).HasMaxLength(30);
            entity.Property(e => e.TiempoTotal).HasMaxLength(30);

            entity.HasOne(d => d.Empleado).WithMany(p => p.HorasTrabajada)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado_Horas");
        });

        modelBuilder.Entity<RegistrosAsistencium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Registro__3214EC27D87DF9E8");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.Hora).HasMaxLength(200);
            entity.Property(e => e.Tipo).HasMaxLength(30);

            entity.HasOne(d => d.Empleado).WithMany(p => p.RegistrosAsistencia)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empleado");
        });

        modelBuilder.Entity<TotalesAsistencium>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.TotalEntrada)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.TotalSalida)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Empleado).WithMany()
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TotalesAsistencia_Empleados");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
