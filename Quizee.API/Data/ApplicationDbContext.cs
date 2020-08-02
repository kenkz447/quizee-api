using Microsoft.EntityFrameworkCore;
using Quizee.Modules.Models;
using Quizee.Modules.UserPermissions.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Quizee.API.Data
{
    public class ApplicationDbContext: UserPermissionsDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Model.GetEntityTypes()
               .Where(entityType => typeof(IModelBase).IsAssignableFrom(entityType.ClrType))
               .ToList()
               .ForEach(entityType =>
               {
                   var entityTypeBuilder = modelBuilder.Entity(entityType.ClrType);
                   if (entityTypeBuilder == null)
                   {
                       return;
                   }

                   // Add global filter: deletedBy
                   var parameter = Expression.Parameter(entityType.ClrType, "e");
                   var body = Expression.Equal(
                       Expression.Call(typeof(EF), nameof(EF.Property), new[] { typeof(string) }, parameter, Expression.Constant("DeletedBy")),
                       Expression.Constant(null));

                   entityTypeBuilder.HasQueryFilter(Expression.Lambda(body, parameter));
               });
        }
    }
}
