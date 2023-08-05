namespace SampleApp.Infra.Data;

public abstract class CrudEntityConfig<TEntity>: IEntityTypeConfiguration<TEntity>
    where TEntity: CrudEntity
{

    public virtual void Configure(EntityTypeBuilder<TEntity> entity)
    {
        entity.HasKey(p => p.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.ModifiedDate);

        entity.Property(e => e.ModifiedBy)
            .HasMaxLength(50);

        entity.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        entity.Property(e => e.DeletedDate);

        //entity.HasQueryFilter(p => p.IsDeleted==false);
    }
}
