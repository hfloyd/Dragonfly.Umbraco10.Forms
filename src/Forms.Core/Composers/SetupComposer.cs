#pragma warning disable 1591


namespace Dragonfly.UmbracoForms.Composers
{
    using Dragonfly.UmbracoForms.FieldTypes;
    using Dragonfly.UmbracoForms.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Umbraco.Cms.Core.Composing;
    using Umbraco.Cms.Core.DependencyInjection;
    using Umbraco.Forms.Core.Providers;
    using Umbraco.Forms.Core.Services;

    public class SetupComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddMvcCore().AddRazorViewEngine();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
          builder.Services.AddHttpContextAccessor();

            //builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            //    .Add<LogWorkflow>();

            builder.WithCollectionBuilder<FieldCollectionBuilder>()
                .Add<LikertGrid>()
                .Add<LikertItem>()
                .Add<LongAnswerNoLabel>()
                .Add<NetPromoter>(); 


            builder.Services.AddScoped<DependencyLoader>();
            //builder.Services.AddScoped<IFormService>();
            //builder.Services.AddScoped<IRecordReaderService>();
            //builder.Services.AddScoped<IFieldTypeStorage>();
            builder.Services.AddScoped<NetPromoterHelperService>();

            //builder.AddUmbracoOptions<Settings>();

        }

    }

}