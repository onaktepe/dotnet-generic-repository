using SampleApp.Domain;

namespace SampleApp.Infra.Data;

public class TodoConfig : CrudEntityConfig<Todo>
{
    public override void Configure(EntityTypeBuilder<Todo> entity)
    {
        base.Configure(entity);

        entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

         entity.Property(e => e.Priority)
            .IsRequired();    

        entity.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(20); //New, InProgress, Completed

        entity.HasIndex(e => e.Status).HasDatabaseName("IX_TODO_STATUS");

    }
}
