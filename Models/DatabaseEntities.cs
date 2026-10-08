namespace Anemette.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using MySql.Data.EntityFramework;

    //Databasen er MySQL (webhotellet giver kun én MSSQL-database). Modellen er beskrevet i kode
    //(tidligere AnemetteModel.edmx) og tabellerne oprettes med Database/Anemette-mysql.sql.
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class DatabaseEntities : DbContext
    {
        static DatabaseEntities()
        {
            //Tabellerne findes allerede - EF skal ikke forsøge at oprette eller migrere databasen
            Database.SetInitializer<DatabaseEntities>(null);
        }

        public DatabaseEntities()
            : base("name=DatabaseEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Tabelnavnene er i ental (tblProdukt, ikke tblProdukts)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<tblTjan>().ToTable("tblTjans");

            //Relationer (fremmednøgler) - alle er valgfrie
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblArtikel).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Artikel);
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblProdukt).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Produkt);
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblType).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Type);
            modelBuilder.Entity<tblPerson>().HasOptional(x => x.tblBillet).WithMany(x => x.tblPersons).HasForeignKey(x => x.FK_BilletType);
            modelBuilder.Entity<tblPerson>().HasOptional(x => x.tblEventRegion).WithMany(x => x.tblPersons).HasForeignKey(x => x.FK_Region_Hjemstavn);
            modelBuilder.Entity<tblNyhedsbrev>().HasOptional(x => x.tblPerson).WithMany(x => x.tblNyhedsbrevs).HasForeignKey(x => x.FK_Person);
            modelBuilder.Entity<tblPerson>().HasOptional(x => x.tblSted).WithMany(x => x.tblPersons).HasForeignKey(x => x.FK_HvorKenderViPersonenFra);
            modelBuilder.Entity<tblPerson>().HasOptional(x => x.tblTjan).WithMany(x => x.tblPersons).HasForeignKey(x => x.FK_TjansUnderEvent);
            modelBuilder.Entity<tblSvar>().HasOptional(x => x.tblPerson).WithMany(x => x.tblSvars).HasForeignKey(x => x.FK_Person);
            modelBuilder.Entity<tblTilmelding>().HasOptional(x => x.tblPerson).WithMany(x => x.tblTilmeldings).HasForeignKey(x => x.FK_Person);
            modelBuilder.Entity<tblProdukt>().HasOptional(x => x.tblProduktKategori).WithMany(x => x.tblProdukts).HasForeignKey(x => x.FK_ProductCategoryID);
            modelBuilder.Entity<tblTag>().HasOptional(x => x.tblSide).WithMany(x => x.tblTags).HasForeignKey(x => x.FK_Side);
            modelBuilder.Entity<tblSpm>().HasOptional(x => x.tblType).WithMany(x => x.tblSpms).HasForeignKey(x => x.FK_Type);
            modelBuilder.Entity<tblSvar>().HasOptional(x => x.tblSpm).WithMany(x => x.tblSvars).HasForeignKey(x => x.FK_Spm);
            modelBuilder.Entity<tblValg>().HasOptional(x => x.tblSpm).WithMany(x => x.tblValgs).HasForeignKey(x => x.FK_Spm);
            modelBuilder.Entity<tblSponsor>().HasOptional(x => x.tblSponsorType).WithMany(x => x.tblSponsors).HasForeignKey(x => x.FK_SponsorType);
            modelBuilder.Entity<tblValg>().HasOptional(x => x.tblSvar).WithMany(x => x.tblValgs).HasForeignKey(x => x.FK_Svar_DetKorrekteSvar);
            modelBuilder.Entity<tblSide>().HasOptional(x => x.tblType).WithMany(x => x.tblSides).HasForeignKey(x => x.FK_Type);
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblBestyrelse).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Bestyrelse);
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblPerson).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Person);
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblSide).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Side);
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblSponsor).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Sponsor);
            modelBuilder.Entity<tblBillede>().HasOptional(x => x.tblEvent).WithMany(x => x.tblBilledes).HasForeignKey(x => x.FK_Event);
            modelBuilder.Entity<tblEvent>().HasOptional(x => x.tblEventRegion).WithMany(x => x.tblEvents).HasForeignKey(x => x.FK_Region);
            modelBuilder.Entity<tblEvent>().HasOptional(x => x.tblSponsor).WithMany(x => x.tblEvents).HasForeignKey(x => x.FK_Sponsor);
            modelBuilder.Entity<tblEvent>().HasOptional(x => x.tblType).WithMany(x => x.tblEvents).HasForeignKey(x => x.FK_Type);
            modelBuilder.Entity<tblTilmelding>().HasOptional(x => x.tblEvent).WithMany(x => x.tblTilmeldings).HasForeignKey(x => x.FK_Event);
        }

        public virtual DbSet<tblAdmin> tblAdmins { get; set; }
        public virtual DbSet<tblArtikel> tblArtikels { get; set; }
        public virtual DbSet<tblBestyrelse> tblBestyrelses { get; set; }
        public virtual DbSet<tblBillede> tblBilledes { get; set; }
        public virtual DbSet<tblBillet> tblBillets { get; set; }
        public virtual DbSet<tblEventRegion> tblEventRegions { get; set; }
        public virtual DbSet<tblNyhedsbrev> tblNyhedsbrevs { get; set; }
        public virtual DbSet<tblPerson> tblPersons { get; set; }
        public virtual DbSet<tblProdukt> tblProdukts { get; set; }
        public virtual DbSet<tblProduktKategori> tblProduktKategoris { get; set; }
        public virtual DbSet<tblSide> tblSides { get; set; }
        public virtual DbSet<tblSpm> tblSpms { get; set; }
        public virtual DbSet<tblSponsor> tblSponsors { get; set; }
        public virtual DbSet<tblSponsorType> tblSponsorTypes { get; set; }
        public virtual DbSet<tblSted> tblSteds { get; set; }
        public virtual DbSet<tblSvar> tblSvars { get; set; }
        public virtual DbSet<tblTag> tblTags { get; set; }
        public virtual DbSet<tblTilmelding> tblTilmeldings { get; set; }
        public virtual DbSet<tblTjan> tblTjans { get; set; }
        public virtual DbSet<tblType> tblTypes { get; set; }
        public virtual DbSet<tblValg> tblValgs { get; set; }
        public virtual DbSet<tblEvent> tblEvents { get; set; }
    }
}
