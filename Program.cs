using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.RazorPages;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
});
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();



app.MapGet("/", () => "Hello World!");
app.MapGet("/postgis", async(HttpContext context,[FromServices]AppDbContext appDbContext,int? page) => {
    if(page is null || page == 0)
    {
        page = 1;
    }
    var data = await appDbContext.VgFlurs.Skip(page.Value*30).Take(30).ToListAsync();
    Debug.WriteLine($"{data.First().Geom.AsText()}");
    Console.WriteLine($"{page}");
    var responseData = new List<VgFlurDto>();
    Parallel.ForEach(data, (v) =>{
        responseData.Add(new(v));
    });
    return Results.Json(responseData);
});
app.MapPost("/postgis", (HttpContext context,[FromServices]AppDbContext appDbContext,[FromBody] string body) => {

});

app.Run();


public class AppDbContext: DbContext
{

    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // PostgreSQL mit NetTopologySuite konfigurieren
        optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=postgres;Username=postgres;Password=postgis",
            o => o.UseNetTopologySuite());
    }

    public DbSet<MySpatialEntity> SpatialEntities { get; set; }
    public DbSet<VgFlur> VgFlurs {get;set;}
    public DbSet<VgGemarkung> VgGemarkungs {get;set;}  
}


public class MySpatialEntity
{
    public int Id { get; set; }
    
    public NetTopologySuite.Geometries.Point Location { get; set; }
}
public class VgFlurDto
{
    [JsonPropertyName("WKT")]
    public string WKt{ get; set; }
    [JsonPropertyName("OID")]
    public string Oid{ get; set; }
    public VgFlurDto(VgFlur vgFlur)
    {
     this.WKt = vgFlur.Geom.AsText();
     this.Oid = vgFlur.Oid;   
    }
}
[Table("vg_gemarkung", Schema = "public")]
    public class VgGemarkung
    {
        [Key]
        [Column("oid")]
        [Required] // nicht null
        [MaxLength(255)] // Maximal 255 Zeichen (falls notwendig)
        public string Oid { get; set; }

        [Column("geom")]
        public Geometry Geom { get; set; } // Verwende 'string', um den Geo-Datentyp zu repräsentieren

        [Column("aktualit")]
        [MaxLength(11)] // Maximal 11 Zeichen
        public string Aktualit { get; set; }

        [Column("art")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Art { get; set; }

        [Column("name")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Name { get; set; }

        [Column("schluessel")]
        [MaxLength(9)] // Maximal 9 Zeichen
        public string Schluessel { get; set; }

        [Column("gemeinde")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Gemeinde { get; set; }

        [Column("gmdschl")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Gmdschl { get; set; }
    }
[Table("vg_flur", Schema = "public")]
    public class VgFlur
    {
        [Key]
        [Column("oid")]
        [Required] // nicht null
        [MaxLength(255)] // Maximal 255 Zeichen (falls notwendig)
        public string Oid { get; set; }

        [Column("geom")]
        public Geometry Geom { get; set; } // Verwende 'string', um den Geo-Datentyp zu repräsentieren

        [Column("aktualit")]
        [MaxLength(11)] // Maximal 11 Zeichen
        public string Aktualit { get; set; }

        [Column("art")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Art { get; set; }

        [Column("name")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Name { get; set; }

        [Column("schluessel")]
        [MaxLength(9)] // Maximal 9 Zeichen
        public string Schluessel { get; set; }

        [Column("gemeinde")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Gemeinde { get; set; }

        [Column("gmdschl")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Gmdschl { get; set; }

        [Column("uebobjekt")]
        [MaxLength(9)] // Maximal 9 Zeichen
        public string Uebobjekt { get; set; }

        [Column("ueboname")]
        [MaxLength(254)] // Maximal 254 Zeichen
        public string Ueboname { get; set; }
    }